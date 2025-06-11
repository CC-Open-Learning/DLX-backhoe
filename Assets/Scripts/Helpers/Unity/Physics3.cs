/*
* FILE			: Physics3.cs
* PROJECT		: Socket League
* PROGRAMMER	: Andy Sgro
* FIRST VERSION : July 18, 2019
* DESCRIPTION	: This file contains the definition of the Physics class, 
*				  which gives vector/trig math functions to all classes.
*/

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : Physics3
	* PURPOSE : 
	*	- This static class gives vector/trig math functions to all classes.
	*	- This is modeled after the Math class in many libraries.
	*	- The domain of this class:
	*		- Trig angle/distance vector math.
	*	- This is used by any class that wants to do easy trig/physics math.
	*	- It is recommended that you create non-static classes that abstract these methods.
*/
	public static class Physics3
	{


		/**
		* \brief	Converts a normalized direction to an euler angle.
		* 
		* \param	Vector3 direction : The normalized direction to convert.
		*
		* \return	Returns an euler angle.
		*/
		public static Vector2 GetAngle(Vector3 direction)
		{
			return GetAngle(Vector3.zero, direction);
		}


		/**
		* \brief	Converts a normalized direction to an euler angle.
		*			
		* \details	The normalized direction is the delta between the two arguments.
		*			This is similar to a LookAt method.
		* 
		* \param	Vector3 from & to : These make up the normalized direction.
		*
		* \return	Returns an euler angle.
		*/
		public static Vector2 GetAngle(Vector3 from, Vector3 to)
		{
			// get y-value
			float yAxis = Physics2.GetAngle(new Vector2(to.x - from.x, to.z - from.z));

			// align the 'to' object so it's infront of the 'from' object (rotate its y-axis, pivoted to the 'from' object)
			float distance = Physics2.GetDistance(new Vector2(from.x, from.z), new Vector2(to.x, to.z));
			Vector3 tmpPoint = new Vector3(from.x, to.y, from.z + distance);

			// get x-value
			float xAxis = Physics2.GetAngle(new Vector2(tmpPoint.z - from.z, (tmpPoint.y - from.y))) - 90;

			return new Vector2(xAxis, yAxis);
		}


		/**
		* \brief	Flips an euler angle so it faces the opposite direction.
		*
		* \param	Vector2 angle : The euler angle to flip.
		*
		* \return	Returns the flipped euler angle.
		*/
		public static Vector2 FlipAngle(Vector2 angle)
		{
			return new Vector2(Physics2.FlipAngle(angle.x), Physics2.FlipAngle(angle.y));
		}



		/**
		* \brief	Models a relationship between distance (input) and magnitude (output).
		*			The magnitude increases linearly as the distance decreases.
		*				- The max possible magnitude is equal to the 'radius' argument.
		*				- The min possible magnitude is zero.
		*
		* \param	Vector2 selfPos & otherPos : These arguments determine the inputted distance.
		* \param	float otherRadius	       : Determines the 'area of influence' around the
		*										 otherPos, and the max magnitude.
		*
		* \return	Returns the resulting magnitude based on the distance between
		*			the two objects and the other.
		*/
		public static float InverseDistance(Vector3 selfPos, Vector3 otherPos, float otherRadius)
		{
			return otherRadius - Vector3.Distance(otherPos, selfPos);
		}

	}
}