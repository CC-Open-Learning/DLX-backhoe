/*
* FILE			: AnimateShader.cs
* PROJECT		: JWC VR Training
* PROGRAMMER	: Andy Sgro
* FIRST VERSION : Jan 20, 2020
* DESCRIPTION	: Animates objects in a loop.
*/

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : AnimateShader
	* PURPOSE : 
	*	- Defines the alpha of a shader over time.
*/
	public class AnimateShader : MonoBehaviour
	{
		//********//
		// fields //
		//********//

		[SerializeField] private ShaderProperty alphaProperty = null;
		[SerializeField] private float motionTweenDuration = 1;
		[Header("Keyframe values")]
		[SerializeField] private float[] scale = null;
		[SerializeField] private float[] alpha = null;

		private int frameCount = 0;
		private int frameIndex = 0;
		private float time = 0;
		private Transform trans = null;

		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
		 * \brief	Initialzes the fields.
		 *			Sets 'frameCount' to the smallest possible value
		 *			to prevent "Index not set to instance of array"
		 *			exceptions.
		 */
		private void Start()
		{
			frameCount = Mathf.Min(scale.Length, alpha.Length);
			trans = GetComponent<Transform>();
		}

		/**
		 * \brief	When this GameObject is disabled, reset the animation.
		 */
		private void OnDisable()
		{
			time = 0;
			frameIndex = 0;
		}

		/**
		 * \brief	Animates the alpha of the shader, depending on where
		 *			the playhead (frameIndex + time fields) is compared
		 *			to the frames.
		 */
		private void Update()
		{
			float normalTime = time / motionTweenDuration;

			// set scale
			float scaleFactor = Mathf.Lerp(scale[frameIndex], scale[frameIndex + 1], normalTime);
			trans.localScale = Vector3.one * scaleFactor;

			// set alpha
			alphaProperty.Alpha = Mathf.Lerp(alpha[frameIndex], alpha[frameIndex + 1], normalTime);

			// increment time and keyFrameIndex
			time += Time.deltaTime;
			if (time >= motionTweenDuration)
			{
				time %= motionTweenDuration;
				if (frameIndex < (frameCount - 2))
				{
					++frameIndex;
				}
				else
				{
					frameIndex = 0;
				}
			}
		}
	}
}