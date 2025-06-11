/*
*  FILE          :	ITaskable.cs
*  PROJECT       :	Core Engine
*  PROGRAMMER    :	Duane Cressman
*  FIRST VERSION :	2020-09-24
*  DESCRIPTION   :  This file contains the ITaskable interface and TaskableObject class. 
*  
*  interface Description: This interface will allow the task manager to interface with any class
*                         that uses it. This means that the object can be used as part of a task.
*                         The Taskable.ComponentType defines which types of tasks the object can 
*                         be a part of. Through out the functionality of class, Taskable.PokeTaskManager()
*                         should be called whenever it is possible that the task may be completed.
*                         
*              object CheckTask(Task task) : This method will be defined by each class that uses the interface.
*                                            The method should define the response to a question that is asked by
*                                            the task object that is passed in. They type of question is defined by
*                                            task.TaskCheckType. Some of the questions are pretty general, like 
*                                            TaskCheckType.GetBool. This means that each class gets to define how it
*                                            wants to answer the question. It will be up to use to make sure that
*                                            we don't do anything to outlandish with the responses. Each class
*                                            does NOT need to answer every question that TaskCheckType has. If
*                                            it doesn't make sense for the question that is being asked, null
*                                            shall be returned. This would be a bug though if this happens.
*                                            
*/

using System;

namespace RemoteEducation.Scenarios
{
    // This interface should be a implemented on any class that can be part of a task.
    public interface ITaskable
    {
        object CheckTask(int checkType, object inputData);

        TaskableObject Taskable { get; }
    }

    public class TaskableObject
    {
        //This is the event that the components will fire when they want to tell the task manager to check if the task is completed.
        public Action TellTaskManagerToUpdate;

        //the interface this class is on
        private ITaskable ParentInterface;

        public Action OnTaskableActivated;

        public string Identifier { get; private set; }
      
        public TaskableObject(ITaskable parentInterface, string identifier = null)
        {
            ParentInterface = parentInterface;
            Identifier = identifier;

            TaskVertexManager.AddComponent(parentInterface);
        }

        public void SetupListener(Action action)
        {
            TellTaskManagerToUpdate = action;

            if(OnTaskableActivated != null && action != null)
            {
                OnTaskableActivated.Invoke();
            }
        }

        public void PokeTaskManager()
        {
            TellTaskManagerToUpdate?.Invoke();
        }
    }
}