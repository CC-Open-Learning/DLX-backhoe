/*
 *  FILE          :	TaskCanvas.cs
 *  PROJECT       :	VARLab CORE Template
 *  PROGRAMMER    :	Chowon Jung
 *  FIRST VERSION :	2020-07-30
 *  DESCRIPTION   : This file contains the TaskCanvas class which is responsible for spawning and displaying
 *					in world instruction on canvas(called as Task Canvas) throughout the scenario.
 *                  The canvas object rotates itself tracking player's position.
 */

#region Using Statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation;
using TMPro;

#endregion

namespace RemoteEducation.UI
{
    public class TaskCanvas : MonoBehaviour
    {
        //-------------------------- Object spawn related properties Start
        private GameObject spawnPointObject;    // spawn position can be assigned as an object tagged <TaskCanvasSpawn>
        private Transform spawnPos;             // transform info of spawn position object
        private GameObject canvas;              // spawned canvas game object
        private string missingTarget = "TaskCanvas : Cannot find GameObject tagged <MainCamera>.";
        private string missingCanvasPrefab = "TaskCanvas : Task Canvas prefab is not assigned";
        private string missingSpawnPoint = "TaskCanvas : Cannot find GameObject tagged <TaskCanvasSpawn>. Spawn on Player.";
        // No references, commented out to suppress warning.
        //private string missingTaskList = "TaskCanvas : Failed to read task list in from Scenario Manager";
        //-------------------------- Object spawn related properties End


        //-------------------------- Player tracking related properties Start
        private GameObject Target;              // target to track
        private Vector3 velocity;
        private float rot;
        private float rotSpeed = 180f;
        //-------------------------- Player tracking related properties End



        // Start is called before the first frame update
        void Start()
        {
            // set target to track
            Target = GameObject.FindGameObjectWithTag("MainCamera");
        }



        // Update is called once per frame 
        void Update()
        {
            // TrackTarget();           < --------commented out for debugging purpose, may or may not be used in the future
        }



        #region Private Methods



        /*
         * FUNCTION      : TrackTarget()
         * DESCRIPTION   : This method have the task canvas tracks target object and rotate itself after the target.
         * PARAMETERS    : void
         * RETURN        : void
         */
        private void TrackTarget()
        {
            // get rotation to turn with tangent from x and z
            velocity = Target.transform.position - transform.position;
            float rotAmount = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

            // rotate by given speed and rotation
            rot = Mathf.LerpAngle(transform.eulerAngles.y, rotAmount, Time.deltaTime * rotSpeed);
            transform.eulerAngles = new Vector3(0, rot + 180, 0);   // + 180 on Y to invert front and back of the object
        }



        #endregion



        /*
         * FUNCTION      : SpawnTaskCanvas()
         * DESCRIPTION   : This method spawns and connects the task canvas object on the given position.
         * PARAMETERS    : GameObject taskCanvasPrefab  : task canvas prefab to spawn
         * RETURN        : void
         */
        public void SpawnTaskCanvas(GameObject taskCanvasPrefab)
        {
            // set target to track
            Target = GameObject.FindGameObjectWithTag("MainCamera");

            // log error when main camera object is missing
            if (Target == null)
            {
                Debug.LogError(missingTarget);
            }

            // set spawn point by tag <------------- may need to be modified in the future!
            spawnPointObject = GameObject.FindGameObjectWithTag("TaskCanvasSpawn");

            // get position to spawn canvas object
            if (spawnPointObject != null)
            {
                // spawn on object position when assigned
                spawnPos = spawnPointObject.transform;
            }
            else
            {
                // log error when spawn point object is missing
                Debug.LogError(missingSpawnPoint);

                // <------------ spawn at the main camera for debugging purpose since there's nothing settled on this yet
                spawnPos = Target.transform;
            }

            // spawn canvas object
            if (taskCanvasPrefab != null)
            {
                canvas = Instantiate(taskCanvasPrefab, spawnPos); // <----------- will be replaced with Scenebuilder related lines
            }
            else
            {
                // log error when canvas prefab is missing
                Debug.LogError(missingCanvasPrefab);
            }
        }



        /*
         * FUNCTION      : ConnectExistingTaskCanvas()
         * DESCRIPTION   : This method connects to the task canvas object that exists in world already.
         * PARAMETERS    : GameObject taskCanvas    : task canvas object passed in from ScenarioManager
         * RETURN        : void
         */
        public void ConnectExistingTaskCanvas(GameObject taskCanvas)
        {
            // connect existing task canvas object
            canvas = taskCanvas;
        }



        /*
         * FUNCTION      : UpdateTask()
         * DESCRIPTION   : This method updates task canvas text by the description passed in.
         * PARAMETERS    : string description   : task description to display, passed in from ScenarioManager
         * RETURN        : void
         */
        public void UpdateTask(string description)
        {
            if (canvas != null)
            {
                // display description passed in
                canvas.transform.Find("Text").GetComponent<TextMeshPro>().text = description;
            }
            else
            {
                Debug.LogError("TaskCanvas : Canvas object not assigned");
            }
        }
    }
}