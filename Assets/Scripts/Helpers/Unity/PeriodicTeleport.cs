/*
FILENAME		: PeriodicTeleport.cs
PROJECT			: JWC Project
PROGRAMMERS		: Andy Sgro & Mike Hilts
FIRST VERSION	: Nov 8, 2019
DESCRIPTION		: Periodically teleports a transform to a specified location.
				  Make sure this script gets executed last, otherwise this script's
				  effects may be inadvertantly negated by another more powerful script.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : PeriodicTeleport
	* PURPOSE : 
	*	- Periodically teleports a transform to a specified location.
*/
	public class PeriodicTeleport : MonoBehaviour
	{
		//********//
		// fields //
		//********//

		[SerializeField]
		private float secondsBetweenEachTeleport = 0;
		public Transform position;

		private float timeTillNextTeleport;

		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
		* \brief	Initializes the 'timeTillNextTeleport' data member.
		*/
		private void Start()
		{
			timeTillNextTeleport = secondsBetweenEachTeleport;
		}

		/**
		* \brief	Periodically teleports the transform.
		*/
		private void LateUpdate()
		{
			timeTillNextTeleport -= Time.deltaTime;
			if (timeTillNextTeleport <= 0)
			{
				timeTillNextTeleport = secondsBetweenEachTeleport + (-timeTillNextTeleport);
				transform.position = position.position;
			}
		}
	}
}