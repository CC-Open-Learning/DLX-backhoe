/*
FILENAME		: ShaderProperty.cs
PROJECT			: JWC Project
PROGRAMMERS		: Andy Sgro
FIRST VERSION	: Nov 26, 2019
DESCRIPTION		: This gives easy access to the alpha properties in Shader Graph materials.
*/


using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : SceneManagement
	* PURPOSE : 
	*	- This gives easy access to the alpha properties in Shader Graph materials.
*/
	public class ShaderProperty : MonoBehaviour
	{
		//*******************//
		// fields & property //
		//*******************//
		[SerializeField] private Renderer material = null;

		[Tooltip("Name of the alpha shader graph property.")]
		[SerializeField] private string alphaPropertyName = "";

		[Range(0, 1)]
		[SerializeField] private float quantity = 0;

		[SerializeField] private bool useClamp = false;
		[SerializeField] private Vector2 clamp = new Vector2(0, 1);

		private void LateUpdate()
		{
			if (useClamp)
			{
				float alpha = Alpha;
				if ((alpha < clamp.x) | (alpha > clamp.y))
				{
					Alpha = Mathf.Clamp(alpha, clamp.x, clamp.y);
				}
			}
		}

		private void Start()
		{
			Alpha = quantity;
		}

		public float Alpha
		{
			get { return material.material.GetFloat(alphaPropertyName); }
			set { material.material.SetFloat(alphaPropertyName, Mathf.Clamp01(value)); }
		}
	}
}