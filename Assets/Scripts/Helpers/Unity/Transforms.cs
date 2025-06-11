/*
* FILE			: Transforms.cs
* PROJECT		: BroseVRTraining
* PROGRAMMER	: Andy Sgro
* FIRST VERSION : July 22, 2019
* DESCRIPTION	: This file adds simple, much needed funcitons that
*				  help with managing arrays and lists of Transforms.
*/

using RemoteEducation.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : Transforms
	* PURPOSE : 
	*	- This file adds simple, much needed funcitons that
	*	  help with managing arrays and lists of Transforms.
*/
	public class Transforms : MonoBehaviour
	{
		//********//
		// fields //
		//********//
		public List<Transform> transforms = new List<Transform>();


		//***************************//
		// non-monobehaviour methods //
		//***************************//

		public static Transform[] GetTransforms(Component[] components)
		{
			Transform[] transforms = new Transform[components.Length];
			for (int i = 0; i < components.Length; ++i)
			{
				transforms[i] = components[i].transform;
			}
			return transforms;
		}

		/**
		* \brief	Determines which transform (in the array) is
		*			closest to the 'primaryObj' argument.
		*
		* \param	Transform primaryObj : The positions of the Transforms in the array
		*								   get compared to this transform's position.
		* \param	Transform otherObj	 : This method checks which of these transforms
		*								   is clostest to the 'primaryObj'.
		*			
		* \return	Returns the transform (int the array) is
		*			closest to the 'primaryObj' argument.
		*/
		public static Transform Closest(Transform primaryObj, Transform[] otherObj)
		{
			int objCount = otherObj.Length;
			Transform closestObj = otherObj[0];
			float minDist = Vector3.Distance(primaryObj.position, closestObj.position);

			for (int i = 1; i < objCount; ++i)
			{
				float newDist = Vector3.Distance(primaryObj.position, otherObj[i].position);
				if (newDist < minDist)
				{
					minDist = newDist;
					closestObj = otherObj[i];
				}
			}
			return closestObj;
		}


		/**
		* \brief	Activates/deactivates all transforms in a list.
		*
		* \param	ref List<Transform> transforms : The transforms to activate/deactivate.
		* \param	bool option                    : Whether to activate/deactivate them.
		* 
		* \return	Returns the number of non-null transforms that were in the list.
		*			Returns -1 if the 'transforms' argument was null.
		*/
		public static int ActivateAll(ref List<Transform> transforms, bool option)
		{
			if (transforms == null)
				return -1;

			int count = 0;
			foreach (Transform trans in transforms)
			{
				if (trans != null)
				{
					trans.gameObject.SetActive(option);
					++count;
				}
			}
			return count;
		}

		/**
		* \brief	Gets first immidiate child by index.
		* 
		*			The index is simply a number that's inside the gameobject's name.
		*			
		* \param	Transform parent : The parent to search through.
		* \param	int index        : The index to match.
		* 
		* \return	Returns first immidiate child that has the specified index.
		*/
		public static Transform GetChildByIndex(Transform parent, int index)
		{
			foreach (Transform child in parent)
			{
				if (child.parent == parent.transform)
				{
					int nameIndex = 0;
					if (child.name.GetInt(out nameIndex) && (nameIndex == index))
					{
						return child;
					}
				}
			}

			return null;
		}

		/**
		* \brief	Gets immidiate children by name.
		*			
		* \param	Transform parent : The parent to search through.
		* \param	string name      : The name to match.
		* 
		* \return	Returns all children that have the specified name.
		*/
		public static Transform[] GetChildrenByName(Transform parent, string name)
		{
			var list = new List<Transform>();

			foreach (Transform child in parent)
			{
				if ((child.parent == parent.transform) && child.name.Equals(name))
				{
					list.Add(child);
				}
			}

			return list.ToArray();
		}
	}
}