using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Scenarios;
using Lean.Gui;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Tires and steering part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildTiresAndSteeringScenario(CoreGraph graph)
        {
            // Vertices
            UserTaskVertex section9Start = CreateWaitTask("stage9Start", timeBetweenVerts);
            section9Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage9Start");
            section9Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage9Start");

            GraphVertex leakCheck = new GraphVertex(); //This vertex is skipped

            UserTaskVertex frontLeftTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontLeftTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontLeftTire"),
            };
            UserTaskVertex frontRightTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontRightTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontRightTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontRightTire"),
            };

            UserTaskVertex frontAxleInspectionCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontAxle.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleFrontAxle"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionFrontAxle"),
            };

            UserTaskVertex openCabDoor = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage21OpenDoor"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage21OpenDoor"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            UserTaskVertex exitCabDoor = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleExitBackhoe"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionExitBackhoe"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            UserTaskVertex steeringInspectionCheck = new UserTaskVertex(typeof(InspectableManager),CheckTypes.ElementsInspected, backhoe.SteeringWheel.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleSteering"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSteering"),
            };

            UserTaskVertex section10Start = CreateWaitTask("stage10Start", timeBetweenVerts);
            section10Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage10Start");
            section10Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage10Start");

            frontLeftTireCheck.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.FrontLeftTire.GroupElements);
            };

            frontLeftTireCheck.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 2f);

            frontRightTireCheck.OnEnterVertex += (_) => module.InspectableManager.UpdateActiveElements(sceneData.FrontRightTire.GroupElements);
            section9Start.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().SetupTarget(1);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                backhoe.InteractManager.SetWheels(false);
            };

            openCabDoor.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            openCabDoor.OnLeaveVertex += (_) => backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);

            frontAxleInspectionCheck.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.UnderFrontAxle));
                module.InspectableManager.AddActiveElement(backhoe.Axles[0].InspectableElement);
                backhoe.InteractManager.SetAxles(false);
            };

            frontAxleInspectionCheck.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetAxles(true);
                module.InspectableManager.RemoveActiveElement(backhoe.Axles[0].InspectableElement);
            };

            exitCabDoor.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
                backhoe.InteractManager.SetSteeringWheel(true);
                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
            };

            steeringInspectionCheck.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetSteeringWheel(false);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                module.InspectableManager.AddActiveElement(backhoe.SteeringWheel.InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCabin));
                leanWindow.Set(true);
            };

            steeringInspectionCheck.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.RemoveActiveElement(backhoe.SteeringWheel.InspectableElement);
                leanWindow.Set(false);
            };

            exitCabDoor.OnLeaveVertex += (_) =>
            { 
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
                StageCompleted(Stage.Steering);
            };

            GraphEdge Section9Start_to_leakCheck = new GraphEdge(section9Start, leakCheck, new GraphData(true));
            GraphEdge leakCheck_to_frontLeftTireCheck = new GraphEdge(leakCheck, frontRightTireCheck, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));

            GraphEdge frontLeftTireCheck_to_frontRightTireCheck = new GraphEdge(frontRightTireCheck, frontLeftTireCheck, new GraphData(true));
            GraphEdge frontRightTireCheck_to_backLeftTireCheck = new GraphEdge(frontLeftTireCheck, frontAxleInspectionCheck, new GraphData(true));

            GraphEdge frontAxleInspectionCheck_to_openDoor = new GraphEdge(frontAxleInspectionCheck, openCabDoor, new GraphData(true));
            GraphEdge openDoor_to_steeringInspectionCheck = new GraphEdge(openCabDoor, steeringInspectionCheck, new GraphData(1));

            GraphEdge steeringInspectionCheck_to_exitCabDoor = new GraphEdge(steeringInspectionCheck, exitCabDoor, new GraphData(true));
            GraphEdge exitCabDoor_to_section10Start = new GraphEdge(exitCabDoor, section10Start, new GraphData(1));

            graph.AddVertices(
                section9Start,
                openCabDoor,
                exitCabDoor,
                leakCheck,
                frontLeftTireCheck,
                frontRightTireCheck,
                steeringInspectionCheck,
                frontAxleInspectionCheck,
                section10Start
                );

            graph.AddEdges(
                Section9Start_to_leakCheck,
                leakCheck_to_frontLeftTireCheck,
                frontLeftTireCheck_to_frontRightTireCheck,
                frontRightTireCheck_to_backLeftTireCheck,
                frontAxleInspectionCheck_to_openDoor,
                openDoor_to_steeringInspectionCheck,
                steeringInspectionCheck_to_exitCabDoor,
                exitCabDoor_to_section10Start
                );


            return (section9Start, section10Start);
        }
    }
}

