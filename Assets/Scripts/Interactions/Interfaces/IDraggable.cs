/*
*  FILE          :	IDraggable.cs
*  PROJECT       :	Core Engine (Interaction system)
*  PROGRAMMER    :	Duane Cressman
*  FIRST VERSION :	2020-7-15
*  DESCRIPTION   :  This file contains the IDraggable interface and Draggable class. 
*  
*  Class Description: This interface/Class is used to give a game objects the ability to be dragged. It uses a collider to be the 
*                     plane that the objects are dragged on. This collider set by the HydraulicsBoard. It will be toggled on and off 
*                     each time an object is being dragged.
*                     
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace RemoteEducation.Interactions
{
    public interface IDraggable
    {
        Draggable DraggableClass { get; }
    }

    public class Draggable
    {
        private GameObject TheGameObject; //The game object this class is on

        public bool DragJustHappened { get; private set; } //If the last time this object was clicked on, it was dragged.

        private string LayerName = "DraggablePlane"; //name of the layer the plane collider is on
        private int DraggableLayerMask;              //the layermask, used to raycast and only hit a specific layer
        private static List<Collider> PlaneColliders; //the planes that the objects get dragged on.

        private Vector3 ObjectsStartingPoint; //when a drag starts, where the object was
        private Vector3 MouseDownPoint;       //when a drag starts, where the mouse was
        private const float MinDragDistance = 0.002f; //how far the mouse must move before a drag starts
        private bool MouseDownPointHasBeenSet;

        private bool Frozen;
        
        /// <summary>
        /// Invoked when the user stops dragging the object.
        /// </summary>
        public Action OnDragCompleted;

        public static bool ObjectBeingDragged { get; private set; }

        public Draggable(GameObject gameObject)
        {
            TheGameObject = gameObject;

            SetupLayerMask();
        }

        /* Method Header : SetupLayerMask
         * This method will set up the layer mask. This is used in ray casting
         */
        private void SetupLayerMask()
        {
            int layerIndex = LayerMask.NameToLayer(LayerName);
            if (layerIndex == -1)
            {
                Debug.LogError("Unable to find DraggablePlane layer");
            }
            else
            {
                DraggableLayerMask = (1 << layerIndex);
            }
        }

        /* Method Header : SetPlaneCollider
         * This static method will be used by other objects to set the plane colliders that this 
         * class uses. 
         */
        public static void SetPlaneCollider(Collider collider)
        {
            if (PlaneColliders == null)
            {
                PlaneColliders = new List<Collider>();
            }

            PlaneColliders.Add(collider);
        }

        /* Method Header : SetPlaneCollider
        * Overload for public static void SetPlaneCollider(Collider collider)
        */
        public static void SetPlaneCollider(Collider[] colliders, bool overrideOldColliders)
        {
            if (overrideOldColliders)
            {
                PlaneColliders = new List<Collider>();
            }

            foreach (Collider collider in colliders)
            {
                SetPlaneCollider(collider);
            }
        }

        /* Method Header : GetPointUnderMouse
         * This method get the point under the mouse.
         * It will only find points that are on a collider that is in the PlaneColliders list and are
         * also on the layer specified in the layer mask.
         */
        public bool GetPointUnderMouse(out Vector3 output)
        {
            output = new Vector3();
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, DraggableLayerMask))
            {
                if (PlaneColliders.Contains(hit.collider))
                {
                    output = hit.collider.gameObject.transform.InverseTransformPoint(hit.point);

                    return true;
                }
            }

            return false;
        }

        #region OnMouse... Methods

        /* Method Header : OnMouseDown
         * Any class that uses the IDraggable interface MUST call this method
         * in the OnMouseDown method that is defined in MonoBehaviour.
         * 
         * This method will turn on all the plan colliders and mark where the drag started.
         */
        public void OnMouseDown()
        {
            //if the mouse isn't over a UI element
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                TogglePlaneColliders(true);

                DragJustHappened = false;
                MouseDownPointHasBeenSet = false;
                ObjectsStartingPoint = TheGameObject.transform.localPosition;

                if (GetPointUnderMouse(out Vector3 point))
                {
                    MouseDownPointHasBeenSet = true;
                    MouseDownPoint = point;
                }
            }
        }

        /* Method Header : OnMouseDrag
         * Any class that uses the IDraggable interface MUST call this method
         * in the OnMouseDown method that is defined in MonoBehaviour.
         * 
         * This method will first check if the mouse has been moved enough to register a drag. 
         * Once it has it will find the difference of where the mouse currently is and where it started.
         * The object being dragged will be moved by the same distance.
         */
        public void OnMouseDrag()
        {
            //if the mouse isn't over a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (GetPointUnderMouse(out Vector3 point) && !Frozen)
                {
                    if (!MouseDownPointHasBeenSet)
                    {
                        MouseDownPoint = point;
                        MouseDownPointHasBeenSet = true;
                    }

                    if (!DragJustHappened)
                    {
                        float squareLength = (point - MouseDownPoint).sqrMagnitude;

                        if (squareLength >= MinDragDistance)
                        {
                            DragJustHappened = true;
                            ObjectBeingDragged = true;
                        }
                    }
                    else
                    {
                        Vector3 difference = point - MouseDownPoint;

                        TheGameObject.transform.localPosition = ObjectsStartingPoint + difference;
                    }
                }
            }
        }

        /* Method Header : OnMouseUp
         * Any class that uses the IDraggable interface MUST call this method
         * in the OnMouseUp method that is defined in MonoBehaviour.
         * 
         * This method will turn off all the plane colliders.
         */
        public void OnMouseUp()
        {
            TogglePlaneColliders(false);
            ObjectBeingDragged = false;

            if(DragJustHappened)
            {
                OnDragCompleted?.Invoke();
            }
        }

        #endregion

        public void FreezeObject(bool trueOrFalse = true)
        {
            Frozen = trueOrFalse;
        }

        /* Method Header : TogglePlaneColliders
         * This method will toggle all of the plane colliders.
         */
        public static void TogglePlaneColliders(bool isActive)
        {
            if (PlaneColliders != null)
            {
                foreach (Collider collider in PlaneColliders)
                {
                    collider.enabled = isActive;
                }
            }
        }
    }
}