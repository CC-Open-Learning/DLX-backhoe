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
        public (GraphVertex start, GraphVertex end) BuildCabScenario(CoreGraph graph)
        {
            // Vertices
            UserTaskVertex stage21Start = CreateWaitTask("stage21Start", timeBetweenVerts);
            stage21Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage21Start");
            stage21Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage21Start");

            UserTaskVertex stage21OpenDoor = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage21OpenDoor"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage21OpenDoor"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            UserTaskVertex checkAlarmsAndWarnings = CreateWaitTask("checkAlarmsAndWarnings", timeBetweenVerts);
            checkAlarmsAndWarnings.Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckAlarmsAndWarnings");
            checkAlarmsAndWarnings.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckAlarmsAndWarnings");

            UserTaskVertex checkBrakePedals = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.BrakePedals.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBrakePedals"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBrakePedals")
            };

            UserTaskVertex checkSteerColumnLock = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.ColumnLock.steerLock.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckSteeringLock"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckSteeringLock")
            };

            UserTaskVertex checkParkingBrake = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.ParkingBrake.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckParkingBrake"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckParkingBrake")
            };

            UserTaskVertex turnWiperOn = new UserTaskVertex(typeof(WiperButton), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleTurnOnWipers"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionTurnOnWipers")
            };

            //Check if the wiper is operational
            UserTaskVertex checkWiperOn = CreateWaitTask("checkWiperOn", timeBetweenVerts);
            checkWiperOn.Title = Localizer.Localize("HeavyEquipment.PreTripTitleWipersObserve");
            checkWiperOn.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionWipersObserve");

            UserTaskVertex checkWiper = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.Wiper.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWiper"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWiper")
            };

            UserTaskVertex turnWiperOff = new UserTaskVertex(typeof(WiperButton), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleTurnOffWipers"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionTurnOffWipers")
            };

            UserTaskVertex checkSignalLights = CreateWaitTask("checkSignalLights", timeBetweenVerts);
            checkSignalLights.Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckSignalLights");
            checkSignalLights.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckSignalLights");

            UserTaskVertex turnOnLeftIndicators = new UserTaskVertex(typeof(BackhoeLightController), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleTurnOnLeftIndicators"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionTurnOnLeftIndicators")
            };

            UserTaskVertex checkFrontLeftIndicator = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontLeftIndicator"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontLeftIndicator")
            };

            UserTaskVertex checkBackLeftIndicator = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[3].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackLeftIndicator"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackLeftIndicator")
            };

            UserTaskVertex turnOnRightIndicators = new UserTaskVertex(typeof(BackhoeLightController), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleTurnOnRightIndicators"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionTurnOnRightIndicators")
            };

            UserTaskVertex checkFrontRightIndicator = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[0].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontRightIndicator"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontRightIndicator")
            };

            UserTaskVertex checkBackRightIndicator = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[2].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackRightIndicator"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackRightIndicator")
            };

            UserTaskVertex turnOffIndicator = new UserTaskVertex(typeof(BackhoeLightController), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleTurnOffIndicatorLights"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionTurnOffIndicatorLights")
            };

            UserTaskVertex checkReverseAlarm = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.ReverseAlarm.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckReverseAlarm"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckReverseAlarm")
            };

            UserTaskVertex turnOffReverseAlarm = new UserTaskVertex(typeof(ReverseAlarm), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleTurnOffReverseAlarm"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionTurnOffReverseAlarm")
            };

            UserTaskVertex checkHorn = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.Horn.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckHorn"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckHorn")
            };

            UserTaskVertex checkGearShifting = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.GearShifter.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckGearShifting"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckGearShifting")
            };

            UserTaskVertex stage21ExitCab = new UserTaskVertex(typeof(CabDoor), CheckTypes.Bool, backhoe.CabDoors[0])
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage21ExitCab"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage21ExitCab")
            };

            // Transitions
            stage21OpenDoor.OnEnterVertex += (_) =>
            {
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.Stage21;
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            };
            stage21OpenDoor.OnLeaveVertex += (_) =>
            {
                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.None;
            };

            checkBrakePedals.OnEnterVertex += (_) =>
            {
                leanWindow.TurnOn();
                module.InspectableManager.UpdateActiveElements(sceneData.BrakePedals.GroupElements);
                sceneData.BrakePedals.ToggleGroupInteraction(false);
            };

            checkBrakePedals.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.BrakePedals.ToggleGroupInteraction(true);
            };

            checkSteerColumnLock.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.ColumnLock.steerLock);
                module.InspectableManager.AddActiveElement(backhoe.ColumnLock.steerLock.InspectableElement);
            };

            checkSteerColumnLock.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.ColumnLock.steerLock);

                module.InspectableManager.RemoveActiveElement(backhoe.ColumnLock.steerLock.InspectableElement);
            };

            checkParkingBrake.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.ParkingBrake.GroupElements);
                sceneData.ParkingBrake.ToggleGroupInteraction(false);
            };

            checkParkingBrake.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.ParkingBrake.ToggleGroupInteraction(true);
                leanWindow.TurnOff();
            };

            turnWiperOn.OnEnterVertex += (_) =>
            {
                backhoe.WiperBtn.SetBoolean(false);
                ToggleInteractability(true, backhoe.WiperBtn);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WiperPOI));
            };

            turnWiperOn.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCabin));
                ToggleInteractability(false, backhoe.WiperBtn);
            };

            checkWiper.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.Wiper.GroupElements);
                leanWindow.TurnOn();
                sceneData.Wiper.ToggleGroupInteraction(false);
            };

            checkWiper.OnLeaveVertex += (_) =>
            {
                leanWindow.TurnOff();
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Wiper.ToggleGroupInteraction(true);
            };

            turnWiperOff.OnEnterVertex += (_) =>
            {
                backhoe.WiperBtn.SetBoolean(false);
                ToggleInteractability(true, backhoe.WiperBtn);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WiperPOI));
            };

            turnWiperOff.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCabin));
                ToggleInteractability(false, backhoe.WiperBtn);
            };

            turnOffIndicator.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetLever(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCabin));
                module.InspectableManager.UpdateActiveElements(sceneData.ReverseAlarm.GroupElements);
                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
                backhoe.CabDoors[0].SetDoorInteractable(true);
            };

            turnOffIndicator.OnLeaveVertex -= (_) =>
            {
                backhoe.InteractManager.SetLever(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkHorn.OnEnterVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.ReverseAlarm);
                module.InspectableManager.UpdateActiveElements(sceneData.Horn.GroupElements);
                ToggleInteractability(true, backhoe.Horn);
                leanWindow.TurnOn();
            };

            checkHorn.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                ToggleInteractability(false, backhoe.Horn);
                leanWindow.TurnOff();
            };

            checkReverseAlarm.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                leanWindow.TurnOff();
            };

            turnOffReverseAlarm.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetLever(true);
                ToggleInteractability(false, backhoe.ReverseAlarm);
            };

            checkGearShifting.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.GearShifter.GroupElements);
                ToggleInteractability(true, backhoe.GearShifter);
            };

            checkGearShifting.OnLeaveVertex += (_) =>
             {
                 module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                 ToggleInteractability(false, backhoe.GearShifter);
                 leanWindow.TurnOff();
             };

            // Transitions for the signal light stuff
            turnOnLeftIndicators.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCabin));
            checkFrontLeftIndicator.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI));
            checkFrontLeftIndicator.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().SetupTarget(0);

            checkSignalLights.OnEnterVertex += (_) => backhoe.InteractManager.SetLever(false);

            turnOnRightIndicators.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCabin));
                backhoe.InteractManager.SetLever(false);
            };

            turnOnRightIndicators.OnLeaveVertex += (_) => backhoe.InteractManager.SetLever(true);

            checkFrontRightIndicator.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI));
            checkFrontRightIndicator.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().SetupTarget(0);

            turnOnLeftIndicators.OnEnterVertex += (_) => backhoe.InteractManager.SetLever(false);
            turnOnLeftIndicators.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            turnOnLeftIndicators.OnLeaveVertex += (_) => backhoe.InteractManager.SetLever(true);

            checkFrontLeftIndicator.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            turnOnRightIndicators.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            checkFrontRightIndicator.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            checkBackLeftIndicator.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);

            checkFrontLeftIndicator.OnEnterVertex += (_) => module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[1].InspectableElement);

            checkFrontLeftIndicator.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().SetUserControl(true);

            checkBackLeftIndicator.OnEnterVertex += (_) => module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[3].InspectableElement);

            checkFrontRightIndicator.OnEnterVertex += (_) => module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[0].InspectableElement);
            checkFrontRightIndicator.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().SetUserControl(true);

            checkBackRightIndicator.OnEnterVertex += (_) => module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[2].InspectableElement);

            checkReverseAlarm.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetLever(true);
                ToggleInteractability(true, backhoe.ReverseAlarm);
                leanWindow.TurnOn();
            };

            stage21ExitCab.OnEnterVertex += (_) =>
            {
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.Stage21End;
                backhoe.Windows[0].gameObject.SetActive(true);
            };
            stage21ExitCab.OnLeaveVertex += (_) =>
            {
                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
                backhoe.CabDoors[0].SetDoorInteractable(true);
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.None;
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
                StageCompleted(Stage.InsideCab2);
            };

            UserTaskVertex openCabDoorToExit = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleExitBackhoe"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionExitBackhoe"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            openCabDoorToExit.OnLeaveVertex += (_) => backhoe.CabDoors[0].SetDoorInteractable(false);

            UserTaskVertex exitCabAfterGearShifting = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleExitBackhoe"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionExitBackhoe"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            UserTaskVertex goToOpenPosition = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.LightsDolly.ToString()
            };

            UserTaskVertex goToReverseAlarmOpenPosition = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.LightsDolly.ToString()
            };

            goToOpenPosition.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().MoveToTarget(0, 3f);

            goToReverseAlarmOpenPosition.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().MoveToTarget(0, 3f);

            // Define Edges
            GraphEdge stage21Start_to_stage21OpenDoor = new GraphEdge(stage21Start, stage21OpenDoor, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge stage21OpenDoor_to_checkAlarmsAndWarnings = new GraphEdge(stage21OpenDoor, checkAlarmsAndWarnings, new GraphData(1));
            GraphEdge checkAlarmsAndWarnings_to_checkBrakePedals = new GraphEdge(checkAlarmsAndWarnings, checkBrakePedals, new GraphData(true));
            GraphEdge checkBrakePedals_to_checkSteerColumnLock = new GraphEdge(checkBrakePedals, checkSteerColumnLock, new GraphData(true));
            GraphEdge checkSteerColumnLock_to_checkParkingBrake = new GraphEdge(checkSteerColumnLock, checkParkingBrake, new GraphData(true));
            GraphEdge checkParkingBrake_to_turnWiperOn = new GraphEdge(checkParkingBrake, turnWiperOn, new GraphData(true));
            GraphEdge turnWiperOn_to_checkWiperOn = new GraphEdge(turnWiperOn, checkWiperOn, new GraphData(true));
            GraphEdge checkWiperOn_to_checkWiper = new GraphEdge(checkWiperOn, checkWiper, new GraphData(true));
            GraphEdge checkWiper_to_checkWiperOff = new GraphEdge(checkWiper, turnWiperOff, new GraphData(true));
            GraphEdge checkWiperOff_to_checkSignalLights = new GraphEdge(turnWiperOff, checkSignalLights, new GraphData(true));
            GraphEdge checkSignalLights_to_turnOnLeftIndicators = new GraphEdge(checkSignalLights, turnOnLeftIndicators, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge turnOnLeftIndicators_to_openCabDoorToExit = new GraphEdge(turnOnLeftIndicators, openCabDoorToExit, new GraphData(1));
            GraphEdge openCabDoorToExit_to_checkFrontLeftIndicator = new GraphEdge(openCabDoorToExit, checkFrontLeftIndicator, new GraphData(1));
            GraphEdge checkFrontLeftIndicator_to_checkBackLeftIndicator = new GraphEdge(checkFrontLeftIndicator, checkBackLeftIndicator, new GraphData(true));
            GraphEdge checkBackLeftIndicator_to_goToOpenPosition = new GraphEdge(checkBackLeftIndicator, goToOpenPosition, new GraphData(true));
            GraphEdge goToOpenPosition_to_turnOnRightIndicators = new GraphEdge(goToOpenPosition, turnOnRightIndicators, new GraphData(0));
            GraphEdge turnOnRightIndicators_to_checkFrontRightIndicator = new GraphEdge(turnOnRightIndicators, checkFrontRightIndicator, new GraphData(2));
            GraphEdge checkFrontRightIndicator_to_checkBackRightIndicator = new GraphEdge(checkFrontRightIndicator, checkBackRightIndicator, new GraphData(true));
            GraphEdge checkBackRightIndicator_to_turnOffIndicator = new GraphEdge(checkBackRightIndicator, goToReverseAlarmOpenPosition, new GraphData(true));
            GraphEdge turnOffIndicator_to_goToReverseAlarmOpenPosition = new GraphEdge(goToReverseAlarmOpenPosition, turnOffIndicator, new GraphData(0));
            GraphEdge goToReverseAlarmOpenPosition_to_checkReverseAlarm = new GraphEdge(turnOffIndicator, checkReverseAlarm, new GraphData(0));
            GraphEdge checkReverseAlarm_to_turnOffReverseAlarm = new GraphEdge(checkReverseAlarm, turnOffReverseAlarm, new GraphData(true));
            GraphEdge turnOffReverseAlarm_to_checkHorn = new GraphEdge(turnOffReverseAlarm, checkHorn, new GraphData(false));
            GraphEdge checkHorn_to_checkGearShifting = new GraphEdge(checkHorn, checkGearShifting, new GraphData(true));
            GraphEdge checkGearShifting_to_exitCabAfterGearShifting = new GraphEdge(checkGearShifting, exitCabAfterGearShifting, new GraphData(true));
            GraphEdge exitCabAfterGearShifting_to_stage21ExitCab = new GraphEdge(exitCabAfterGearShifting, stage21ExitCab, new GraphData(1));

            // Add Vertices and Edges
            graph.AddVertices(
                stage21Start,
                stage21OpenDoor,
                checkAlarmsAndWarnings,
                checkBrakePedals,
                checkSteerColumnLock,
                checkParkingBrake,
                turnWiperOn,
                checkWiper,
                checkWiperOn,
                turnWiperOff,
                checkSignalLights,
                turnOnLeftIndicators,
                openCabDoorToExit,
                checkFrontLeftIndicator,
                checkBackLeftIndicator,
                goToOpenPosition,
                turnOnRightIndicators,
                checkFrontRightIndicator,
                checkBackRightIndicator,
                turnOffIndicator,
                goToReverseAlarmOpenPosition,
                checkReverseAlarm,
                turnOffReverseAlarm,
                checkHorn,
                checkGearShifting,
                exitCabAfterGearShifting,
                stage21ExitCab);

            graph.AddEdges(
                stage21Start_to_stage21OpenDoor,
                stage21OpenDoor_to_checkAlarmsAndWarnings,
                checkAlarmsAndWarnings_to_checkBrakePedals,
                checkBrakePedals_to_checkSteerColumnLock,
                checkSteerColumnLock_to_checkParkingBrake,
                checkParkingBrake_to_turnWiperOn,
                turnWiperOn_to_checkWiperOn,
                checkWiperOn_to_checkWiper,
                checkWiper_to_checkWiperOff,
                checkWiperOff_to_checkSignalLights,
                checkSignalLights_to_turnOnLeftIndicators,
                turnOnLeftIndicators_to_openCabDoorToExit,
                openCabDoorToExit_to_checkFrontLeftIndicator,
                checkFrontLeftIndicator_to_checkBackLeftIndicator,
                checkBackLeftIndicator_to_goToOpenPosition,
                goToOpenPosition_to_turnOnRightIndicators,
                turnOnRightIndicators_to_checkFrontRightIndicator,
                checkFrontRightIndicator_to_checkBackRightIndicator,
                checkBackRightIndicator_to_turnOffIndicator,
                turnOffIndicator_to_goToReverseAlarmOpenPosition,
                goToReverseAlarmOpenPosition_to_checkReverseAlarm,
                checkReverseAlarm_to_turnOffReverseAlarm,
                turnOffReverseAlarm_to_checkHorn,
                checkHorn_to_checkGearShifting,
                checkGearShifting_to_exitCabAfterGearShifting,
                exitCabAfterGearShifting_to_stage21ExitCab);

            return (stage21Start, stage21ExitCab);
        }
    }
}
