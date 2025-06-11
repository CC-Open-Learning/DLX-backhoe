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
        public (GraphVertex start, GraphVertex end) BuildInteriorChecklistScenario(CoreGraph graph)
        {
            // Vertices

            UserTaskVertex stage3ReadingDiagonistics = CreateWaitTask("stage3ReadingDiagonistics", timeBetweenVerts);
            stage3ReadingDiagonistics.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3ReadingDiagonistics");
            stage3ReadingDiagonistics.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3ReadingDiagonistics");

            UserTaskVertex stage3Checklist = new UserTaskVertex(typeof(ClipboardController), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3Checklist"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3Checklist")
            };

            UserTaskVertex stage3ObserveFuelGauge = new UserTaskVertex(typeof(Gauge), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3ObserveFuelGauge"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3ObserveFuelGauge"),
                ComponentIdentifier = Gauge.Locations.GaugePanel.ToString()
            };

            UserTaskVertex stage5ExitCab = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage5ExitCab"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage5ExitCab"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            // Transitions


            stage3ReadingDiagonistics.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InteriorSidePanel));

            stage3Checklist.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.ChecklistPOI));
                backhoe.GaugePanelAnimator.OpenClipboard();
            };

            stage3Checklist.OnLeaveVertex += (_) =>
            {
                backhoe.GaugePanelAnimator.CloseClipboard();
            };

            stage3ObserveFuelGauge.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.FuelTank.GetFuelGauge());
            };

            stage3ObserveFuelGauge.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.FuelTank.GetFuelGauge());
            };

            stage5ExitCab.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));
                backhoe.CabDoors[0].CanClear = false;
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.Stage5;
                backhoe.CabDoors[0].SetDoorInteractable(true);
                // unhide rear window after inspecting the boom lock latch
                backhoe.Windows[0].gameObject.SetActive(true);
            };
            stage5ExitCab.OnLeaveVertex += (_) =>
            {
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.None;
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).GetComponent<DollyWithTargets>().SetUserControl(false);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).GetComponent<DollyWithTargets>().SetupTarget(0);
                StageCompleted(Stage.InsideCab1);
            };

            stage5ExitCab.OnLeaveVertex += (_) => backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);

            GraphVertex end = new GraphVertex();

            GraphEdge stage3ReadingDiagonistics_to_stage3Checklist = new GraphEdge(stage3ReadingDiagonistics, stage3Checklist, new GraphData(true));
            GraphEdge stage3Checklist_to_stage3ObserveFuelGauge = new GraphEdge(stage3Checklist, stage3ObserveFuelGauge, new GraphData(true));
            GraphEdge stage3ObserveFuelGauge_to_stage5ExitCab = new GraphEdge(stage3ObserveFuelGauge, stage5ExitCab, new GraphData(true));
            GraphEdge stage5ExitCab_to_stage5End = new GraphEdge(stage5ExitCab, end, new GraphData(1));

            graph.AddVertices(
                stage3ReadingDiagonistics,
                stage3Checklist,
                stage3ObserveFuelGauge,
                stage5ExitCab,
                end);

            graph.AddEdges(
                stage3ReadingDiagonistics_to_stage3Checklist,
                stage3Checklist_to_stage3ObserveFuelGauge,
                stage3ObserveFuelGauge_to_stage5ExitCab,
                stage5ExitCab_to_stage5End
                );

            return (stage3ReadingDiagonistics, end);
        }
    }
}

