using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using Cinemachine;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Wheels and Windows and Engine part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildWheelsAndWindowsEngineScenario(CoreGraph graph)
        {
            UserTaskVertex checkWheelsAndWindow = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.Task1_WheelsAndWindows.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWheelsAndWindows"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWheelsAndWindows")
            };

            UserTaskVertex beforeEnterHood = CreateWaitTask("beforeEnterHood", timeBetweenVerts);
            beforeEnterHood.Title = Localizer.Localize("HeavyEquipment.PreTripTitleOutsideEngine");
            beforeEnterHood.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionOutsideEngine");

            checkWheelsAndWindow.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.Task1_WheelsAndWindows.GroupElements);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(false);
                leanWindow.TurnOn();
            };

            checkWheelsAndWindow.OnLeaveVertex += (_) =>
            {
                foreach(CabWindow windTemp in backhoe.Windows)
                {
                    if(windTemp.WState == CabWindow.WindowStates.Dirty)
                        windTemp.SetClean();
                }

                backhoe.Windows[0].WindowsCleanedToast();
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(true);
                StageCompleted(Stage.WheelsAndWindows);
            };

            UserTaskVertex openFrontHood = new UserTaskVertex(typeof(FrontHood), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleOpenFrontHood"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionOpenFrontHood")
            };

            beforeEnterHood.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).GetComponent<DollyWithTargets>().MoveToTarget(0, 2f);
            beforeEnterHood.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).CanMove = false;

            openFrontHood.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.FrontHood);
            };

            openFrontHood.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
                ToggleInteractability(false, backhoe.FrontHood);
            };

            UserTaskVertex stage1CheckPuddles = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Puddles.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage1CheckPuddles"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage1CheckPuddles")
            };

            stage1CheckPuddles.OnEnterVertex += (_) => module.InspectableManager.AddActiveElement(backhoe.Puddles.InspectableElement);

            stage1CheckPuddles.OnLeaveVertex += (_) => module.InspectableManager.RemoveActiveElement(backhoe.Puddles.InspectableElement);
            stage1CheckPuddles.OnLeaveVertex += (_) => leanWindow.TurnOff();

            GraphEdge checkWheelsAndWindow_to_checkPuddles = new GraphEdge(checkWheelsAndWindow, stage1CheckPuddles, new GraphData(true));
            GraphEdge checkPuddles_to_beforeEnterHood = new GraphEdge(stage1CheckPuddles, beforeEnterHood, new GraphData(true));
            GraphEdge beforeEnterHood_to_openFrontHood = new GraphEdge(beforeEnterHood, openFrontHood, new GraphData(true));


            graph.AddVertices(checkWheelsAndWindow, openFrontHood, stage1CheckPuddles, beforeEnterHood);
            graph.AddEdges(checkWheelsAndWindow_to_checkPuddles, checkPuddles_to_beforeEnterHood, beforeEnterHood_to_openFrontHood);

            UserTaskVertex clearEngineDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.EngineDebris.ToString(),

                Title = Localizer.Localize("HeavyEquipment.PreTripTitleClearEngineDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionClearEngineDebris")
            };

            clearEngineDebris.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).CanMove = true;

            clearEngineDebris.OnEnterVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.EngineCoolant, backhoe.TransmissionDrip, backhoe.EngineDrip);
                backhoe.InteractManager.SetDebris(true);
            };

            clearEngineDebris.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.EngineCoolant);
                backhoe.InteractManager.SetDebris(false);
            };

            GraphEdge openFrontHood_to_clearEngineDebris = new GraphEdge(openFrontHood, clearEngineDebris, new GraphData(true));

            graph.AddVertex(clearEngineDebris);
            graph.AddEdges(openFrontHood_to_clearEngineDebris);

            //Engine Coolant Check
            UserTaskVertex selectCoolant = new UserTaskVertex()
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleSelectCoolant"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSelectCoolant")
            };

            selectCoolant.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.EngineCoolant, backhoe.EngineCoolant.Fluid);
            };

            GraphEdge clearEngineDebris_to_selectCoolant = new GraphEdge(clearEngineDebris, selectCoolant, new GraphData(true));

            graph.AddVertex(selectCoolant);
            graph.AddEdges(clearEngineDebris_to_selectCoolant);

            (GraphVertex start, GraphVertex end) coolantVerts = BuildCoolantScenario(graph);
            GraphEdge selectCoolant_to_checkCoolant = new GraphEdge(selectCoolant, coolantVerts.start, new GraphData(backhoe.EngineCoolant));

            graph.AddEdges(selectCoolant_to_checkCoolant);

            // Window Washer Reservoir
            UserTaskVertex checkWiperFluidTank = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.WiperFluidReservoir.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWiperFluidTank"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWiperFluidTank")
            };

            checkWiperFluidTank.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.WiperFluidReservoir.GroupElements);
                ToggleInteractability(false, backhoe.EngineCoolant, backhoe.EngineCoolant.Fluid);
                ToggleInteractability(true, backhoe.WiperFluidReservoir);
            };

            checkWiperFluidTank.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                ToggleInteractability(false, backhoe.WiperFluidReservoir);
            };

            graph.AddVertex(checkWiperFluidTank);
            GraphEdge checkCoolant_to_checkWiperFluidTank = new GraphEdge(coolantVerts.end, checkWiperFluidTank);

            //Engine Fluid Check
            UserTaskVertex selectEngineOil = new UserTaskVertex()
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleSelectEngineOil"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSelectEngineOil")
            };

            GraphEdge checkWiperFluidTank_to_selectEngineOil = new GraphEdge(checkWiperFluidTank, selectEngineOil, new GraphData(true));

            selectEngineOil.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.EngineDrip);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
            };

            (GraphVertex start, GraphVertex end) engineFluidVerts = BuildDripCheckScenario(graph, backhoe.EngineDrip);

            //Tranmission Fluid Check
            UserTaskVertex selectTransmissionFluid = new UserTaskVertex()
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleSelectTransmissionOil"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSelectTransmissionOil")
            };

            selectTransmissionFluid.OnEnterVertex += (_) =>
            {
                backhoe.TransmissionDrip.SetEnabled(true);
                backhoe.SecondTransmissionDrip.SetEnabled(false);

                ToggleInteractability(false, backhoe.EngineDrip);
                ToggleInteractability(true, backhoe.TransmissionDrip);
            };

            (GraphVertex start, GraphVertex end) transmissionFluidVerts = BuildDripCheckScenario(graph, backhoe.TransmissionDrip);

            UserTaskVertex lastEnginePrompt = CreateWaitTask("lastEnginePrompt", timeBetweenVerts);
            lastEnginePrompt.Title = Localizer.Localize("HeavyEquipment.PreTripTitleEnginePrompt");
            lastEnginePrompt.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionEnginePrompt");

            //Fuel Water Seperator
            UserTaskVertex selectFuelWaterSeperator = new UserTaskVertex()
            {
                Title = Localizer.Localize("HeavyEquipment.selectFuelWaterSeparatorTitle"),
                Description = Localizer.Localize("HeavyEquipment.selectFuelWaterSeparatorDescription")
            };

            selectFuelWaterSeperator.OnEnterVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.TransmissionDrip);
                ToggleInteractability(true, backhoe.FuelWaterSeparator);
            };

            selectFuelWaterSeperator.OnLeaveVertex += (_) => ToggleInteractability(false, backhoe.FuelWaterSeparator);

            (GraphVertex start, GraphVertex end) fuelWaterSeperatorVerts = BuildFuelWaterSeperatorScenario(graph);


            UserTaskVertex stage2AirFilterCheck = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.AirFilter.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleAirFilterCheck"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionAirFilterCheck")
            };

            stage2AirFilterCheck.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.AirFilter);
                module.InspectableManager.AddActiveElement(backhoe.AirFilter.InspectableElement);
            };

            stage2AirFilterCheck.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.AirFilter);
                module.InspectableManager.RemoveActiveElement(backhoe.AirFilter.InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
                FrontHood hood = FindObjectOfType<FrontHood>();
                if (hood.IsOpen) hood.ToggleHoodOpen(0);
                StageCompleted(Stage.EngineBay);
            };

            UserTaskVertex HydraulicFluidReservoirCheck = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.HydraulicFluidReservoir.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleHydraulicFluidReservoirCheck"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionHydraulicFluidReservoirCheck")
            };

            UserTaskVertex ObserveHydraulicFuelGauge = new UserTaskVertex(typeof(Gauge), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleObserveHydraulicFuelGauge"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionObserveHydraulicFuelGauge"),
                ComponentIdentifier = Gauge.Locations.HydraulicFluidReservoir.ToString()
            };

            ObserveHydraulicFuelGauge.OnEnterVertex += (_) => {
                ToggleInteractability(true, backhoe.HydraulicFluidReservoir.GetFuelGauge());
                ToggleInteractability(true, backhoe.HydraulicFluidReservoir);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.HydraulicFluidReservoirPOI));
            };

            HydraulicFluidReservoirCheck.OnEnterVertex += (_) => {
                module.InspectableManager.AddActiveElement(backhoe.HydraulicFluidReservoir.InspectableElement);
            };

            HydraulicFluidReservoirCheck.OnLeaveVertex += (_) => {
                module.InspectableManager.RemoveActiveElement(backhoe.HydraulicFluidReservoir.InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
                ToggleInteractability(false, backhoe.HydraulicFluidReservoir.GetFuelGauge());
                ToggleInteractability(false, backhoe.HydraulicFluidReservoir);
            };

            GraphVertex end = new GraphVertex();

            GraphEdge selectEngineOil_to_checkEngineFluid = new GraphEdge(selectEngineOil, engineFluidVerts.start, new GraphData(backhoe.EngineDrip));
            GraphEdge checkEngineFluid_to_selectTransmissionFluid = new GraphEdge(engineFluidVerts.end, selectTransmissionFluid);
            GraphEdge selectTransmissionFluid_to_checkTransmissionFluid = new GraphEdge(selectTransmissionFluid, transmissionFluidVerts.start, new GraphData(backhoe.TransmissionDrip));
            GraphEdge transmissionFluid_to_ObserveHydraulicFuelGauge = new GraphEdge(transmissionFluidVerts.end, ObserveHydraulicFuelGauge);
            GraphEdge ObserveHydraulicFuelGauge_to_HydraulicFluidReservoirCheck = new GraphEdge(ObserveHydraulicFuelGauge, HydraulicFluidReservoirCheck, new GraphData(true));
            GraphEdge HydraulicFluidReservoirCheck_to_lastPrompt = new GraphEdge(HydraulicFluidReservoirCheck, lastEnginePrompt, new GraphData(true));
            GraphEdge checkTransmissionFluid_to_selectFuelWaterSeperator = new GraphEdge(lastEnginePrompt, selectFuelWaterSeperator, new GraphData(true));
            GraphEdge selectFuelWaterSeperator_to_checkFuelWaterSeperator = new GraphEdge(selectFuelWaterSeperator, fuelWaterSeperatorVerts.start, new GraphData(backhoe.FuelWaterSeparator));
            GraphEdge lastPrompt_to_stage2HydraulicFluidCheck = new GraphEdge(fuelWaterSeperatorVerts.end, stage2AirFilterCheck);
            GraphEdge endOfFirstStage = new GraphEdge(stage2AirFilterCheck, end, new GraphData(true));

            graph.AddVertices(
                    stage2AirFilterCheck, 
                    selectEngineOil, 
                    selectTransmissionFluid, 
                    lastEnginePrompt, 
                    selectFuelWaterSeperator, 
                    ObserveHydraulicFuelGauge,
                    HydraulicFluidReservoirCheck,
                    end
                );

            graph.AddEdges(
                    transmissionFluid_to_ObserveHydraulicFuelGauge,
                    ObserveHydraulicFuelGauge_to_HydraulicFluidReservoirCheck,
                    HydraulicFluidReservoirCheck_to_lastPrompt,
                    selectFuelWaterSeperator_to_checkFuelWaterSeperator,
                    checkTransmissionFluid_to_selectFuelWaterSeperator,
                    lastPrompt_to_stage2HydraulicFluidCheck,
                    selectTransmissionFluid_to_checkTransmissionFluid,
                    checkEngineFluid_to_selectTransmissionFluid,
                    selectEngineOil_to_checkEngineFluid,
                    checkCoolant_to_checkWiperFluidTank,
                    checkWiperFluidTank_to_selectEngineOil,
                    endOfFirstStage
                );


            return (checkWheelsAndWindow, end);
        }
    }
}

