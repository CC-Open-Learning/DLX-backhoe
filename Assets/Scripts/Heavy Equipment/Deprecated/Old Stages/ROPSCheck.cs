using Cinemachine;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using CheckTypes = TaskVertexManager.CheckTypes;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the ROPS check part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildROPSScenario(CoreGraph graph)
        {
            // Vertices
            GraphVertex stage11End = new GraphVertex();

            UserTaskVertex openCabDoor = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage21OpenDoor"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage21OpenDoor"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            UserTaskVertex turnOnLights = new UserTaskVertex(typeof(BackhoeLightController), TaskVertexManager.CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleBackHoeLights"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionBackHoeLights"),
            };

            UserTaskVertex inspectLights = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackHoeLights.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectLights"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectLights"),
            };

            UserTaskVertex inspectLeftDoorFrame = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, backhoe.CabDoors[0].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckLeftDoorFrame"),
                Description= Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckLeftDoorFrame"),
            };

            UserTaskVertex inspectRightDoorFrame = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, backhoe.CabDoors[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightDoorFrame"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightDoorFrame"),
            };

            UserTaskVertex inspectROPS = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.ROPS.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleROPS"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionROPS"),
            };

            // Transitions
            openCabDoor.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().MoveToTarget(2, 5f);
            turnOnLights.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InteriorSidePanel));
            
            inspectLights.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().SetupTarget(0);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).VirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0.05f;
                module.InspectableManager.UpdateActiveElements(sceneData.BackHoeLights.GroupElements);
                leanWindow.TurnOn();
                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
            };

            inspectLights.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                leanWindow.TurnOff();
            };
            
            inspectLeftDoorFrame.OnEnterVertex += (_) => {
                module.InspectableManager.AddActiveElement(backhoe.CabDoors[0].InspectableElement);
                backhoe.CabDoors[0].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);

                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().MoveToTarget(1, 2f);
            };

            inspectLeftDoorFrame.OnLeaveVertex += (_) => {
                module.InspectableManager.RemoveActiveElement(backhoe.CabDoors[0].InspectableElement);
                backhoe.CabDoors[0].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            };

            inspectRightDoorFrame.OnEnterVertex += (_) => {
                module.InspectableManager.AddActiveElement(backhoe.CabDoors[1].InspectableElement);
                backhoe.CabDoors[1].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);

                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().MoveToTarget(2, 2f);
            };

            inspectRightDoorFrame.OnLeaveVertex += (_) => {
                module.InspectableManager.RemoveActiveElement(backhoe.CabDoors[1].InspectableElement);
                backhoe.CabDoors[1].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            };

            inspectROPS.OnEnterVertex += (_) =>
            {
                sceneData.RightDoorAndWindows.ToggleGroupInteraction(true);
                ToggleInteractability(true, backhoe.ROPS.GetComponent<Interactable>());
                module.InspectableManager.UpdateActiveElements(sceneData.ROPS.GroupElements);
            };

            inspectROPS.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.ROPS.GetComponent<Interactable>());
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                StageCompleted(Stage.LightsAndROPS);
            };

            // Define Edges
            GraphEdge openCabDoor_to_turnOnLights = new GraphEdge(openCabDoor, turnOnLights, new GraphData(1));
            GraphEdge turnOnLights_to_inspectLights = new GraphEdge(turnOnLights, inspectLights, new GraphData(true));
            GraphEdge inspectLights_to_inspectLeftDoorFrame = new GraphEdge(inspectLights, inspectLeftDoorFrame, new GraphData(true));
            GraphEdge inspectLeftDoorFrame_to_inspectRightDoorFrame = new GraphEdge(inspectLeftDoorFrame, inspectRightDoorFrame, new GraphData(true));
            GraphEdge inspectRightDoorFrame_to_inspectROPS = new GraphEdge(inspectRightDoorFrame, inspectROPS, new GraphData(true));
            GraphEdge inspectROPS_to_stage11End = new GraphEdge(inspectROPS, stage11End, new GraphData(true));

            // Add Vertices and Edges
            graph.AddVertices(
                openCabDoor,
                turnOnLights,
                stage11End,
                inspectLights,
                inspectLeftDoorFrame,
                inspectRightDoorFrame,
                inspectROPS);

            graph.AddEdges(
                openCabDoor_to_turnOnLights,
                turnOnLights_to_inspectLights,
                inspectLights_to_inspectLeftDoorFrame,
                inspectLeftDoorFrame_to_inspectRightDoorFrame,
                inspectRightDoorFrame_to_inspectROPS,
                inspectROPS_to_stage11End);

            return (openCabDoor, stage11End);
        }
    }
}

