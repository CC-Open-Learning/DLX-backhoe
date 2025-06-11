using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheckTypes = TaskVertexManager.CheckTypes;
using HeavyEquipmentCheckTypes = RemoteEducation.Modules.HeavyEquipment.HeavyEquipmentModule.HeavyEquipmentCheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Create a task that just shows the description and them completes after a set time.
        /// The <see cref="WaitTask"/> class is used to create these vertices. 
        /// The check on the vertex will return "True" when the wait is complete, so make sure
        /// there is an edge that can be taken with that value out of the returned vertex.
        /// </summary>
        /// <param name="key">A key for the task. You can just use the name of the vertex.</param>
        /// <param name="time">How long the task should take before completing.</param>
        /// <returns>The vertex that was created. The Title and Description still need to be set.</returns>
        private UserTaskVertex CreateWaitTask(string key, float time)
        {
            return new UserTaskVertex(typeof(WaitTask), (int)HeavyEquipmentCheckTypes.Wait, new Tuple<string, float>(key, time));
        }

        /// <summary>This class is used to make a task wait for a set amount of time in secondsbefore completing. </summary>
        /// <remarks>
        /// This class uses the ITaskable system to communicate the vertices.
        /// See the <see cref="CreateWaitTask(string, float)"/> method for how the vertices 
        /// should be set up. 
        /// Each vertex that needs to wait gets a coroutine. 
        /// The coroutine starts the first time the vertex is check, which should be when the 
        /// vertex is entered. Then when the coroutine finishes waiting, it will poke the 
        /// Task Manager. When the task is checked after the wait is completed, it will return True.
        /// </remarks>
        private class WaitTask : MonoBehaviour, ITaskable
        {
            public TaskableObject Taskable { get; private set; }

            private Dictionary<string, Coroutine> waitingRoutines;

            private void Awake()
            {
                Taskable = new TaskableObject(this);
                waitingRoutines = new Dictionary<string, Coroutine>();
            }

            public static void Initialize()
            {
                new GameObject("WaitTask", typeof(WaitTask));
            }

            public object CheckTask(int checkType, object inputData)
            {
                switch (checkType)
                {
                    case (int)HeavyEquipmentCheckTypes.Wait:

                        //get the data passed in about the wait that should happen
                        if (inputData is Tuple<string, float> data)
                        {
                            //data.item1 = A key that is used to track which wait we are checking.
                            //data.item2 = The Coroutine that is handling the wait. If null, the wait is complete.

                            //if this wait has already started
                            if (waitingRoutines.ContainsKey(data.Item1))
                            {
                                bool waitComplete = waitingRoutines[data.Item1] == null;

                                if (waitComplete)
                                {
                                    //if the wait is complete, remove it from the list
                                    waitingRoutines.Remove(data.Item1);
                                }

                                return waitComplete;
                            }
                            else
                            {
                                //start the wait for this key
                                waitingRoutines.Add(data.Item1, StartCoroutine(Wait(data.Item2, data.Item1)));
                                return false;
                            }
                        }
                        else
                        {
                            Debug.LogError("The inputData for a Wait task must be Tuple<string, float>");
                            return null;
                        }


                    default:
                        Debug.LogError("A check type was passed into WaitTask that it could not handle");
                        return null;
                }
            }

            private IEnumerator Wait(float time, string key)
            {
                yield return new WaitForSecondsRealtime(time);

                waitingRoutines[key] = null;
                Taskable.PokeTaskManager();
            }
        }
    }
}
