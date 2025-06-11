using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Lift Arm check part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildLiftArmScenario(CoreGraph graph)
        {
            UserTaskVertex start = CreateWaitTask("BuildLiftArmScenarioStart", timeBetweenVerts);
            start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleBuildLiftArmScenarioStart");
            start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionBuildLiftArmScenarioStart");

            UserTaskVertex inspectLiftArm = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.LiftArm.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLiftArm"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLiftArm"),
            };

            UserTaskVertex inspectLeftPiston = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.LeftPiston.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLeftPiston"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLeftPiston"),
            };

            UserTaskVertex inspectLeftHydraulics = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.LeftHydraulics.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLeftHydraulics"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLeftHydraulics"),
            };

            UserTaskVertex inspectLeftPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectLeftPins"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectLeftPins")
            };

            UserTaskVertex inspectRightPiston = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.RightPiston.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleRightPiston"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionRightPiston"),
            };

            UserTaskVertex inspectRightHydraulics = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.RightHydraulics.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleRightHydraulics"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionRightHydraulics"),
            };

            UserTaskVertex inspectRightPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectRightPins"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectRightPins")
            };

            GraphVertex removeLiftArmDebris = new GraphVertex(); //Skipped

            UserTaskVertex liftArmInspectionResetCamera = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.FrontLiftArm.ToString()
            };

            UserTaskVertex end = CreateWaitTask("stage11Start", timeBetweenVerts);
            end.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage11Start");
            end.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage11Start");

            // Transitions
            inspectLiftArm.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmInspection));
                backhoe.InteractManager.SetLiftArm(false);
                module.InspectableManager.UpdateActiveElements(sceneData.LiftArm.GroupElements);
            };

            inspectLiftArm.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                backhoe.InteractManager.SetLiftArm(true);
            };

            liftArmInspectionResetCamera.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmInspection).GetComponent<DollyWithTargets>().MoveToTarget(0, 3f);

            inspectLeftPiston.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().SetupTarget(0);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly));
                module.InspectableManager.UpdateActiveElements(sceneData.LeftPiston.GroupElements);
                sceneData.LeftPiston.ToggleGroupInteraction(false);

            };

            inspectLeftPiston.OnLeaveVertex += (_) =>
            {
                sceneData.LeftPiston.ToggleGroupInteraction(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            inspectLeftHydraulics.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.LeftHydraulics.GroupElements);
                sceneData.LeftHydraulics.ToggleGroupInteraction(false);
            };

            inspectLeftHydraulics.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.LeftHydraulics.ToggleGroupInteraction(true);
            };

            inspectRightHydraulics.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.RightHydraulics.GroupElements);
                sceneData.RightHydraulics.ToggleGroupInteraction(false);
            };

            inspectLeftPins.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);
            inspectLeftPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            inspectRightHydraulics.OnLeaveVertex += (_) =>
            {
                sceneData.RightHydraulics.ToggleGroupInteraction(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            inspectRightPiston.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 2f);
                module.InspectableManager.UpdateActiveElements(sceneData.RightPiston.GroupElements);
                sceneData.RightPiston.ToggleGroupInteraction(false);
            };

            inspectRightPiston.OnLeaveVertex += (_) =>
            {
                sceneData.RightPiston.ToggleGroupInteraction(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            inspectRightPins.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);
            inspectRightPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            inspectRightPiston.OnLeaveVertex += (_) => StageCompleted(Stage.LiftArm);

            GraphEdge start_to_inspectLiftArm = new GraphEdge(start, inspectLiftArm, new GraphData(true));
            GraphEdge inspectLiftArm_to_liftArmInspectionResetCamera = new GraphEdge(inspectLiftArm, liftArmInspectionResetCamera, new GraphData(true));
            GraphEdge liftArmInspectionResetCamera_to_inspectLeftPiston = new GraphEdge(liftArmInspectionResetCamera, inspectLeftPiston, new GraphData(0));
            GraphEdge inspectLeftPiston_to_inspectLeftHydraulics = new GraphEdge(inspectLeftPiston, inspectLeftHydraulics, new GraphData(true));
            GraphEdge inspectLeftHydraulics_to_inspectLeftPins = new GraphEdge(inspectLeftHydraulics, inspectLeftPins, new GraphData(true));
            GraphEdge inspectLeftPins_to_inspectRightPiston = new GraphEdge(inspectLeftPins, inspectRightPiston, new GraphData(3));
            GraphEdge inspectRightPiston_to_inspectRightHydraulics = new GraphEdge(inspectRightPiston, inspectRightHydraulics, new GraphData(true));
            GraphEdge inspectRightHydraulics_to_inspectRightPins = new GraphEdge(inspectRightHydraulics, inspectRightPins, new GraphData(true));
            GraphEdge inspectRightPins_to_removeLiftArmDebris = new GraphEdge(inspectRightPins, removeLiftArmDebris, new GraphData(2));
            GraphEdge removeLiftArmDebris_to_end = new GraphEdge(removeLiftArmDebris, end, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));

            // Add Vertices and Edges
            graph.AddVertices(
                start,
                end,
                inspectLiftArm,
                liftArmInspectionResetCamera,
                inspectLeftPiston,
                inspectLeftHydraulics,
                inspectLeftPins,
                inspectRightPiston,
                inspectRightHydraulics,
                inspectRightPins,
                removeLiftArmDebris
                );

            graph.AddEdges(
                start_to_inspectLiftArm,
                inspectLiftArm_to_liftArmInspectionResetCamera,
                liftArmInspectionResetCamera_to_inspectLeftPiston,
                inspectLeftPiston_to_inspectLeftHydraulics,
                inspectLeftHydraulics_to_inspectLeftPins,
                inspectLeftPins_to_inspectRightPiston,
                inspectRightPiston_to_inspectRightHydraulics,
                inspectRightHydraulics_to_inspectRightPins,
                inspectRightPins_to_removeLiftArmDebris,
                removeLiftArmDebris_to_end);

            return (start, end);
        }
    }
}

