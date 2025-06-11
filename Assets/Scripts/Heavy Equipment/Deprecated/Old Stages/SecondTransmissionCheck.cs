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
        /// Build the graph for the Second Transmission check part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildSecondTransmissionCheckScenario(CoreGraph graph)
        {
   
            GraphVertex BeginCheckTransmission = new GraphVertex();
            (GraphVertex start, GraphVertex end) checkTransmissionVerts = CheckTransmission(graph, false);

            UserTaskVertex stage22Start = CreateWaitTask("stage22Start", timeBetweenVerts);
            stage22Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage22Start");
            stage22Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage22Start");

            UserTaskVertex stage23Start = CreateWaitTask("stage23Start", timeBetweenVerts);
            stage23Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage23Start");
            stage23Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage23Start");


            checkTransmissionVerts.start.OnEnterVertex += (_) =>
            {
                backhoe.TransmissionDrip.SetEnabled(false);
                backhoe.SecondTransmissionDrip.SetEnabled(true);

                FrontHood hood = FindObjectOfType<FrontHood>();
                if (!hood.IsOpen) hood.ToggleHoodOpen(0);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
            };

            stage23Start.OnEnterVertex += (_) =>
            {
                FrontHood hood = FindObjectOfType<FrontHood>();
                if (hood.IsOpen) hood.ToggleHoodOpen(0);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
                ToggleInteractability(false, backhoe.SecondTransmissionDrip);
            };

            stage23Start.OnLeaveVertex += (_) => StageCompleted(Stage.EngineBay2);

            GraphEdge stage22Start_to_BeginCheckTransmission = new GraphEdge(stage22Start, BeginCheckTransmission, new GraphData(true));
            GraphEdge BeginCheckTransmission_to_CheckTransmissionVertsStart = new GraphEdge(BeginCheckTransmission, checkTransmissionVerts.start, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge CheckTransmissionVertsEnd_to_stage23Start = new GraphEdge(checkTransmissionVerts.end, stage23Start, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));



            graph.AddVertices(
                stage22Start,
                BeginCheckTransmission,
                stage23Start
            );

            graph.AddEdges(
                stage22Start_to_BeginCheckTransmission,
                BeginCheckTransmission_to_CheckTransmissionVertsStart,
                CheckTransmissionVertsEnd_to_stage23Start
            );


            return (stage22Start, stage23Start);
        }
    }
}

