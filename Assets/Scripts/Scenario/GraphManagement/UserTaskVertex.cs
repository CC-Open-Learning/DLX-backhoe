using RemoteEducation.Scenarios;
using System;
using System.Collections.Generic;

public class UserTaskVertex : GraphVertex
{
    public enum States
    {
        NotStarted,
        Started,
        Complete,
        Failed,
        UnDone
    }

    public States State { get; private set; }

    public string Title;
    public string Description;
    public object ConstData;

    /// <summary>
    /// The class that this node will get data from.
    /// </summary>
    public Type ComponentType;

    /// <summary>
    /// The mode this vertex wants its data. This can be int, float, bool, or other kinds based on an extension module.
    /// </summary>
    public int CheckType;

    /// <summary>
    /// If this vertex shall only check components with an <see cref="TaskableObject.Identifier"/> that matches this.
    /// The component type on this vertex is still used.
    /// </summary>
    public string ComponentIdentifier;

    /// <summary>If the title and description are blank.</summary>
    public bool BlankTitleAndDescription => string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Description);

    public delegate void OnInspectionProgressEvent(UserTaskVertex vertex);
    /// <summary>
    /// This event is fired when exiting a graph vertex.
    /// </summary>
    public static event OnInspectionProgressEvent OnInspectionProgress;

    public UserTaskVertex(Type componentType, TaskVertexManager.CheckTypes checkType, object constData = null) : this(componentType, (int)checkType, constData)
    {    }

    public UserTaskVertex(Type componentType, int checkType, object constData = null) : this()
    {
        ComponentType = componentType;
        CheckType = checkType;
        ConstData = constData;
    }

    /// <summary>
    /// This constructor should be used when the task isn't dependent on checking an object in
    /// the scene. This will function more like a regular <see cref="GraphVertex"/>, but will still have the 
    /// title and description.
    /// </summary>
    public UserTaskVertex()
    {
        State = States.NotStarted;
        OnEnterVertex += (edge) => State = States.Started;
        OnLeaveVertex += (edge) => State = States.Complete;

        OnReliantCheckedForCompletion += UpdateTaskCompletionState;

        OnLeaveVertex += (_) =>
        {
            if (OnInspectionProgress != null)
                OnInspectionProgress(this);
        };
    }

    public override void UpdateData(object inData = null)
    {
        if(ComponentType == null)
        {
            base.UpdateData(inData);
        }
        else
        {
            if(TaskVertexManager.GetDataFromComponents(ComponentType, CheckType, out object valueFound, ConstData, ComponentIdentifier))
            {
                base.UpdateData(valueFound);
            }
        }
    }

    /// <summary>
    /// Check if the completion state of this vertex changed.
    /// </summary>
    /// <param name="completed">The current state of the vertex after it was checked by a reliant edge.</param>
    /// <returns>If the completion state changed.</returns>
    public bool UpdateTaskCompletionState(bool completed)
    {
        if(completed && State != States.Complete)
        {
            State = States.Complete;

            return true;
        }
        else if(!completed && State == States.Complete)
        {
            State = States.UnDone;

            return true;
        }

        return false;
    }

    public override string ToString()
    {
        if(Title != null && Title.Length > 0)
        {
            return Title;
        }
        
        if(ComponentType != null)
        {
            return "Check: " + ComponentType.Name;
        }

        return base.ToString();
    }
}
