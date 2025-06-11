using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphVertex
{
    private static int globalVertexCount = 0;
    public static void ResetVertexCount() { globalVertexCount = 0; }
    public int Id { get; private set; }

    public static int CurrentVertex { get; private set; }

    public GraphData Data;

    //delegates
    public delegate void VertexEventDelegate(GraphEdge graphEdge);
    public delegate bool CheckedForCompletionDelegate(bool wasComplete);

    /// <summary>
    /// Called when the vertex becomes the current vertex in a graph controller.
    /// The edge that was taken to get to this vertex is passed as the parameter.
    /// </summary>
    public VertexEventDelegate OnEnterVertex;

    /// <summary>
    /// Called when the vertex stops being the current vertex in a graph controller.
    /// The edge that was taken when leaving the vertex is passed as the parameter.
    /// </summary>
    public VertexEventDelegate OnLeaveVertex;

    /// <summary>
    /// This event will ONLY be called if the vertex is being checked from a reliant edge.
    /// If the check was complete is passed in as the parameter.
    /// The return from this event will be if the completion state changed. (ie completed to undone, or undone to completed)
    /// </summary>
    public CheckedForCompletionDelegate OnReliantCheckedForCompletion;

    public GraphVertex(GraphData data)
    {
        Data = data;
        HandleID();
    }

    public GraphVertex()
    {
        Data = new GraphData();
        HandleID();
    }

    private void HandleID()
    {
        Id = globalVertexCount;
        globalVertexCount++;
        OnEnterVertex += SetCurrentVertexToThisVertex;
    }

    private void SetCurrentVertexToThisVertex(GraphEdge graphEdge)
    {
        CurrentVertex = Id;
    }

    public virtual void UpdateData(object inData = null)
    {
        Data.Value = inData;
    }
}
