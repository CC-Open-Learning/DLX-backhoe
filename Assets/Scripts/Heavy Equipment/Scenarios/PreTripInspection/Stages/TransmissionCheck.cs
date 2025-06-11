using Lean.Gui;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using CheckTypes = TaskVertexManager.CheckTypes;

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
        public (GraphVertex start, GraphVertex end) BuildTransmissionCheckScenario(CoreGraph graph)
        {
            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();

            //This stuff is just temporary for testing the dip sticks

            #region tempWork
            GraphVertex BeginCheckTransmission = new GraphVertex();
            (GraphVertex start, GraphVertex end) checkTransmissionVerts = CheckTransmission(graph, false);

            UserTaskVertex stage22Start = CreateWaitTask("stage22Start", timeBetweenVerts);
            stage22Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage22Start");
            stage22Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage22Start");

            checkTransmissionVerts.start.OnEnterVertex += (_) =>
            {
                backhoe.TransmissionDrip.SetEnabled(false);
                backhoe.SecondTransmissionDrip.SetEnabled(true);

                FrontHood hood = FindObjectOfType<FrontHood>();
                if (!hood.IsOpen) hood.ToggleHoodOpen(0);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
                ToggleInteractability(true, backhoe.SecondTransmissionDrip);
            };

            end.OnLeaveVertex += (_) =>
            {
                FrontHood hood = FindObjectOfType<FrontHood>();
                if (hood.IsOpen) hood.ToggleHoodOpen(0);
                ToggleInteractability(false, backhoe.SecondTransmissionDrip);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
                StageCompleted(Stage.EngineBay2);
            };

            GraphEdge start_to_transmission = new GraphEdge(start, BeginCheckTransmission, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge BeginCheckTransmission_to_CheckTransmissionVertsStart = new GraphEdge(BeginCheckTransmission, checkTransmissionVerts.start, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge CheckTransmissionVertsEnd_to_end = new GraphEdge(checkTransmissionVerts.end, end, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));




            #endregion

            GraphEdge endEdge = new GraphEdge(start, end, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));

            graph.AddVertices(
                start,
                BeginCheckTransmission,
                end
            );

            graph.AddEdges(
                //endEdge
                start_to_transmission,
                BeginCheckTransmission_to_CheckTransmissionVertsStart,
                CheckTransmissionVertsEnd_to_end
            );

            return (start, end);
        }
    }
}

