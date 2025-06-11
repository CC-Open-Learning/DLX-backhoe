/*
* FILE			: SpeedSensor.cs
* PROJECT		: BroseVRTraining
* PROGRAMMER	: Andy Sgro
* FIRST VERSION : Nov 26, 2019
* DESCRIPTION	: Outputs the velocity of the GameObject, in meters per second.
*/

using RemoteEducation.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : SpeedSensor
	* PURPOSE : 
	*	- Outputs the velocity of the GameObject, in meters per second.
*/
	public class SpeedSensor : MonoBehaviour
	{
		//*********************//
		// fields & properties //
		//*********************//
		[SerializeField] private bool xAxis = true;
		[SerializeField] private bool yAxis = true;
		[SerializeField] private bool zAxis = true;

		[Range(1, 32)]
		[Tooltip("Number of frames to sample the speed over.")]
		[SerializeField] private int smoothing = 32;

		private Vector3 lastPos;
		private Queue<float> buffer;

		public float Speed
		{
			get { return buffer.Average(); }
		}

		private Vector3 CurrentPosition
		{
			get { return transform.position.FlattenAxis( xAxis, yAxis, zAxis); }
		}


		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
		* \brief	Initializes the buffer (for collecting samples),
		*			plays the sound, and initializes the distance varianbles.
		*/
		private void Start()
		{
			buffer = new Queue<float>();
            for (int i = 0; i < smoothing; i++)
            {
				buffer.Enqueue(0f);
			}

			lastPos = CurrentPosition;
		}

		/**
		* \brief	Collects the normalized distance of the current frame,
		*			stores it in a buffer, and uses the average of all the samples
		*			in the buffer to write the new pitch and volume values.
		*/
		private void FixedUpdate()
		{
			buffer.Dequeue();
			// get speed, use reciprocal of deltaTime because we are reading distance, not writing it
			buffer.Enqueue(Vector3.Distance(CurrentPosition, lastPos) * (1 / Time.fixedDeltaTime));
			lastPos = CurrentPosition;
		}
	}
}