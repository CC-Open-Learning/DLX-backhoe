/*
 *	FILE		: ActivationFade.cs
 *	PROJECT		: CORE Engine
 *	AUTHOR		:
 *	DESCRIPTION	:
 *		The ActivationFade component can be added to a UI element
 *		in order to support fading in and out.
 *		
 *		Using the FadeIn() and FadeOut() methods will enable and disable
 *		the associated GameObject appropriately
 */

using System;
using System.Collections;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	[RequireComponent(typeof(CanvasGroup))]
	public class ActivationFade : MonoBehaviour
	{
		[Tooltip("The speed to fade the canvas in and out when setting active/inactive")]
		[SerializeField]
		private float fadeTime = 0.166f;

		[SerializeField]
		private bool fadeOnStart = true;

		private CanvasGroup group;

		private void Awake()
		{
			group = GetComponent<CanvasGroup>();
			OnEnable();
		}

		private void OnEnable()
		{
			if (fadeOnStart)
			{
				group.alpha = 0f;
				FadeIn();
			}
		}

		/// <summary>
		/// Fades alpha 
		/// </summary>
		IEnumerator Fade(float startAlpha, float endAlpha, Action callback)
		{
			float elapsedTime = 0.0f;
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.deltaTime;
				group.alpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeTime));
				yield return new WaitForEndOfFrame();
			}

			if (endAlpha == 0f && gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}

			callback?.Invoke();
		}


		/// <summary>
		/// Start a fade out
		/// </summary>
		public void FadeOut(Action callback = null)
		{
			if (!gameObject.activeSelf) { return; }

			StartCoroutine(Fade(1, 0, callback));
		}

		public void FadeOut()
        {
			FadeOut(null);
        }

		public void FadeIn(Action callback = null)
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}

			StartCoroutine(Fade(0, 1, callback));
		}

		public void FadeIn()
        {
			FadeIn(null);
        }

	}
}