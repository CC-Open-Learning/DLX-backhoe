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
        public (GraphVertex start, GraphVertex end) BuildInteriorInspectionScenario(CoreGraph graph)
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

            UserTaskVertex steeringInspectionCheck = new UserTaskVertex(typeof(Steering), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleSteering"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSteering"),
            };

            UserTaskVertex turnKey = new UserTaskVertex(typeof(Key), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3TurningKey"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3TurningKey")
            };

            UserTaskVertex turnOnEngine = new UserTaskVertex(typeof(Key), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage4TurningOnEngine"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage4TurningOnEngine")
            };

            UserTaskVertex checkAlarmsAndWarnings = CreateWaitTask("checkAlarmsAndWarnings", timeBetweenVerts);
            checkAlarmsAndWarnings.Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckAlarmsAndWarnings");
            checkAlarmsAndWarnings.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckAlarmsAndWarnings");

            UserTaskVertex watchGaugeSweep = CreateWaitTask("checkAlarmsAndWarnings", timeBetweenVerts);
            watchGaugeSweep.Title = Localizer.Localize("HeavyEquipment.PreTripTitleWatchGaugeSweep");
            watchGaugeSweep.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionWatchGaugeSweep");

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

            UserTaskVertex checkReverseAlarm = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.ReverseAlarm.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckReverseAlarm"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckReverseAlarm")
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

            UserTaskVertex turnOnLights = new UserTaskVertex(typeof(BackhoeLightController), TaskVertexManager.CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleBackHoeLights"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionBackHoeLights"),
            };

            GraphVertex end = new GraphVertex();

            // Transitions
            steeringInspectionCheck.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetSteeringWheel(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCabin));
            };

            steeringInspectionCheck.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetSteeringWheel(true);
            };

            turnOnLights.OnEnterVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InteriorSidePanel));
                backhoe.BackhoeLightController.EnableLightSwitchColliders(true);
            };

            turnOnLights.OnLeaveVertex += (_) => {
                backhoe.BackhoeLightController.EnableLightSwitchColliders(false);
            };

            turnKey.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.KeyPOI));
                backhoe.Key.CanTurnOnEngine = true;
                backhoe.GaugePanel.scenario = (GaugePanelController.DamageScenario)UnityEngine.Random.Range(1, 3);
                ToggleInteractability(true, backhoe.Key);
            };
            turnKey.OnLeaveVertex += (_) =>
            {
                backhoe.Key.CanTurnOnEngine = false;
                backhoe.Key.Clicked = false;
                ToggleInteractability(false, backhoe.Key);
            };
            turnOnEngine.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.KeyPOI));
                backhoe.Key.CanTurnOnEngine = true;
                ToggleInteractability(true, backhoe.Key);
            };
            turnOnEngine.OnLeaveVertex += (_) =>
            {
                backhoe.Key.Clicked = false;
                ToggleInteractability(false, backhoe.Key);
            };

            stage21OpenDoor.OnEnterVertex += (_) =>
            {
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.Stage21;
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            };
            stage21OpenDoor.OnLeaveVertex += (_) =>
            {
                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.None;
            };

            checkBrakePedals.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.CabPedalsPOI));
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
                backhoe.BackhoeBrakePedals.GetComponent<BoxCollider>().enabled = false;
            };

            checkSteerColumnLock.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.ColumnLock.steerLock);

                module.InspectableManager.RemoveActiveElement(backhoe.ColumnLock.steerLock.InspectableElement);
            };

            checkParkingBrake.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.ParkingBrakePOI));
                module.InspectableManager.UpdateActiveElements(sceneData.ParkingBrake.GroupElements);
                sceneData.ParkingBrake.ToggleGroupInteraction(false);
                backhoe.ParkingBrake.GetComponent<SphereCollider>().enabled = true;
            };

            checkParkingBrake.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.ParkingBrake.ToggleGroupInteraction(true);
                backhoe.ParkingBrake.GetComponent<SphereCollider>().enabled = false;
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
                sceneData.Wiper.ToggleGroupInteraction(false);
                backhoe.WindShieldWipers.GetComponent<BoxCollider>().enabled = true;

                //The front window collider blocks the wiper collider to make the cursor changes even when hovering on where the wipers' collider should be
                backhoe.Windows[2].GetComponent<MeshCollider>().enabled = false;
            };

            checkWiper.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Wiper.ToggleGroupInteraction(true);
                backhoe.WindShieldWipers.GetComponent<BoxCollider>().enabled = false;

                //Enabling the front window collider after inspecting the wiper collider
                backhoe.Windows[2].GetComponent<MeshCollider>().enabled = true;
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
                backhoe.InteractManager.SetLever(false);
            };

            checkHorn.OnEnterVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.ReverseAlarm);
                module.InspectableManager.UpdateActiveElements(sceneData.Horn.GroupElements);
                ToggleInteractability(true, backhoe.Horn);
            };

            checkHorn.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                ToggleInteractability(false, backhoe.Horn);
            };

            checkReverseAlarm.OnLeaveVertex += (_) =>
            {

                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                backhoe.InteractManager.SetLever(true);
                backhoe.ReverseAlarm.GetComponent<SphereCollider>().enabled = false;
                ToggleInteractability(false, backhoe.ReverseAlarm);

                if(backhoe.ReverseAlarm.IsLeverOn) backhoe.ReverseAlarm.ToggleLever();
            };

            checkGearShifting.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.GearShifter.GroupElements);
                ToggleInteractability(true, backhoe.GearShifter);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GearShifterPOI));
            };

            checkGearShifting.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                ToggleInteractability(false, backhoe.GearShifter);
            };

            checkReverseAlarm.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetLever(true);
                module.InspectableManager.UpdateActiveElements(sceneData.ReverseAlarm.GroupElements);
                ToggleInteractability(true, backhoe.ReverseAlarm);
                backhoe.ReverseAlarm.GetComponent<SphereCollider>().enabled = true;
            };


            // Define Edges
            GraphEdge stage21Start_to_stage21OpenDoor = new GraphEdge(stage21Start, stage21OpenDoor, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge stage21OpenDoor_to_turnKey = new GraphEdge(stage21OpenDoor, turnKey, new GraphData(1));
            GraphEdge turnKey_to_watchGaugeSweep = new GraphEdge(turnKey, watchGaugeSweep, new GraphData(1));
            GraphEdge watchGaugeSweep_to_turnOnEngine = new GraphEdge(watchGaugeSweep, turnOnEngine, new GraphData(true));
            GraphEdge turnOnEngine_to_checkAlarmsAndWarnings = new GraphEdge(turnOnEngine, checkAlarmsAndWarnings, new GraphData(2));
            GraphEdge checkAlarmsAndWarnings_to_steeringInspectionCheck = new GraphEdge(checkAlarmsAndWarnings, steeringInspectionCheck, new GraphData(true));
            GraphEdge steeringInspectionCheck_to_checkBrakePedals = new GraphEdge(steeringInspectionCheck, checkBrakePedals, new GraphData(true));
            GraphEdge checkBrakePedals_to_checkSteerColumnLock = new GraphEdge(checkBrakePedals, checkSteerColumnLock, new GraphData(true));
            GraphEdge checkSteerColumnLock_to_checkParkingBrake = new GraphEdge(checkSteerColumnLock, checkParkingBrake, new GraphData(true));
            GraphEdge checkParkingBrake_to_turnWiperOn = new GraphEdge(checkParkingBrake, turnWiperOn, new GraphData(true));
            GraphEdge turnWiperOn_to_checkWiperOn = new GraphEdge(turnWiperOn, checkWiperOn, new GraphData(true));
            GraphEdge checkWiperOn_to_checkWiper = new GraphEdge(checkWiperOn, checkWiper, new GraphData(true));
            GraphEdge checkWiper_to_checkWiperOff = new GraphEdge(checkWiper, turnWiperOff, new GraphData(true));
            GraphEdge checkWiperOff_to_checkReverseAlarm = new GraphEdge(turnWiperOff, checkReverseAlarm, new GraphData(true));
            GraphEdge checkReverseAlarm_to_checkHorn = new GraphEdge(checkReverseAlarm, checkHorn, new GraphData(true));
            GraphEdge checkHorn_to_checkGearShifting = new GraphEdge(checkHorn, checkGearShifting, new GraphData(true));
            GraphEdge checkGearShifting_to_turnOnLights = new GraphEdge(checkGearShifting, turnOnLights, new GraphData(true));
            GraphEdge turnOnLights_to_end = new GraphEdge(turnOnLights, end, new GraphData(1));

            // Add Vertices and Edges
            graph.AddVertices(
                stage21Start,
                stage21OpenDoor,
                turnKey,
                watchGaugeSweep,
                turnOnEngine,
                checkAlarmsAndWarnings,
                steeringInspectionCheck,
                checkBrakePedals,
                checkSteerColumnLock,
                checkParkingBrake,
                turnWiperOn,
                checkWiper,
                checkWiperOn,
                turnWiperOff,
                checkReverseAlarm,
                checkHorn,
                checkGearShifting,
                turnOnLights,
                end
                );

            graph.AddEdges(
                stage21Start_to_stage21OpenDoor,
                stage21OpenDoor_to_turnKey,
                turnKey_to_watchGaugeSweep,
                watchGaugeSweep_to_turnOnEngine,
                turnOnEngine_to_checkAlarmsAndWarnings,
                checkAlarmsAndWarnings_to_steeringInspectionCheck,
                steeringInspectionCheck_to_checkBrakePedals,
                checkBrakePedals_to_checkSteerColumnLock,
                checkSteerColumnLock_to_checkParkingBrake,
                checkParkingBrake_to_turnWiperOn,
                turnWiperOn_to_checkWiperOn,
                checkWiperOn_to_checkWiper,
                checkWiper_to_checkWiperOff,
                checkWiperOff_to_checkReverseAlarm,
                checkReverseAlarm_to_checkHorn,
                checkHorn_to_checkGearShifting,
                checkGearShifting_to_turnOnLights,
                turnOnLights_to_end
                );

            return (stage21Start, end);
        }
    }
}

