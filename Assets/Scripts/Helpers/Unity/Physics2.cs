/*
* FILE			: UberPhysics.cs
* PROJECT		: GAS Final Project
* PROGRAMMER	: Andy Sgro
* FIRST VERSION : April 17, 2019
* DESCRIPTION	: This file contains the definition of the Physics class, 
*				  which gives vector/trig math functions to all classes.
*/

using System;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : Physics2
	* PURPOSE : 
	*	- This static class gives vector/trig math functions to all classes.
	*	- This is modeled after the Math class in many libraries.
	*	- The domain of this class:
	*		- Trig angle/distance vector math.
	*		- Pool table physics (Newton's 'energy transfer' mechanics aren't covered yet:()
	*		- Orbital mechanics (Einstien's 'general relativity' is not covered)
	*	- This is used by any class that wants to do easy trig/physics math.
	*	- It is recommended that you create non-static classes that abstract these methods.
*/
	public static class Physics2
	{

		#region static_public_properties

		public enum Direction
		{
			Top = 0,
			Right = 90,
			Bottom = 180,
			Left = 270
		}

		public const double INVERSE_RADIAN = (Math.PI / 180);
		public const double RADIAN = (180 / Math.PI);

		#endregion



		#region circle_bouncing




		/**
		* \brief	Simulates a pool ball bouncing off another. 
		* 
		* \details	This function does not simulate Newton's 'energy transfer' mechanics,
		*			so objects always retain their speed rather than transfering them.
		*			If Newton's Cradle used this function, then only one side would bounce.
		*
		* \param	Vector2 velocity : The velocity of the object that is colliding with the other.
		* \param	Vector2 thisPos	 : The position of the object that is colliding with the other.
		* \param	Vector2 otherPos : The position of the object that is being collided with.
		*
		* \return	Returns a vector representing the new velocity of the object represented by
		*			the first two arguments.
		*/
		public static Vector2 CircleBounce(Vector2 velocity, Vector2 thisPos, Vector2 otherPos) // tested
		{
			float movementAngle = GetAngle(velocity);
			float speed = GetSpeed(velocity);
			float angleToOther = GetAngle(thisPos, otherPos);

			return CircleBounce(movementAngle, speed, angleToOther);
		}




		/**
		* \brief	Simulates a pool ball bouncing off a hard (vertically/horizontally aligned) wall.
		*
		* \param	Vector2 velocity        : The velocity of the object that is colliding with the wall.
		* \param	Direction wallPlacement : The placement of the wall, relative to the object colliding
		*										with it.
		*
		* \return	Returns a vector representing the new velocity of the object specified by
		*			the first argument.
		*/
		public static Vector2 CircleBounce(Vector2 velocity, Direction wallPlacement) // tested
		{
			float angle = GetAngle(velocity);
			float speed = GetSpeed(velocity);

			return CircleBounce(angle, speed, (float)wallPlacement);
		}




		/**
		* \brief	Simulates a pool ball bouncing off another ball/wall. 
		* 
		* \details	This function does not simulate Newton's 'energy transfer' mechanics,
		*			so objects always retain their speed rather than transfering them.
		*			If Newton's Cradle used this function, then only one side would bounce.
		*
		* \param	float motionAngle  : The angle of the object that is colliding with the other.
		* \param	float speed	       : The speed of the object that is colliding with the other.
		* \param	float angleToOther : The placement of the other, relative to the object colliding with it.
		*
		* \return	Returns a vector representing the new velocity of the object represented by
		*			the first two arguments.
		*/
		public static Vector2 CircleBounce(float motionAngle, float speed, float angleToOther) // tested
		{
			float reverseMotionAngle = FlipAngle(motionAngle);
			float angleFromOther = FlipAngle(angleToOther);
			float delta = Roll(angleFromOther - reverseMotionAngle);
			float newMotionAngle = Roll((reverseMotionAngle + (delta * 2)));

			return GetVelocity(newMotionAngle, speed);
		}


		#endregion



		#region orbital_mechanics



		/**
		* \brief	Returns a motion vector representing the effects of radial gravity 
		*			around a point.
		*			
		* \details	Call this when an object's origin point is inside the radius of the gravity field,
		*			then add the value returned from this function to the motion vector of that object.
		*
		* \param	Vector2 selfPos     : The position of the object that is being affected by gravity.
		* \param	Vector2 gravityPos	: The position of the gravitational object.
		* \param	float gravityRadius	: The radius of the gravity field.
		* \param	float scale			: A modifier for the strength of the gravity.
		*
		* \return	Returns a motion vector representing the effects of radial gravity 
		*			around a point.
		*/
		public static Vector2 GetGravityVector(Vector2 selfPos, Vector2 gravityPos, float gravityRadius, float scale = 1)
		{
			// angle points towards singularity
			// speed is based on inverse distance from the singularity

			float angle = GetAngle(selfPos, gravityPos);
			float speed = (GetInverseDistance(selfPos, gravityPos, gravityRadius) / gravityRadius) * scale;

			return GetVelocity(angle, speed);
		}



		/**
		* \brief	Estimates the strength of gravity around a point,
		*			assuming that the strength of gravity increases 
		*			linearly as you get closer to it (which isn't the case).
		*
		* \param	Vector2 selfPos		 : The point the measure gravity from.
		* \param	Vector2 gravityField : The position of the gravityField.
		* \param	float radius		 : The radius of the gravity field.
		*
		* \return	Returns the strength of gravity at the point specified by the first argument.
		*/
		public static float GetInverseDistance(Vector2 selfPos, Vector2 gravityField, float radius)
		{
			float distance = GetDistance(gravityField, selfPos);
			return radius - distance;
		}

		#endregion



		#region translations


		/**
		* \brief	Translates a vector coordinate, given a direction and a magnitude.
		*
		* \param	Vector2 pos			 : The position to move from.
		* \param	float directionAngle : The direction to move towards.
		* \param	float distance		 : The distance to travel.
		*
		* \return	Returns a translated vector coordinate.
		*/
		public static Vector2 TranslateVector(Vector2 pos, float directionAngle, float distance)
		{
			Vector2 vel = GetVelocity(directionAngle, distance);
			return new Vector2(pos.x + vel.x, pos.y + vel.y);
		}




		/**
		* \brief	Provides the coordinates so that a circle can
		*			take the shortest path towards touching the edge of another circle.
		*
		* \details	Might be good for making circle collision physics less glitchy.
		* 
		* \param	Vector2 selfPos	  : The position of the circle to change.
		* \param	float selfRadius  : The radius of the circle to change.
		* \param	Vector2 otherPos  : The position of the circle to touch.
		* \param	float otherRadius : The radius of the circle to touch.
		*
		* \return	Returns the coordinates so that a circle can touch the edge of another circle.
		*/
		public static Vector2 TouchCircle(Vector2 selfPos, float selfRadius, Vector2 otherPos, float otherRadius)
		{
			float distance = selfRadius + otherRadius;
			float angleFromOther = GetAngle(otherPos, selfPos);
			return TranslateVector(otherPos, angleFromOther, distance);
		}




		/**
		* \brief	This is the same as TouchCircle(), but assumes that both circles are
		*			calling this function. Feel free to make a Newtonian physics simulation
		*			and tell me which of these two functions actually help out...
		*			In all fairness, there's probably a better solution than using these functions.
		*/
		public static Vector2 TouchCircleHalf(Vector2 selfPos, Vector2 otherPos, float selfRadius, float otherRadius)
		{
			float distance = (selfRadius + otherRadius) / 2;
			float angleFromOther = GetAngle(otherPos, selfPos);
			return TranslateVector(otherPos, angleFromOther, distance);
		}


		#endregion



		#region getters



		/**
		* \brief	Returns the angle between two postions.
		*
		* \details	The returned angle is relative to the first argument.
		*			Returns degrees, not radians.
		*
		* \param	Vector2 thisPos	 : The first position.
		* \param	Vector2 otherPos : The second position.
		*
		* \return	Returns the angle between two postions.
		*/
		public static float GetAngle(Vector2 thisPos, Vector2 otherPos)
		{
			return GetAngle(new Vector2(otherPos.x - thisPos.x, otherPos.y - thisPos.y));
		}


		/**
		* \brief	Returns the angle of a velocity.
		*
		* \details	Velocity is speed + direction, the returns the direction.
		*			Returns degrees, not radians.
		*
		* \param	Vector2 velocity : The velocity to get the speed of.
		*
		* \return	Returns the angle (direction) of the velocity.
		*/
		public static float GetAngle(Vector2 velocity)
		{
			return (float)(Math.Atan2(velocity.x, velocity.y) * RADIAN);
		}


		/**
		* \brief	Returns the distance between two points.
		*
		* \param	Vector2 a : Point A.
		* \param	Vector2 b : Point B.
		*
		* \return	Returns the distance between two points.
		*/
		public static float GetDistance(Vector2 a, Vector2 b)
		{
			return (float)(Math.Sqrt(Math.Pow(a.y - b.y, 2) + Math.Pow(a.x - b.x, 2)));
		}



		/**
		* \brief	Returns the speed of a velocity.
		*
		* \param	Vector2 velocity : The velocity to get the speed of.
		*
		* \return	Returns the speed (magnitude) of a velocity.
		*/
		public static float GetSpeed(Vector2 velocity)
		{
			return (float)(Math.Sqrt(Math.Pow(velocity.y, 2) + Math.Pow(velocity.x, 2)));
		}



		/**
		* \brief	Returns the velocity of a vector, given two points and a magnitude.
		*
		* \details	The speed is relative to the first point.
		*			This method is useful for when objects
		*			need to follow others at a specified pace.
		*
		* \param	Vector2 thisPos	 : The point to measure from.
		* \param	Vector2 otherPos : The point to measure towards.
		* \param	float speed		 : The speed of the resulting vector.
		*
		* \return	Returns the velocity, given two points and a magnitude.
		*/
		public static Vector2 GetVelocity(Vector2 thisPos, Vector2 otherPos, float speed) // tested
		{
			float direction = GetAngle(thisPos, otherPos);
			return GetVelocity(direction, speed);
		}



		/**
		* \brief	Returns the velocity of a vector, given a speed and an angle.
		*
		* \details	This method is useful for when objects need
		*			to go in a specific direction.
		*
		* \param	Vector2 thisPos	 : The point to measure from.
		* \param	Vector2 otherPos : The point to measure towards.
		* \param	float speed		 : The speed of the resulting vector.
		*
		* \return	Returns the velocity, given a speed and an angle.
		*/
		public static Vector2 GetVelocity(float angle, float speed) // tested
		{
			float radian = CountRadians(Roll(angle));

			return new Vector2(
				(float)Math.Sin(radian) * speed,
				(float)Math.Cos(radian) * speed);
		}


		#endregion



		#region setters


		/**
		* \brief	Returns a vector with a new speed.
		*
		* \details	The returned vector will have the same direction
		*			as the one in the first argument.
		*
		* \param	Vector2 velocity : The vector to modify.
		* \param	float newSpeed	 : The speed that the vector will have.
		*
		* \return	Returns a vector with a new speed.
		*/
		public static Vector2 SetSpeed(Vector2 velocity, float newSpeed)
		{
			float angle = GetAngle(velocity);
			return GetVelocity(angle, newSpeed);
		}


		/**
		* \brief	Returns a vector with a new angle.
		*
		* \details	The returned vector will have the same magnitude
		*			as the one in the first argument.
		*
		* \param	Vector2 velocity : The vector to modify.
		* \param	float newAngle	 : The angle that the vector will have.
		*
		* \return	Returns a vector with a new angle.
		*/
		public static Vector2 SetAngle(Vector2 velocity, float newAngle)
		{
			float speed = GetSpeed(velocity);
			return GetVelocity(newAngle, speed);
		}
		#endregion



		#region private_support


		/**
		* \brief	Applies rollover to an angle so that it is between 
		*			0 and 359 degrees.
		*
		* \details	This helps the functions in the Physics class.
		*			They deal with angles a lot, and sometimes the angle HAS
		*			to be in the specified range. If you make a new Physics
		*			function, feel free to use this function AS MUCH as you want.
		*			-45 becomes 315 degrees.
		*			370 becomes 10 degrees.
		*
		* \param	float angle : The angle to rollover.
		*
		* \return	Returns an angle that is between 0 and 359 degrees.
		*/
		public static float Roll(float angle) // tested
		{
			angle %= 360;

			while (angle < 0)
			{
				angle += 360;
			}

			return angle;
		}



		public static float SignedRoll(float angle) // tested
		{
			angle %= 360;

			while (angle < -180)
			{
				angle += 360;
			}

			if (angle >= 180)
			{
				angle -= 360;
			}

			return angle;
		}




		/**
		* \brief	Expresses an angle as the number of radians it is.
		*
		* \param	float angle : The angle to convert.
		*
		* \return	Returns the number of radians that an angle is.
		*/
		private static float CountRadians(float angle) // tested
		{
			return (float)(INVERSE_RADIAN * Roll(angle));
		}



		/**
		* \brief	Flips an angle so it's pointing in the
		*			opposite direction.
		*
		* \param	float angle : The angle to flip.
		*
		* \return	Returns a flipped angle.
		*/
		public static float FlipAngle(float angle) // tested
		{
			return Roll(angle + 180);
		}


		#endregion
	}
}