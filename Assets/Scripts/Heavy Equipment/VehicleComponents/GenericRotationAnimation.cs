/*
 * SUMMARY: The purpose of this class is to provide a generic animation script that can be used for any
 *          interior controls on the backhoe. The animation duration, angle of the rotation, pivot point, 
 *          and axis of rotation can all be edited in the Unity editor. Simply drag and drop this script 
 *          onto any interior control
 *          
 *          You will need to include:
 *          private GenericRotationAnimation genericRotationAnimation;
 *          
 *          In the Initialize method:
 *          genericRotationAnimation = GetComponent<GenericRotationAnimation>();
 *          genericRotationAnimation.SetAxisOfRotation();
 *          
 *          When you want to use the animation:
 *          StartCoroutine(genericRotationAnimation.RotationAnimation(bool toggleState));
 */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GenericRotationAnimation : MonoBehaviour
{
    [SerializeField, Tooltip("Pivot point which the control will pivot around")]
    private Transform pivotPoint;

    [SerializeField, Tooltip("The duration of the animation")]
    private float animationDuration;

    [SerializeField, Tooltip("Angle that the control rotates each iteration of the while loop during the animation")]
    private float angle;

    [SerializeField, Tooltip("Axis to rotate around")]
    private Axis axisOfRotation;

    /// <summary>Vector containing the required axis to rotate game object around</summary>
    private Vector3 currentAxisOfRotation;

    public bool IsAnimating { get; private set; } = false;

    /// <summary>Indicates which axis to rotate the control around to accommodate for different orientation controls in the scene</summary>
    private enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    public void ToggleIsAnimating()
    {
        IsAnimating = !IsAnimating;
    }

    /// <summary>Sets the private variable currentAxisOfRotation equal to a 1 unit vector3 of x, y, or z so the control 
    ///  rotates in the correct direction. Since game objects are oriented in different ways this cannot be hard coded</summary>
    public void SetAxisOfRotation()
    {
        switch (axisOfRotation)
        {
            case Axis.X:
                currentAxisOfRotation = new Vector3(1, 0, 0);
                break;

            case Axis.Y:
                currentAxisOfRotation = new Vector3(0, 1, 0);
                break;

            case Axis.Z:
                currentAxisOfRotation = new Vector3(0, 0, 1);
                break;
        }
    }

    /// <summary>Creates an animation of an interior control pivoting back and forth on click depending on control state</summary>
    public IEnumerator RotationAnimation(bool inDefaultPos)
    {
        IsAnimating = true;

        //Start timer and end timer for our animation
        float startTimer = Time.time;
        float endTimer = startTimer + animationDuration;

        // 1 if control is in the start position (true), -1 if control is already toggled (false)
        var direction = inDefaultPos ? 1 : -1;

        //While we are animating, animate the object
        while (Time.time <= endTimer)
        {
            transform.RotateAround(pivotPoint.position, currentAxisOfRotation, Time.deltaTime * direction * angle);
            yield return null;
        }

        IsAnimating = false;
    }
}