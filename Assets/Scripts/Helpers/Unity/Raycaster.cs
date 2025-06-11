/*
 *  FILE          :	Raycaster.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Andy Sgro
 *  FIRST VERSION :	Jan 6, 2020
 *  DESCRIPTION   : A base raycaster class that contains a virtual function 
 *					called IntersectionAction() which lets you create custom behavior
 *					in child classes that specifies what happens when intersections are found.
 *					
 *					You can optionally have a LineRenderer component
 *					on the GameObject that this component is on so that you can see
 *					the raycast, but it isn't required.
 */

using RemoteEducation;
using UnityEngine;
using System.Linq;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : Raycaster
	* PURPOSE : 
	*	- A base raycaster class that contains a virtual function 
	 *	  called IntersectionAction() which lets you create custom behavior
	 *	  in child classes that specifies what happens when intersections are found.
	 *	- This class exists so that raycast logic doesn't need to be duplicated every single time
	 *	  it's reused in a class.
*/
	public class Raycaster : MonoBehaviour
	{
		//*********************//
		// fields + properties //
		//*********************//

		[SerializeField] private float maxDistance = 20;
		[SerializeField] private bool allowOtherLayersToBlockRay = false;
		[SerializeField] private string[] layerNames = null;

		public Collider hitCollider { get; private set; } = null;
		public int invisibleLayers { get; private set; } = Constants.UNDEFINED;
		protected int[] layers { get; private set; } = null;
		private LineRenderer laser = null;
		private int targetLayerMask = Constants.UNDEFINED;
		private float lineLength = Constants.UNDEFINED;



		//***************************//
		// non-monobehaviour methods //
		//***************************//

		/**
		* \brief	This method specifies the action that is performed when
		*			an intersection is found.
		*			
		* \details	Do not call the base method when overriding this.
		*			Doing so would hide the return statement.
		*			
		* \param	RaycastHit hitInfo : The details of the raycast collision.
		* 
		* \return	True is returned if the intersection is a valid one,
		*			otherwise false is returned.
		*/
		protected virtual bool IntersectionAction(RaycastHit hitInfo)
		{
			return true;
		}

		/**
		 * \brief	Truncates the visible line renderer if the intersection is valid.
		 *
		 * \param	RaycastHit hitInfo : Information about the raycast collision.
		 */
		private void TruncateVisibleLine(RaycastHit hitInfo)
		{
			lineLength = hitInfo.distance;
			hitCollider = hitInfo.collider;
		}



		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
		 * \brief	Makes the laser visible.
		 */
		private void OnEnable()
		{
			if (laser != null)
				laser.enabled = true;
		}

		/**
		 * \brief	Makes the laser invisible.
		 */
		private void OnDisable()
		{
			if (laser != null)
				laser.enabled = false;
		}

		/**
		 * \brief	Initilizes this component.
		 */
		private void Awake()
		{
			// convert layers from strings to ints
			layers = new int[layerNames.Length];
			for (int i = 0; i < layerNames.Length; ++i)
			{
				layers[i] = LayerMask.NameToLayer(layerNames[i]);
			}

			// create layerMasks
			invisibleLayers = LayerMask.GetMask("TransparentFX", "Ignore Raycast", "PostProcessing", "GazeTarget");
			targetLayerMask = LayerMask.GetMask(layerNames);

			laser = GetComponent<LineRenderer>();
		}

		/**
		 * \brief	Shoots rays and updates the 'hitCollider' field
		 *			to let other classes know which GameObject this
		 *			component is pointing at. Additionally, when a
		 *			ray hits a target that's on a valid collision layer,
		 *			IntersectionAction() gets called, which may be overriden
		 *			by polymorphic child classes of this component.
		 */
		private void FixedUpdate()
		{
			// calculate intersection
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hitInfo;
			bool hit = allowOtherLayersToBlockRay ?
					   Physics.Raycast(ray, out hitInfo, maxDistance, ~invisibleLayers, QueryTriggerInteraction.Ignore) :
					   Physics.Raycast(ray, out hitInfo, maxDistance, targetLayerMask, QueryTriggerInteraction.Ignore);
			lineLength = maxDistance;

			if (hit)
			{
				TruncateVisibleLine(hitInfo);
				RaycastTarget raycastTarget = hitInfo.collider.GetComponent<RaycastTarget>();

				if (allowOtherLayersToBlockRay)
				{
					if (layers.Contains(hitInfo.transform.gameObject.layer))
					{
						if (raycastTarget != null)
						{
							raycastTarget.HitByRay(hitInfo, this);
						}
						IntersectionAction(hitInfo);
					}
				}
				else
				{
					if (raycastTarget != null)
					{
						raycastTarget.HitByRay(hitInfo, this);
					}
					IntersectionAction(hitInfo);
				}
			}

			// render the line 
			if (laser != null)
			{
				laser.positionCount = 2;
				Vector3 laserEnd = transform.position + transform.forward * lineLength;
				Vector3[] points = { transform.position, laserEnd };
				laser.SetPositions(points);
			}
		}
	}
} 