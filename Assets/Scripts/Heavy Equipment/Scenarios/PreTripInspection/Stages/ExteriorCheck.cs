using Lean.Gui;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using CheckTypes = TaskVertexManager.CheckTypes;
using System.Collections.Generic;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>
    /// Build the graph for the Inside cab inspection part of the scenario.
    /// </summary>
    /// <param name="graph">The graph in the scene.</param>
    /// <returns>
    /// Start = A dummy vertex at the start of this section of the scenario. 
    ///   End = A dummy vertex at the end of this section of the scenario.
    /// </returns>
    partial class PreTripInspectionGS
    {
        public (GraphVertex start, GraphVertex end) BuildExteriorInspectionScenario(CoreGraph graph)
        {
            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();

            var Stages = new List<(GraphVertex start, GraphVertex end)>() {
                BuildLeftSideCheck(graph),
                BuildBackLeftCheck(graph),
                BuildBackRightCheck(graph),
                BuildBackOverallCheck(graph),
                BuildRightSideCheck(graph),
                BuildFrontRightCheck(graph),
                BuildFrontCheck(graph),
                BuildFrontLeftCornerCheck(graph),
                BuildOverallChecks(graph)
            };

            for (int i = 1; i < Stages.Count; i++)
            {
                GraphEdge e = new GraphEdge(Stages[i - 1].end, Stages[i].start, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
                graph.AddEdge(e);
            }

            GraphEdge startEdge = new GraphEdge(start, Stages[0].start, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));

            GraphEdge endEdge = new GraphEdge(Stages[Stages.Count - 1].end, end, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));

            graph.AddVertices(start, end);
            graph.AddEdges(startEdge, endEdge);

            return (start, end);
        }
    }
}

