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
        /// This class is used to track when edges are taken in a graph.
        /// The case that is used in this graph is to track 3 paths through the graph that come into the
        /// same vertex. If all of the paths have been taken, vertex takes one out edge. If not all of the 
        /// paths have been taken, the vertex takes a different out edge that leads back to the start of the paths.
        /// What this achieves is that all 3 paths must be taken through the graph before you can continue onto 
        /// the rest of the graph.
        /// An instance of this class is "attached" to a vertex in the graph. This is done through a 
        /// Dictionary where value is type GraphVertex and value is EdgeTakenTracker. The 3 "paths" are
        /// the 3 in edges into the vertex this class is on.
        /// The vertex in its OnEnterVertex event should add the edge taken to the <see cref="EdgeTakenTracker.edgesTaken"/> 
        /// list. It should then use the ITaskable system to check if this class says that all the edges are taken.
        /// Make sure the <see cref="Identifier"/> on this class matches the <see cref="UserTaskVertex.ComponentIdentifier"/>
        /// for the vertex it is attached to.
        /// </summary>
        public class EdgeTakenTracker : ITaskable
        {
            public TaskableObject Taskable { get; private set; }

            /// <summary>The edges that are needed to be taken.</summary>
            private List<GraphEdge> neededEdges;

            /// <summary>The edges that have been taken.</summary>
            private List<GraphEdge> edgesTaken;

            /// <summary>Used to ensure that the Taskable system finds the right instance of this class.</summary>
            public string Identifier { get; private set; }

            private bool AllEdgesAreTaken
            {
                get
                {
                    foreach (GraphEdge graphEdge in neededEdges)
                    {
                        if (!edgesTaken.Contains(graphEdge))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            public EdgeTakenTracker(string identifier)
            {
                edgesTaken = new List<GraphEdge>();

                Taskable = new TaskableObject(this, identifier);

                Identifier = identifier;
            }

            public EdgeTakenTracker(string identifier, List<GraphEdge> neededEdges) : this(identifier)
            {
                this.neededEdges = neededEdges;
            }

            /// <summary>Add edges that need to be take for this object to complete.</summary>
            public void AddEdgesNeeded(params GraphEdge[] edges)
            {
                if (neededEdges == null)
                {
                    neededEdges = new List<GraphEdge>();
                }

                neededEdges.AddRange(edges);
            }

            /// <summary>
            /// Add an edge that has been taken. This should be called in the <see cref="GraphVertex.OnEnterVertex"/> event
            /// of the vertex this object is attached to. 
            /// </summary>
            public void AddEdgeTaken(GraphEdge graphEdge)
            {
                if (!edgesTaken.Contains(graphEdge))
                {
                    edgesTaken.Add(graphEdge);

                    Debug.Log("Adding a edge to the edge tracker");
                }
            }

            public object CheckTask(int checkType, object inputData)
            {
                switch (checkType)
                {
                    case (int)CheckTypes.Bool:
                        return AllEdgesAreTaken;

                    default:
                        Debug.LogError("A check type was passed to EdgeTakenTracker that it couldn't handle");
                        return null;
                }
            }
        }
    }
}
