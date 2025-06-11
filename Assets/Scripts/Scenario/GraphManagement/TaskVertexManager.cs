using RemoteEducation.Scenarios;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TaskVertexManager 
{
    public enum CheckTypes 
    {
        Int = 0,
        Float = 1,
        Bool = 2,
        ElementsEvaluated = 3,
        ElementsInspected = 4
    }

    private static Dictionary<Type, List<ITaskable>> AllComponents;
    private static List<Type> SendersTurnedOn;

    public static Action PokeTaskManager;

    static TaskVertexManager()
    {
        AllComponents = new Dictionary<Type, List<ITaskable>>();
        SendersTurnedOn = new List<Type>();
    }

    /// <summary>
    /// This method should be called each time the <see cref="TaskManager"/> attaches
    /// to a new scenario. It will ensure that the correct <see cref="PokeTaskManager"/>
    /// actions is set up. It will also ensure that all the senders are turned off.
    /// </summary>
    /// <param name="action"></param>
    public static void Initialize(Action action)
    {
        PokeTaskManager = action;

        //if there were any senders turned on already, turn them off.
        TurnOffAllSenders();
    }

    public static void AddComponent(ITaskable component)
    {
        Type componentType = component.GetType();

        if(AllComponents.ContainsKey(componentType))
        {
            AllComponents[componentType].Add(component);
        }
        else
        {
            AllComponents.Add(componentType, new List<ITaskable>() { component });
        }

        if(SendersTurnedOn.Contains(componentType))
        {
            ToggleComponentSender(component, true); 
        }
    }

    public static void RemoveComponent(ITaskable component)
    {
        Type componentType = component.GetType();

        if(AllComponents.ContainsKey(componentType))
        {
            AllComponents[componentType].Remove(component);
            
            if(AllComponents[componentType].Count == 0)
            {
                AllComponents.Remove(componentType);
            }
        }
    }

    /// <summary>
    /// Get data from a taskable component in the scene.
    /// </summary>
    /// <param name="componentType">The type of component to check</param>
    /// <param name="checkType">The type of check to do</param>
    /// <param name="output">The returned data</param>
    /// <param name="inputData">Any data needed to make the check</param>
    /// <param name="identifier">An optional identifier to specify which Taskable component to check</param>
    /// <returns>If a component was available to be checked</returns>
    public static bool GetDataFromComponents(Type componentType, int checkType, out object output, object inputData, string identifier = null)
    {
        //check if any components have been destroyed
        CheckForNullComponents(componentType);

        if (AllComponents.ContainsKey(componentType))
        {

            //!!DISCLAIMER!! right now this is only set up to return data from one component. 
            // This will be okay for now since none of the task have more than one type of the component. 
            // This should eventually be expanded out though.
            //output = new List<object>();
            //foreach (ITaskable taskable in AllComponents[componentType])
            //{
            //    if(taskable != null)
            //    {
            //        output.Add(taskable.CheckTask(checkType, inputData));
            //    }
            //}

            ITaskable taskableToCheck = null;

            if(identifier != null)
            {
                taskableToCheck = AllComponents[componentType].Find(x => string.Compare(x.Taskable.Identifier, identifier) == 0);

                if(taskableToCheck == null)
                {
                    Debug.LogWarning("The identifier \"" + identifier + "\" was not found in the type \"" + componentType.Name + "\"");
                }
            }
            else
            {
                taskableToCheck = AllComponents[componentType][0];
            }

            if(taskableToCheck != null)
            {
                output = taskableToCheck.CheckTask(checkType, inputData);
                return true;
            }
        }

        //no components of that type are currently being tracked
        output = null;
        return false;
    }

    /// <summary>
    /// Update the dictionary of components to clear out any null values.
    /// This can happen if the gameobject that the component was on is destroyed.
    /// In theory this should be tied to the onDestroy of the game object. I haven't
    /// figured out how to do this without manually adding it to every components OnDestoy
    /// </summary>
    /// <param name="type">The type of component to check</param>
    private static void CheckForNullComponents(Type type)
    {
        if(AllComponents.ContainsKey(type))
        {
            //remove any null values
            for(int i = AllComponents[type].Count - 1; i >= 0; i--)
            {
                //If the ITaskable is on a MonoBehaviour, check if that MonoBehaviour is null.
                // Otherwise just do a regular null check.
                if ((AllComponents[type][i] is MonoBehaviour mono && mono == null) || AllComponents[type][i] is null)
                {
                    AllComponents[type].RemoveAt(i);
                }
            }

            //if there are no more values, remove the type from the dictionary
            if(AllComponents[type].Count == 0)
            {
                AllComponents.Remove(type);
            }
        }
    }

    private static void TurnOffAllSenders()
    {
        UpdateSendersByType(new List<Type>());
    }

    public static void UpdateSendersByType(List<Type> types)
    {
        ToggleSendersByType(SendersTurnedOn.Where(x => !types.Contains(x)).ToList(), false);
        ToggleSendersByType(types.Where(x => !SendersTurnedOn.Contains(x)).ToList(), true);
    }


    public static void ToggleSendersByType(List<Type> types, bool onOrOff)
    {
        foreach(Type t in types)
        {
            ToggleSendersByType(t, onOrOff);
        }
    }

    public static void ToggleSendersByType(Type type, bool onOrOff)
    {
        //if ON and its not in there, or its OFF and it is there
        if(onOrOff ? !SendersTurnedOn.Contains(type) : SendersTurnedOn.Contains(type))
        {
            //add or remove the type from the list of active senders
            if(onOrOff)
            {
                SendersTurnedOn.Add(type);
            }
            else
            {
                SendersTurnedOn.Remove(type);
            }

            CheckForNullComponents(type);

            //toggle each component
            if (AllComponents.ContainsKey(type))
            {
                foreach (ITaskable taskable in AllComponents[type])
                {
                    ToggleComponentSender(taskable, onOrOff);
                }
            }
        }
    }

    private static void ToggleComponentSender(ITaskable component, bool onOrOff)
    {
        if (component == null)
        {
            Debug.LogError("TaskVertexManager : Cannot set up a listener on a null ITaskable component");
        }
        else if (component.Taskable == null)
        {
            Debug.LogError("TaskVertexManager : Cannot set up a listener on an ITaskable component with a null TaskableObject member");
        }
        else
        {
            component.Taskable.SetupListener(onOrOff ? PokeTaskManager : null);
        }
    }

    /// <summary>
    ///     Clears the private static <see cref="SendersTurnedOn"/> collection
    /// </summary>
    public static void ClearSenders()
    {
        SendersTurnedOn.Clear();
    }
}
