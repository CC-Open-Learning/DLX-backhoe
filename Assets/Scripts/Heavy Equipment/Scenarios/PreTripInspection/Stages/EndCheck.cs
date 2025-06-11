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
        public (GraphVertex start, GraphVertex end) BuildEndScenario(CoreGraph graph)
        {
            //Testing stuff

            UserTaskVertex stage21OpenDoor = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage21OpenDoor"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage21OpenDoor"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            UserTaskVertex turnOffEngine = CreateWaitTask("turnOffEngine", timeBetweenVerts);
            turnOffEngine.Description = "When we end the day, ensure that the backhoe is off.";//Localizer.Localize("HeavyEquipment.PreTripDescriptionStage10Start");

            UserTaskVertex endOfDayReminder01 = CreateWaitTask("endOfDayReminder01", timeBetweenVerts * 1.5f);
            endOfDayReminder01.Description = "We should always make sure that the fuel is topped up before we leave.";//Localizer.Localize("HeavyEquipment.PreTripDescriptionStage10Start");

            UserTaskVertex endOfDayReminder02 = CreateWaitTask("endOfDayReminder02", timeBetweenVerts * 1.5f);
            endOfDayReminder02.Description = "If the backhoe has DEF, make sure that is topped up as well.";//Localizer.Localize("HeavyEquipment.PreTripDescriptionStage10Start");

            // Transitions
            stage21OpenDoor.OnEnterVertex += (_) =>
            {
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.Stage21;
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            };
            stage21OpenDoor.OnLeaveVertex += (_) =>
            {
                backhoe.EngineSound.ChangeEngineSoundPosition(EngineSoundController.Position.Inside);

                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.None;
                backhoe.EngineSound.StopEngine();
            };

            endOfDayReminder02.OnLeaveVertex += (_) =>
            {
                DLXCompleted();
                GraphVertex.ResetVertexCount();
                OnDeleteSave();
            };

            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();

            //GraphEdge endEdge = new GraphEdge(start, end, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));

            GraphEdge start_to_stage21OpenDoor = new GraphEdge(start, stage21OpenDoor, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge openDoor_to_engineOff = new GraphEdge(stage21OpenDoor, turnOffEngine, new GraphData(1));
            GraphEdge engineOff_to_reminder1 = new GraphEdge(turnOffEngine, endOfDayReminder01, new GraphData(true));
            GraphEdge reminder1_to_reminder2 = new GraphEdge(endOfDayReminder01, endOfDayReminder02, new GraphData(true));
            GraphEdge reminder2_to_end = new GraphEdge(endOfDayReminder02, end, new GraphData(true));

            graph.AddVertices(
                start,
                stage21OpenDoor,
                turnOffEngine,
                endOfDayReminder01,
                endOfDayReminder02,
                end
            );
            (GraphVertex start, GraphVertex end) Stage21Verts = BuildCabScenario(graph);

            graph.AddEdges(
                //endEdge
                start_to_stage21OpenDoor,
                openDoor_to_engineOff,
                engineOff_to_reminder1,
                reminder1_to_reminder2,
                reminder2_to_end
            );

            return (start, end);
        }
    }
}

