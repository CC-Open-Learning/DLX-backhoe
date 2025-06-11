using RemoteEducation.Scenarios;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Edge = GraphEdge;

/// <summary>
/// This class defines how the graph is can be moved through. 
/// It adds a layer of abstraction on top of the actual graph object so
/// that it can more easily be used in Core Engine. 
/// This class shall never contain any business logic for a specific 
/// scenario.
/// 
/// The main functions this class executes is tracking the current vertex and
/// defining how it can move. This is (at a high level) done by:
/// 1. Doing all the Data Check edges.
/// 2. Checking if all the reliant edges are complete.
/// 3. Checking if the data in the current vertex matches to any of the data in
/// any of its out edges.
/// 
/// 
/// There are 2 general use cases for this class: Scenario Graphs and Hard-Coded Graphs
/// Scenario Graphs:
/// These are graphs that define a scenario. They are defined by a class
/// that implements the <see cref="ITaskableGraphBuilder"/> interface.
/// They should use the <see cref="GraphController(ITaskableGraphBuilder)"/> constructor.
/// There shall only be one of these graph active at a time. They are held and controlled 
/// by the <see cref="TaskManager"/>. These graphs should be loaded into CoreEngine through 
/// StartScene (or whatever the current startup process is).
/// 
/// Hard-Coded Graphs:
/// These are graphs that are used to define functionality that is not related to 
/// user tasks. This can be UI controls, camera movements, or anything else. 
/// There can be any number of these graphs in a scene.
/// These graphs "Hard-Coded" because they will be defined by the classes in the scene.
/// </summary>
public class GraphController
{
    //The actual graph object
    public CoreGraph Graph;

    /// <summary> The vertex the graph is currently on </summary>
    public GraphVertex CurrentVertex { get; private set; }

    /// <summary> The first vertex in the graph </summary>
    public GraphVertex StartVertex { get; private set; }

    /// <summary> The last vertex in the graph </summary>
    public GraphVertex EndVertex { get; private set; }

    /// <summary> This event is fired once the <see cref="EndVertex"/> becomes the <see cref="CurrentVertex"/> </summary>
    public Action OnEndVertexHit;

    /// <summary>
    /// All the completed reliant edges on for the <see cref="CurrentVertex"/>. This list is cleared out whenever the current vertex is updated
    /// </summary>
    private List<ReliantEdge> CompletedReliantEdges;

    public delegate void VertexChangeDelegate(GraphVertex newCurrentVertex);
    
    /// <summary>
    /// Called when a new vertex becomes the current vertex.
    /// </summary>
    public VertexChangeDelegate OnCurrentVertexUpdated;

    /// <summary>
    /// Called when a vertex is checked by a reliant edge and its state changes.
    /// So far only <see cref="UserTaskVertex"/>s use this method.
    /// </summary>
    /// <remarks>
    /// This event is NOT called when the vertex is completed the first time.
    /// Use <see cref="OnCurrentVertexUpdated"/> to get that event.
    /// </remarks>
    public VertexChangeDelegate OnVertexCompletionStateChanged;

    /// <summary> Whether to end the current progress </summary>
    private bool endProgress = false;

    public int SavePointVertexID = 0;

    #region Setup

    /// <summary>
    /// Create a graph controller for a scenario graph. There should only be
    /// one of these graphs active at a time.
    /// This constructor should never be used for hard-coded graphs
    /// </summary>
    /// <param name="graphBuilder">A graph builder for the scenario you wanna run</param>
    public GraphController(ITaskableGraphBuilder graphBuilder, IExtensionModule extensionModule)
    {
        Graph = graphBuilder.BuildGraph(out GraphVertex start, out GraphVertex end, extensionModule);
        StartVertex = start;
        EndVertex = end;

        CompletedReliantEdges = new List<ReliantEdge>();

        //set up that this controller can interface with UI buttons
        ButtonGraphInterface.AddGraphController(this);
    }

    /// <summary>
    /// Update an exist graph controller and its graph.
    /// </summary>
    /// <param name="graphBuilder">A graph builder for the scenario you wanna run</param>
    /// <param name="extensionModule">The module used to get the graph scenario</param>
    public void UpdateGraphController(ITaskableGraphBuilder graphBuilder, IExtensionModule extensionModule)
    {
        Graph = graphBuilder.BuildGraph(out GraphVertex start, out GraphVertex end, extensionModule);
        StartVertex = start;
        EndVertex = end;

        CompletedReliantEdges = new List<ReliantEdge>();
    }

    /// <summary>
    /// Create a graph controller for a "hard-coded" graph. 
    /// This is a graph that runs back-end functionality. It should not 
    /// contain any UserTaskVertices.
    /// </summary>
    /// <param name="graph">The graph to run</param>
    /// <param name="start">The starting vertex</param>
    /// <param name="end">The ending vertex</param>
    public GraphController(CoreGraph graph, GraphVertex start, GraphVertex end, bool startRightAway = false)
    {
        Graph = graph;
        StartVertex = start;
        EndVertex = end;

        CompletedReliantEdges = new List<ReliantEdge>();

        //set up that this controller can interface with UI buttons
        ButtonGraphInterface.AddGraphController(this);

        if(startRightAway)
        {
            StartGraph();
        }
    }

    public void StartGraph()
    {
        StartVertex.UpdateData();
        StartVertex.OnEnterVertex?.Invoke(null);
        CurrentVertex = StartVertex;

        SkipGraphToSavePoint(SavePointVertexID);

        OnCurrentVertexUpdated?.Invoke(CurrentVertex);

        ProgressIfPossible();
    }

    #endregion

    #region Vertex Progression

    /// <summary>
    /// This method will be called by objects throughout the scene when they do something that
    /// they think could finish the current node.
    /// </summary>
    /// <param name="data">Data to be passed to the current node</param>
    public void PokeCurrentVertex(object data)
    {
        CurrentVertex.UpdateData(data);
        ProgressIfPossible();
    }

    /// <summary>
    /// Update the current vertex based on an edge out of the current vertex.
    /// All of the events for leaving and entering a vertex are called here.
    /// Also the list of completed reliant edges is cleared out here.
    /// </summary>
    /// <param name="edge">The out edge on the current vertex to take</param>
    private void MoveToVertex(Edge edge)
    {
        //Debug.Log("Moving to the next vertex");

        CompletedReliantEdges.Clear();

        //invoke any actions for the 2 vertices
        CurrentVertex.OnLeaveVertex?.Invoke(edge);

        CurrentVertex = edge.Target;

        CurrentVertex.OnEnterVertex?.Invoke(edge);

        //activate the new current node
        CurrentVertex.UpdateData(edge.PassVertexData ? CurrentVertex.Data : null);

        OnCurrentVertexUpdated?.Invoke(CurrentVertex);

        if (edge.Target == EndVertex)
        {
            //fire the end vertex hit event and don't try to progress
            OnEndVertexHit?.Invoke();
            return;
        }

        //check if the current node is completed.
        ProgressIfPossible();
    }

    /// <summary>
    /// This method will trigger the engProgress variable to determine whether to end the progress
    /// </summary>
    public void EndCurrentProgress(bool isEnable)
    {
        endProgress = isEnable;
    }

    /// <summary>
    /// This method will progress to the next node if that is possible.
    /// 
    /// It will first do all the data checks on the current node.
    /// It will then start the checking of the reliant edges.
    /// If all the reliant edges are complete, a traversal edge will be taken 
    /// if possible.
    /// </summary>
    private void ProgressIfPossible()
    {
        if(CurrentVertex == EndVertex || endProgress)
        {
            return;
        }

        //first do all the data check edges
        DoAllDataCheckEdges(CurrentVertex, Graph.GetOutEdges(CurrentVertex, Edge.EdgeTypes.DataCheck));

        //do all the reliant complete edges
        if(CheckAllReliantEdges(CurrentVertex))
        {
            //check if there is an edge that can be taken
            Edge traverableEdge = GetPossibleTraversableEdge(CurrentVertex);

            if(traverableEdge != null)
            {
                MoveToVertex(traverableEdge);
            }
        }
    }

    /// <summary>
    /// Skip to the next vertex regardless of the states of the
    /// current vertex and the out edges.
    /// This method only works if there is only one out traversal edge.
    /// </summary>
    public void BeACheater()
    {
        List<Edge> outEdges = Graph.GetOutEdges(CurrentVertex, Edge.EdgeTypes.Traversable);
        
        if(outEdges.Count == 1)
        {
            MoveToVertex(outEdges[0]);
        }
        else
        {
            Debug.LogWarning("There are multiple out edges. You can't cheat on this one.");
        }
    }

    /// <summary>
    /// Skip to one passed the specified vertex regardless of the states of the
    /// current vertex and the out edges. Create a shortcut from start to target vertex.
    /// This method only works if there is only one out traversal edge.
    /// Mainly use for loading save point.
    /// </summary>
    /// <param name="targetVertexID">The vertex Id you want to skip to. It skips one pass the target vertex.</param>
    private void SkipGraphToSavePoint(int targetVertexID = 0)
    {
        if (targetVertexID <= 0) return;

        GraphVertex currentVertex = StartVertex;
        TaskBoardController taskBoardController = TaskBoardController.Instance;
        List<Edge> outEdges = Graph.GetOutEdges(currentVertex, Edge.EdgeTypes.Traversable);

        //Traverse graph until it reaches target vertex 
        while (currentVertex.Id != targetVertexID && outEdges.Count == 1)
        {
            currentVertex = outEdges[0].Target;
            outEdges = Graph.GetOutEdges(currentVertex, Edge.EdgeTypes.Traversable);
            taskBoardController.RefreshTaskBoardLoad(currentVertex);
        }
        
        GraphVertex targetVertex = null;

        if (outEdges.Count == 1)
        {
            targetVertex = outEdges[0].Target;
            taskBoardController.RefreshTaskBoardLoad(targetVertex);

            //Remove any edge from the start vertex
            Graph.RemoveEdge(GetPossibleTraversableEdge(StartVertex));

            //Add a new edge from start vertex to the save point vertex
            Graph.AddEdge(new GraphEdge(StartVertex, targetVertex, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass)));
        }
        else
        {
            Debug.LogWarning("There are multiple out edges. You can't cheat on this one.");
        }
    }

    #endregion

    #region Edge Checking

    /// <summary>
    /// This method will do all the data check edges on a vertex. 
    /// </summary>
    /// <param name="vertex">The vertex to do the edge on</param>
    /// <param name="dataCheckEdges">All the edges to take. If this is null, the edges will be calculated in the method</param>
    public void DoAllDataCheckEdges(GraphVertex vertex, List<Edge> dataCheckEdges = null)
    {
        if(dataCheckEdges == null)
        {
            dataCheckEdges = Graph.GetOutEdges(vertex, Edge.EdgeTypes.DataCheck);
        }

        foreach(Edge edge in dataCheckEdges)
        {
            DoDataCheckEdge(edge);
        }
    }

    /// <summary>
    /// This method will complete a DataCheck Edge. If the target vertex also has 
    /// DataCheck edges, those edges will also be checked.
    /// </summary>
    /// <param name="edge">The DataCheck edge to take</param>
    public void DoDataCheckEdge(Edge edge)
    {
        if(edge.EdgeType != Edge.EdgeTypes.DataCheck)
        {
            Debug.LogError("The wrong edge type was passed into DoDataCheckEdge");
            return;
        }

        //have the data check vertex interpret the data from the target
        edge.Target.UpdateData(edge.PassVertexData ? edge.Source.Data : null);

        //check for any data checks on the data check vertex
        DoAllDataCheckEdges(edge.Target);

        //pass the data back to the source
        edge.Source.UpdateData(edge.Target.Data);
    }

    /// <summary>
    /// Check if all of the Reliant edges on a vertex are complete.
    /// </summary>
    /// <param name="vertex">The vertex to check</param>
    /// <param name="stepsLeft">How many more steps down reliant edges can be taken</param>
    /// <returns></returns>
    private bool CheckAllReliantEdges(GraphVertex vertex, int stepsLeft = 0)
    {
        //get the out edge
        List<ReliantEdge> reliantEdges = Graph.GetOutEdges<ReliantEdge>(vertex);

        if(reliantEdges.Count == 0)
        {
            //if there are no reliant edges, we treat it as if they are all complete.
            return true;
        }

        //if this isn't the current vertex, the steps left will already be set
        if (vertex != CurrentVertex)
        {
            //decrement it and check how many steps left there are.
            stepsLeft = stepsLeft == ReliantEdge.NoStepCountLimit ? stepsLeft : stepsLeft - 1;

            if (stepsLeft < 0)
            {
                return true;
            }
        }

        //sort the sub edges out of the other edges
        List<ReliantEdge> subEdges = new List<ReliantEdge>();
        for (int i = reliantEdges.Count - 1; i >= 0; i--)
        {
            if (reliantEdges[i].IsSubEdge)
            {
                subEdges.Add(reliantEdges[i]);
                reliantEdges.RemoveAt(i);
            }
        }

        bool subEdgesComplete = true;
        bool regularEdgesComplete = true;

        //walk through each list
        for (int i = 0; i < 2; i++)
        {
            //first check regular, then sub edges
            List<ReliantEdge> edgesToCheck = i == 0 ? reliantEdges : subEdges;

            foreach (ReliantEdge edge in edgesToCheck)
            {
                //if this vertex is the current vertex, start all the checks with the edges steps left count.
                if (vertex == CurrentVertex)
                {
                    stepsLeft = edge.StepCount;
                }

                if (!CheckReliantEdge(edge, stepsLeft))
                {
                    //if were checking the non-leafs
                    if (i == 0)
                    {
                        regularEdgesComplete = false;
                    }
                    //if were checking the leafs
                    else
                    {
                        subEdgesComplete = false;
                    }
                }
            }

            //if any of the regular edges were not complete, don't check the sub edges.
            if(!regularEdgesComplete)
            {
                return false;
            }
        }

        return subEdgesComplete;
    }

    /// <summary>
    /// Check if a ReliantEdge leads to a complete vertex.
    /// If this edge has "OnlyCompleteOnce" set to true and it has already been completed,
    /// true will be returned but it will not be checked again.
    /// The stepsLeft variable will be passed to the call to CheckAllReliantEdges() in this method.
    /// </summary>
    /// <param name="edge">The Reliant edge to check</param>
    /// <param name="stepsLeft">How many more reliant steps down this path can be taken</param>
    /// <returns>If the target vertex is complete</returns>
    public bool CheckReliantEdge(ReliantEdge edge, int stepsLeft)
    {
        if(edge.EdgeType != Edge.EdgeTypes.Reliant)
        {
            Debug.LogError("The wrong edge type was passed into the CheckReliantEdgeCompletness");
            return false;
        }

        //check if the edge has already been marked complete.
        if(edge.OnlyCompleteOnce && CompletedReliantEdges.Contains(edge))
        {
            return true;
        }

        //update the targets data
        edge.Target.UpdateData(edge.PassVertexData ? edge.Source.Data.Value : null);

        //do all data checks on the target vertex
        DoAllDataCheckEdges(edge.Target);

        bool targetWasComplete = false;

        //check that the target vertices reliant edges are complete.
        if(CheckAllReliantEdges(edge.Target, stepsLeft))
        {
            //check if the vertex can take any of its traversable edges
            Edge traversableEdge = GetPossibleTraversableEdge(edge.Target);

            if(traversableEdge != null)
            {
                //check that the edge is a sub edge, or has a path back to the current vertex
                if(edge.IsSubEdge|| IsLeaf(edge.Target) || Graph.EdgeHasPathToVertex(traversableEdge, CurrentVertex))
                {
                    if(edge.OnlyCompleteOnce)
                    {
                        //mark this edge as complete
                        CompletedReliantEdges.Add(edge);
                    }

                    //mark that the target was complete
                    targetWasComplete = true;
                }
            }
        }

        //tell the vertex that it was checked by a reliant edge. Check if it returns that its completion state changed.
        bool? vertexStateChanged = edge.Target.OnReliantCheckedForCompletion?.Invoke(targetWasComplete);

        if (vertexStateChanged != null && (bool)vertexStateChanged)
        {
            //fire the event on the graph controller to say that this vertex had its state changed. 
            OnVertexCompletionStateChanged?.Invoke(edge.Target);
        }

        return targetWasComplete;
    }

    /// <summary>
    /// Check if any of the out traversable edges can be taken based on the data in the 
    /// vertex and edge
    /// </summary>
    /// <param name="vertex">The vertex to check</param>
    /// <returns>The out edge, or null if none are compatible</returns>
    public Edge GetPossibleTraversableEdge(GraphVertex vertex)
    {
        List<Edge> possibleOutEdges = Graph.GetOutEdges(vertex, Edge.EdgeTypes.Traversable);

        foreach(Edge edge in possibleOutEdges) 
        {
            //compare if the data in the vertex is compatible with the data in the edge
            if(GraphData.Compare(vertex, edge))
            {
                return edge;
            }
        }

        return null;
    }

    /// <summary>
    /// If the vertex is a leaf. A leaf is defined by having only one out
    /// traversal edge that leads back into the vertex.
    /// </summary>
    /// <param name="vertex">The vertex to check</param>
    /// <returns>If the vertex is a leaf</returns>
    public bool IsLeaf(GraphVertex vertex)
    {
        List<Edge> outEdges = Graph.GetOutEdges(vertex, Edge.EdgeTypes.Traversable);

        if (outEdges.Count == 1 && outEdges[0].Source == outEdges[0].Target)
        {
            return true;
        }

        return false;
    }


    #endregion

    #region Getters

    /// <summary>
    /// Return a list of leaf vertices connected to the passed in vertex by 
    /// Reliant Edges. <see cref="IsLeaf"/> is used to determine which vertices are leafs.
    /// </summary>
    /// <param name="vertex">The vertex to check</param>
    /// <returns>A list of the connected leafs</returns>
    public List<GraphVertex> GetLeafs(GraphVertex vertex)
    {
        List<GraphVertex> outputLeafs = new List<GraphVertex>();

        List<ReliantEdge> reliantEdges = Graph.GetOutEdges<ReliantEdge>(vertex);

        foreach (ReliantEdge reliantEdge in reliantEdges)
        {
            if (IsLeaf(reliantEdge.Target))
            {
                outputLeafs.Add(reliantEdge.Target);
            }
        }

        return outputLeafs;
    }

    public List<GraphVertex> GetSubVertices(GraphVertex start)
    {
        List<ReliantEdge> reliantEdges = Graph.GetOutEdges<ReliantEdge>(start).Where(x => x.IsSubEdge).ToList();

        List<GraphVertex> targets = new List<GraphVertex>();

        reliantEdges.ForEach((x) => { targets.Add(x.Target); });

        return targets;
    }

    public List<T> GetSubVertices<T>(GraphVertex start)
    {
        return GetSubVertices(start).OfType<T>().ToList();
    }

    /// <summary>
    /// Get a list of all the vertices that a vertex relies on.
    /// This is based on all the reliant edges that come out of the vertex
    /// passed in. The step out on the reliant edges coming out if the initial 
    /// vertex is used to determine how far the check through more reliant edges
    /// should go.
    /// </summary>
    /// <param name="startingVertex">The vertex to start on</param>
    /// <returns>All the relied on vertices</returns>
    public List<GraphVertex> GetReliedOnVertexies(GraphVertex startingVertex)
    {
        Queue<Tuple<GraphVertex, int>> next = new Queue<Tuple<GraphVertex, int>>();
        List<GraphVertex> visited = new List<GraphVertex>();

        //add the initial edges
        foreach (ReliantEdge reliantEdge in Graph.GetOutEdges<ReliantEdge>(startingVertex))
        {
            next.Enqueue(new Tuple<GraphVertex, int>(reliantEdge.Target, reliantEdge.StepCount));
        }

        //while there are still vertices to check
        while (next.Count > 0)
        {
            Tuple<GraphVertex, int> current = next.Dequeue();

            visited.Add(current.Item1);

            //if we can take another step
            if (current.Item2 > 0)
            {
                //add all the next vertices with one less step available
                foreach (ReliantEdge reliantEdge in Graph.GetOutEdges<ReliantEdge>(current.Item1))
                {
                    //don't re add any vertices that we already checked
                    if(!visited.Contains(reliantEdge.Target))
                    {
                        next.Enqueue(new Tuple<GraphVertex, int>(reliantEdge.Target, current.Item2 - 1));

                    }
                }
            }
        }

        return visited;
    }

    #endregion
}
