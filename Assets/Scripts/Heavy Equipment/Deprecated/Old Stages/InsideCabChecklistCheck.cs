//using RemoteEducation.Localization;
//using RemoteEducation.Scenarios.Inspectable;
//using CheckTypes = TaskVertexManager.CheckTypes;

//namespace RemoteEducation.Modules.HeavyEquipment
//{
//    partial class PreTripInspectionGS
//    {
//        /// <summary>
//        /// Build the graph for the Inside cab checklist part of the scenario.
//        /// </summary>
//        /// <param name="graph">The graph in the scene.</param>
//        /// <returns>
//        /// Start = A dummy vertex at the start of this section of the scenario. 
//        ///   End = A dummy vertex at the end of this section of the scenario.
//        /// </returns>
//        public (GraphVertex start, GraphVertex end) BuildCabChecklistScenario(CoreGraph graph)
//        {
//            // Vertices
//            UserTaskVertex stage3Start = CreateWaitTask("stage3Start", timeBetweenVerts);
//            stage3Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3Start");
//            stage3Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3Start");

//            UserTaskVertex stage3OpenDoor = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3OpenDoor"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3OpenDoor"),
//                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
//            };

//            UserTaskVertex stage3TurningKey = new UserTaskVertex(typeof(Key), CheckTypes.Bool)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3TurningKey"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3TurningKey")
//            };

//            UserTaskVertex stage3ReadingDiagonistics = CreateWaitTask("stage3ReadingDiagonistics", timeBetweenVerts);
//            stage3ReadingDiagonistics.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3ReadingDiagonistics");
//            stage3ReadingDiagonistics.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3ReadingDiagonistics");

//            UserTaskVertex stage3Checklist = new UserTaskVertex(typeof(ClipboardController), CheckTypes.Bool)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3Checklist"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3Checklist")
//            };

//            UserTaskVertex stage3ObserveFuelGauge = new UserTaskVertex(typeof(Gauge), CheckTypes.Bool)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3ObserveFuelGauge"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3ObserveFuelGauge"),
//                ComponentIdentifier = Gauge.Locations.GaugePanel.ToString()
//            };

//            UserTaskVertex stage3TurnKeyOff = new UserTaskVertex(typeof(Key), CheckTypes.Bool)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3TurnKeyOff"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3TurnKeyOff")
//            };

//            UserTaskVertex stage3ExitCabToFillTank = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3ExitCabToFillTank"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3ExitCabToFillTank"),
//                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
//            };

//            UserTaskVertex stage3FillTank = new UserTaskVertex(typeof(FuelTank), CheckTypes.Bool, backhoe.FuelTank)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage3FillTank"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage3FillTank")
//            };

//            UserTaskVertex stage4TurningOnEngine = new UserTaskVertex(typeof(Key), CheckTypes.Bool)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage4TurningOnEngine"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage4TurningOnEngine")
//            };

//            UserTaskVertex toggleRearBoomLockLatch = new UserTaskVertex(typeof(BoomLockLatch), CheckTypes.Bool)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleToggleRearBoomLockLatch"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionToggleRearBoomLockLatch")
//            };

//            UserTaskVertex checkRearBoomLockLatch = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.BoomLockLatch.InspectableElement)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearBoomLockLatch"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearBoomLockLatch")
//            };


//            UserTaskVertex stage4waitingInCab = CreateWaitTask("stage4waitingInCab", timeBetweenVerts);
//            stage4waitingInCab.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage4WaitingInCab");
//            stage4waitingInCab.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage4WaitingInCab");

//            UserTaskVertex stage5ExitCab = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
//            {
//                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage5ExitCab"),
//                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage5ExitCab"),
//                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
//            };

//            UserTaskVertex stage5End = CreateWaitTask("stage5End", timeBetweenVerts);
//            stage5End.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage5End");
//            stage5End.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage5End");

//            // Transitions
//            stage3OpenDoor.OnEnterVertex += (_) =>
//            {
//                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.Stage3;
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
//            };
//            stage3OpenDoor.OnLeaveVertex += (_) =>
//            {
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.KeyPOI));
//                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.None;
//                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
//            };

//            stage3TurningKey.OnEnterVertex += (_) =>
//            {
//                backhoe.Key.CanTurnOnEngine = true;
//                backhoe.Key.currentIteration = Key.ScenarioPass.Stage3;
//                backhoe.GaugePanel.scenario = (GaugePanelController.DamageScenario)UnityEngine.Random.Range(1, 3);
//                ToggleInteractability(true, backhoe.Key);
//            };
//            stage3TurningKey.OnLeaveVertex += (_) =>
//            {
//                backhoe.Key.CanTurnOnEngine = false;
//                backhoe.Key.currentIteration = Key.ScenarioPass.None;
//                backhoe.Key.Clicked = false;
//                ToggleInteractability(false, backhoe.Key);
//            };

//            stage3ReadingDiagonistics.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InteriorSidePanel));

//            stage3Checklist.OnEnterVertex += (_) =>
//            {
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.ChecklistPOI));
//                backhoe.GaugePanelAnimator.OpenClipboard();
//            };

//            stage3Checklist.OnLeaveVertex += (_) =>
//            {
//                backhoe.GaugePanelAnimator.CloseClipboard();
//            };

//            stage3ObserveFuelGauge.OnEnterVertex += (_) =>
//            {
//                ToggleInteractability(true, backhoe.FuelTank.GetFuelGauge());
//            };

//            stage3ObserveFuelGauge.OnLeaveVertex += (_) =>
//            {
//                ToggleInteractability(false, backhoe.FuelTank.GetFuelGauge());
//            };

//            stage3TurnKeyOff.OnEnterVertex += (_) => ToggleInteractability(true, backhoe.Key);
//            stage3TurnKeyOff.OnLeaveVertex += (_) => ToggleInteractability(false, backhoe.Key);

//            stage3FillTank.OnEnterVertex += (_) => ToggleInteractability(true, backhoe.FuelTank);
//            stage3FillTank.OnLeaveVertex += (_) => ToggleInteractability(false, backhoe.FuelTank);

//            stage3ExitCabToFillTank.OnEnterVertex += (_) =>
//            {
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));
//                backhoe.CabDoors[0].CanClear = false;
//                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.StageFuelTank;
//            };

//            stage3ExitCabToFillTank.OnLeaveVertex += (_) =>
//            {
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
//                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.None;
//                backhoe.CabDoors[0].SetDoorInteractable(false);
//            };

//            stage3TurnKeyOff.OnEnterVertex += (_) =>
//            {
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.KeyPOI));
//                backhoe.Key.CanTurnOnEngine = true;
//                backhoe.Key.currentIteration = Key.ScenarioPass.Stage35;
//            };

//            stage3TurnKeyOff.OnLeaveVertex += (_) =>
//            {
//                backhoe.Key.CanTurnOnEngine = false;
//                backhoe.Key.currentIteration = Key.ScenarioPass.None;
//                backhoe.Key.Clicked = false;
//            };

//            stage4TurningOnEngine.OnEnterVertex += (_) =>
//            {
//                backhoe.Key.CanTurnOnEngine = true;
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.KeyPOI));
//                backhoe.Key.currentIteration = Key.ScenarioPass.Stage4;
//                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
//                ToggleInteractability(true, backhoe.Key);
//            };

//            stage4TurningOnEngine.OnLeaveVertex += (_) =>
//            {
//                backhoe.Key.CanTurnOnEngine = false;
//                backhoe.Key.currentIteration = Key.ScenarioPass.None;
//                backhoe.Key.Clicked = false;
//                backhoe.CabDoors[0].CanClear = false;
//                ToggleInteractability(false, backhoe.Key);
//            };

//            toggleRearBoomLockLatch.OnEnterVertex += (_) => {
//                ToggleInteractability(true, backhoe.BoomLockLever);
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.BoomLatchLeverPOI));
//                // hide rear window for inspecting the boom lock latch
//                backhoe.Windows[0].gameObject.SetActive(false);
//            };
//            toggleRearBoomLockLatch.OnLeaveVertex += (_) => {
//                ToggleInteractability(false, backhoe.BoomLockLever);
//            };
//            checkRearBoomLockLatch.OnEnterVertex += (_) => {
//                ToggleInteractability(true, backhoe.BoomLockLatch);
//                module.InspectableManager.AddActiveElement(backhoe.BoomLockLatch.InspectableElement);
//            };
//            checkRearBoomLockLatch.OnLeaveVertex += (_) => {
//                ToggleInteractability(false, backhoe.BoomLockLatch);
//                module.InspectableManager.RemoveActiveElement(backhoe.BoomLockLatch.InspectableElement);
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));
//            };

//            stage4waitingInCab.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));

//            stage5ExitCab.OnEnterVertex += (_) =>
//            {
//                backhoe.CabDoors[0].CanClear = false;
//                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.Stage5;
//                backhoe.CabDoors[0].SetDoorInteractable(true);
//                // unhide rear window after inspecting the boom lock latch
//                backhoe.Windows[0].gameObject.SetActive(true);
//            };
//            stage5ExitCab.OnLeaveVertex += (_) =>
//            {
//                backhoe.CabDoors[0].currentIteration = CabDoor.ScenarioPass.None;
//                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview));
//                sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).GetComponent<DollyWithTargets>().SetUserControl(false);
//                sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).GetComponent<DollyWithTargets>().SetupTarget(0);
//                StageCompleted(Stage.InsideCab1);
//            };

//            stage5ExitCab.OnLeaveVertex += (_) => backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);

//            //stage5End.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));

//            GraphVertex end = new GraphVertex();

//            GraphEdge stage3Start_to_stage3OpenDoor = new GraphEdge(stage3Start, stage3OpenDoor, new GraphData(true));
//            GraphEdge stage3OpenDoor_to_stage3TurningKey = new GraphEdge(stage3OpenDoor, stage3TurningKey, new GraphData(1));
//            GraphEdge stage3TurningKey_to_stage3ReadingDiagonistics = new GraphEdge(stage3TurningKey, stage3ReadingDiagonistics, new GraphData(true));
//            GraphEdge stage3ReadingDiagonistics_to_stage3Checklist = new GraphEdge(stage3ReadingDiagonistics, stage3Checklist, new GraphData(true));
//            GraphEdge stage3Checklist_to_stage3ObserveFuelGauge = new GraphEdge(stage3Checklist, stage3ObserveFuelGauge, new GraphData(true));
//            GraphEdge stage3ObserveFuelGauge_to_stage3TurnKeyOff = new GraphEdge(stage3ObserveFuelGauge, stage3TurnKeyOff, new GraphData(true));
//            GraphEdge stage3TurnKeyOff_to_stage3ExitCabToFillTank = new GraphEdge(stage3TurnKeyOff, stage3ExitCabToFillTank, new GraphData(true));
//            GraphEdge stage3ExitCabToFillTank_to_stage3FillTank = new GraphEdge(stage3ExitCabToFillTank, stage3FillTank, new GraphData(1));
//            GraphEdge stage3FillTank_to_stage4TurningOnEngine = new GraphEdge(stage3FillTank, stage4TurningOnEngine, new GraphData(true));
//            GraphEdge stage4TurningOnEngine_to_toggleRearBoomLockLatch = new GraphEdge(stage4TurningOnEngine, toggleRearBoomLockLatch, new GraphData(true));
//            GraphEdge toggleRearBoomLockLatch_to_checkRearBoomLockLatch = new GraphEdge(toggleRearBoomLockLatch, checkRearBoomLockLatch, new GraphData(true));
//            GraphEdge checkRearBoomLockLatch_to_stage4waitingInCab = new GraphEdge(checkRearBoomLockLatch, stage4waitingInCab, new GraphData(true));
//            GraphEdge stage4waitingInCab_to_stage5ExitCab = new GraphEdge(stage4waitingInCab, stage5ExitCab, new GraphData(true));
//            GraphEdge stage5ExitCab_to_stage5End = new GraphEdge(stage5ExitCab, stage5End, new GraphData(1));
//            GraphEdge stage5End_to_end = new GraphEdge(stage5End, end, new GraphData(true));

//            graph.AddVertices(
//                stage3Start,
//                stage3OpenDoor,
//                stage3TurningKey,
//                stage3ReadingDiagonistics,
//                stage3Checklist,
//                stage3ObserveFuelGauge,
//                stage3TurnKeyOff,
//                stage3ExitCabToFillTank,
//                stage3FillTank,
//                stage4TurningOnEngine,
//                toggleRearBoomLockLatch,
//                checkRearBoomLockLatch,
//                stage4waitingInCab,
//                stage5ExitCab,
//                stage5End,
//                end);

//            graph.AddEdges(
//                stage3Start_to_stage3OpenDoor,
//                stage3OpenDoor_to_stage3TurningKey,
//                stage3TurningKey_to_stage3ReadingDiagonistics,
//                stage3ReadingDiagonistics_to_stage3Checklist,
//                stage3Checklist_to_stage3ObserveFuelGauge,
//                stage3ObserveFuelGauge_to_stage3TurnKeyOff,
//                stage3TurnKeyOff_to_stage3ExitCabToFillTank,
//                stage3ExitCabToFillTank_to_stage3FillTank,
//                stage3FillTank_to_stage4TurningOnEngine,
//                stage4TurningOnEngine_to_toggleRearBoomLockLatch,
//                toggleRearBoomLockLatch_to_checkRearBoomLockLatch,
//                checkRearBoomLockLatch_to_stage4waitingInCab,
//                stage4waitingInCab_to_stage5ExitCab,
//                stage5ExitCab_to_stage5End,
//                stage5End_to_end
//                );

//            return (stage3Start, end);
//        }
//    }
//}

