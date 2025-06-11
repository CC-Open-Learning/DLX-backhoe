/*
FILENAME		: StateMachine.cs
PROJECT			: JWC Project
PROGRAMMERS		: Andy Sgro
FIRST VERSION	: Nov 18, 2019
DESCRIPTION		: The abstract base state machine.
*/

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : StateMachine
	* PURPOSE : 
	*	- You can quickly create custom state machines by inheriting
	*	  this class and overriding the StepActions() method.
	*	- When you override the StepActions() method, use a switch-case
	*	  on the 'step' field member to define what happens on each step,
	*	  and when the preconditions are for getting of the next/previous
	*	  steps.
*/
	public abstract class StateMachine : MonoBehaviour
	{
		//*********************//
		// fields & properties //
		//*********************//
		[Tooltip("The instructions that the player reveives with each step. The number of prompts should match the number of steps.")]
		[SerializeField] private string[] prompts = null;

		[Tooltip("The objects that get conditionally enabled/disabled depending on the step.")]
		[SerializeField] private Transforms[] hints = null;

		protected static StateMachine m { get; private set; } = null;
		private float timeOnStep = 0;
		private static bool JustGotOnStep { get { return (m.timeOnStep < 10); } }

		protected int step { get; private set; } = 0;
		protected int Step
		{
			get { return step; }
			set
			{
				if (value > step)
				{
					//PlayerPrompt.Prompt(prompts[value]);
				}
				step = value;
			}
		}

		private int NumSteps
		{
			get { return prompts.Length; }
		}



		//***************************//
		// non-monobehaviour methods //
		//***************************//

		/**
		 * \brief	Override this.
		 *			Use the 'step' field in a switch-case
		 *			to determine the actions that happen
		 *			in each step, and the conditions for the next/
		 *			previous steps.
		 */
		protected abstract int StepActions();

		/**
		 * \brief	Enables/disables the hints and goes to next step.
		 * 
		 * \return	Returns true if we are able to go to the next step,
		 *			returns false if we can't (due to already being on the last step, or
		 *			not being on the correct step).
		 */
		public bool GotoStep(int currentStep, int nextStep)
		{
			if ((step >= NumSteps) | (currentStep != step))
			{
				return false;
			}

			ShowHints(false);
			Step = nextStep;
			ShowHints();

			return true;
		}


		/**
		 * \brief	Activates / deactivates all of the hints on the current step.
		 * 
		 * \param	bool option : Whether to activate or deactivate the components.
		 * 
		 * \return	Returns the number of non-null transforms that were in the list.
		 *			Returns -1 if the current step's hints were null.
		 */
		protected int ShowHints(bool option = true)
		{
			if ((step < hints.Length) && (hints[step] != null))
				return Transforms.ActivateAll(ref hints[step].transforms, option);
			return -1;
		}

		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
		 * \brief	Prompts the player with the instruciton for the first step.
		 */
		protected virtual void Start()
		{
			Step = 0;
			m = this;
		}

		/**
		 * \brief	Checks to see if it's time to go to next step,
		 *			and resets the timeOnStep field if we went to a new step.
		 *			
		 *			Make sure to call this if overriden.
		 */
		protected virtual void LateUpdate()
		{
			int prevStep = Step;
			GotoStep(step, StepActions());
			if (Step != prevStep)
			{
				timeOnStep = 0;
			}

			timeOnStep += Time.deltaTime;
		}
	}
}
