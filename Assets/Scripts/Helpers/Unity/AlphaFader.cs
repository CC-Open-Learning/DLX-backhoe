/*
FILENAME		: AlphaFader.cs
PROJECT			: JWC Project
PROGRAMMERS		: Andy Sgro
FIRST VERSION	: Nov 20, 2019
DESCRIPTION		: Contains an array of ShaderProperties that can be faded in and out.
*/

using RemoteEducation;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : AlphaFader
	* PURPOSE : 
	*	- Contains an array of ShaderProperties that can be faded in and out.
*/
	public class AlphaFader : MonoBehaviour
	{
		//*****************************//
		//  enum, fields, & properties //
		//*****************************//

		public enum State
		{
			Stop,
			FadeIn,
			FadeOut
		}

		[HideInInspector] public State state = State.Stop;
		[SerializeField] private float fadeDuration = 0.5f;
		[SerializeField] private ShaderProperty[] shaderAlphas = null;
		private float fadeSpeed = Constants.UNDEFINED;

		//************//
		// properties //
		//************//
		public float Level
		{
			get
			{
				return shaderAlphas[0].Alpha;
			}
			set
			{
				foreach (var shaderAlpha in shaderAlphas)
				{
					shaderAlpha.Alpha = value;
				}
			}
		}

		private bool IsValid
		{
			get { return (fadeDuration > 0); }
		}


		//***************************//
		// non-monobehaviour methods //
		//***************************//

		/**
		 * \brief	Makes the shaders fade-in if they are invisible.
		 *			Makes the shaders fade-out if they are visible.
		 */
		public void Fade()
		{
			if (shaderAlphas[0].Alpha <= 0)
			{
				state = State.FadeIn;
			}
			else if (shaderAlphas[0].Alpha >= 1)
			{
				state = State.FadeOut;
			}
		}

		/**
		 * \brief	Adjusts the alpha of the shaders.
		 */
		public void AdjustLevel(float delta)
		{
			foreach (var shaderAlpha in shaderAlphas)
			{
				shaderAlpha.Alpha += delta;
			}
		}

		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
		 * \brief	Initializes the 'fadeSpeed' field to be the
		 *			reciprocal of the 'fadeDuration' field.
		 */
		private void Awake()
		{
			fadeSpeed = 1 / fadeDuration;
		}

		/**
		 * \brief	If the 'state' field is set to 'FadeIn', fade in the shaders.
		 *			If the 'state' field is set to 'FadeOut', fade out the shaders.
		 */
		private void Update()
		{
			if (!IsValid)
			{
				Debug.LogError(gameObject.name + "'s AlphaFader component failed to initialize");
			}
			else
			{
				switch (state)
				{
					case State.FadeIn:
						{
							bool allShadersHaveFadedIn = true;
							foreach (var shaderAlpha in shaderAlphas)
							{
								shaderAlpha.Alpha += (fadeSpeed * Time.deltaTime);
								if (shaderAlpha.Alpha < 1)
								{
									allShadersHaveFadedIn = false;
								}

							}
							if (allShadersHaveFadedIn)
							{
								state = State.Stop;
							}
							break;
						}

					case State.FadeOut:
						{
							bool allShadersHaveFadedOut = true;

							foreach (var shaderAlpha in shaderAlphas)
							{
								shaderAlpha.Alpha -= (fadeSpeed * Time.deltaTime);
								if (shaderAlpha.Alpha > 0)
								{
									allShadersHaveFadedOut = false;
								}
							}
							if (allShadersHaveFadedOut)
							{
								state = State.Stop;
							}
							break;
						}

				}
			}
		}
	}
}
