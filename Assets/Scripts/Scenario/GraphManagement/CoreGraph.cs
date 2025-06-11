using System.Collections.Generic;
using QuickGraph;
using System.Linq;

/// <summary>
/// This class will provide an layer of abstraction to the BidirectionalGraph 
/// in the QuickGraph library. This will include providing methods for 
/// functionality that will be repeatedly used. 
/// 
/// This class will contain NO BUSINESS LOGIC about the how the graph is being
/// used in the system. That functionality shall be provided by the controller/
/// TaskManager.
/// </summary>
public class CoreGraph : BidirectionalGraph<GraphVertex, GraphEdge> 
{
    public CoreGraph()
    {

    }

    #region Getters

    /// <summary>
    /// Gets the out edges on a vertex based on the edge type passed in.
    /// </summary>
    /// <param name="vertex">The vertex to get the edges on</param>
    /// <param name="type">The type of edges to get</param>
    /// <returns>A list of the out edges</returns>
    public List<GraphEdge> GetOutEdges(GraphVertex vertex, GraphEdge.EdgeTypes type)
    {
        return GetOutEdges(vertex).Where(x => x.EdgeType == type).ToList();
    }

    /// <summary>
    /// Get all the out edges on the vertex.
    /// </summary>
    /// <param name="vertex">The vertex to get the edges on</param>
    /// <returns>A list of all the out edges</returns>
    public List<GraphEdge> GetOutEdges(GraphVertex vertex)
    {
        if(TryGetOutEdges(vertex, out List<GraphEdge> output))
        {
            return output;
        }
        else
        {
            return new List<GraphEdge>();
        }
    }

    public List<T> GetOutEdges<T>(GraphVertex vertex)
    {
        if (TryGetOutEdges(vertex, out List<GraphEdge> allEdges))
        {
            List<T> output = new List<T>();

            foreach(GraphEdge edge in allEdges)
            {
                if(edge is T)
                {
                    output.Add((T)(object)edge);
                }
            }

            return output;
        }
        else
        {
            return new List<T>();
        }
    }

    /// <summary>
    /// An overload on TryGetOutEdges from QuickGraph. This version uses a List
    /// instead of an IEnumerable. This is just for easier usability.
    /// </summary>
    /// <param name="v">The vertex to get the edges from</param>
    /// <param name="edges">The list to be filled</param>
    /// <returns>If any edges were found</returns>
    public bool TryGetOutEdges(GraphVertex v, out List<GraphEdge> edges)
    { 
        if (TryGetOutEdges(v, out IEnumerable<GraphEdge> output))
        {
            edges = output.ToList();
            return true;
        }

        edges = null;
        return false;
    }

    #endregion

    /// <summary>
    /// If an out edge has a path to the end vertex.
    /// </summary>
    /// <param name="startingEdge">The edge to start on</param>
    /// <param name="endVertex">The vertex to end on</param>
    /// <returns>If a path exists</returns>
    public bool EdgeHasPathToVertex(GraphEdge startingEdge, GraphVertex endVertex)
    {
        if(startingEdge.Target == endVertex)
        {
            return true;
        }

        return VertexsAreConnected(startingEdge.Target, endVertex);
    }

    /// <summary>
    /// Checks if a path can be made using traversable edges from a start vertex to an ending vertex.
    /// A breadth first search algorithm is used
    /// </summary>
    /// <param name="start">The vertex to start at</param>
    /// <param name="end">The vertex to search for</param>
    /// <returns>If a path can be made</returns>
    public bool VertexsAreConnected(GraphVertex start, GraphVertex end)
    {
        List<GraphVertex> visited = new List<GraphVertex>(); 
        Queue<GraphVertex> nextToVist = new Queue<GraphVertex>();
        nextToVist.Enqueue(start);

        while(nextToVist.Count > 0)
        {
            GraphVertex vertex = nextToVist.Dequeue();

            if(vertex == end)
            {
                return true;
            }

            if(visited.Contains(vertex))
            {
                continue;
            }

            visited.Add(vertex);
            
            foreach(GraphEdge edge in GetOutEdges(vertex, GraphEdge.EdgeTypes.Traversable))
            {
                nextToVist.Enqueue(edge.Target);
            }
        }

        return false;
    }

    /// <summary>
    /// Get a path of vertices between 2 vertices.
    /// Only traversal edges are checked.
    /// This method is very basic. It will only find one path and theres no guarantee that 
    /// it will be the shortest path. There for this method should really only be used for
    /// a graph that only has one path through it. 
    /// </summary>
    /// <param name="start">The vertex to start at</param>
    /// <param name="end">The vertex to end at</param>
    /// <returns>A list of the path between the start and end vertices (inclusive). Null if no path is found</returns>
    public List<GraphVertex> GetPathBetweenNodes(GraphVertex start, GraphVertex end)
    {
        List<GraphVertex> firstList = new List<GraphVertex>() { start };
        List<List<GraphVertex>> allLists = new List<List<GraphVertex>>() { firstList };

        while(allLists.Count > 0)
        {
            for (int i = allLists.Count - 1; i >= 0; i--)
            {
                List<GraphVertex> currentList = allLists[i];

                List<GraphEdge> outedges = GetOutEdges(currentList[currentList.Count - 1], GraphEdge.EdgeTypes.Traversable);

                if (outedges.Count == 0)
                {
                    allLists.RemoveAt(i);
                    continue;
                }

                //if there is more than one edge out, create a new list for each of them
                for (int j = 1; j < outedges.Count; j++)
                {
                    //check that its not self referential and that its not already in the list
                    if (outedges[j].Target == outedges[j].Source || currentList.Contains(outedges[j].Target))
                    {
                        continue;
                    }

                    //copy the start of the list
                    List<GraphVertex> newList = new List<GraphVertex>(currentList);
                    newList.Add(outedges[j].Target);

                    if (outedges[j].Target == end)
                    {
                        return newList;
                    }

                    //add the new list to all the lists
                    allLists.Add(newList);
                }

                //check the first out edge using the list that is already there.
                if (outedges[0].Target != outedges[0].Source && !currentList.Contains(outedges[0].Target))
                {
                    //add the first edge to the list that already exists
                    currentList.Add(outedges[0].Target);

                    if (outedges[0].Target == end)
                    {
                        return currentList;
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// This method sits on top of <see cref="BidirectionalGraph{TVertex, TEdge}.AddVertexRange(IEnumerable{TEdge})"/>.
    /// This method just uses the "params" feature so that the parameters can be passed in.
    /// </summary>
    /// <param name="graphVertices">All the vertices to add. They can be passed in as a comma separated list.</param>
    /// <returns>The return of <see cref="BidirectionalGraph{TVertex, TEdge}.AddVertexRange(IEnumerable{TEdge})"/></returns>
    public int AddVertices(params GraphVertex[] graphVertices)
    {
        return AddVertexRange(graphVertices);
    }

    /// <summary>
    /// This method sits on top of <see cref="BidirectionalGraph{TVertex, TEdge}.AddEdgeRange(IEnumerable{TEdge})"/>.
    /// This method just uses the "params" feature so that the parameters can be passed in.
    /// </summary>
    /// <param name="graphEdges">All the edges to add. They can be passed in as a comma separated list.</param>
    /// <returns></returns>
    public int AddEdges(params GraphEdge[] graphEdges)
    {
        return AddEdgeRange(graphEdges);
    }
}
