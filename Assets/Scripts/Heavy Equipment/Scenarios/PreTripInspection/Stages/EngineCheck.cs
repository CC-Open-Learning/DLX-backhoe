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
        public (GraphVertex start, GraphVertex end) BuildEngineCompartmentScenario(CoreGraph graph)
        {
            UserTaskVertex beforeEnterHood = CreateWaitTask("beforeEnterHood", timeBetweenVerts);
            beforeEnterHood.Title = Localizer.Localize("HeavyEquipment.PreTripTitleOutsideEngine");
            beforeEnterHood.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionOutsideEngine");

            UserTaskVertex delayBeforeExiting = CreateWaitTask("delayBeforeExiting", 1f);

            UserTaskVertex openFrontHood = new UserTaskVertex(typeof(FrontHood), CheckTypes.Bool);

            UserTaskVertex openSidePanel = new UserTaskVertex(typeof(SidePanel), CheckTypes.Bool);

            UserTaskVertex closeFrontHood = new UserTaskVertex(typeof(FrontHood), CheckTypes.Bool);

            UserTaskVertex closeSidePanel = new UserTaskVertex(typeof(SidePanel), CheckTypes.Bool);

            UserTaskVertex clearEngineDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.EngineDebris.ToString(),

                Title = Localizer.Localize("HeavyEquipment.PreTripTitleClearEngineDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionClearEngineDebris")
            };

            UserTaskVertex selectCoolant = CreateWaitTask("selectCoolant", coolantTimedVerts);
            selectCoolant.Title = Localizer.Localize("HeavyEquipment.PreTripTitleSelectCoolant");
            selectCoolant.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSelectCoolant");


            (GraphVertex start, GraphVertex end) coolantVerts = BuildCoolantScenario(graph);


            UserTaskVertex checkWiperFluidTank = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.WiperFluidReservoir.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWiperFluidTank"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWiperFluidTank")
            };

            UserTaskVertex selectEngineOil = new UserTaskVertex()
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleSelectEngineOil"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSelectEngineOil")
            };

            (GraphVertex start, GraphVertex end) engineFluidVerts = BuildDripCheckScenario(graph, backhoe.EngineDrip);

            UserTaskVertex selectTransmissionFluid = new UserTaskVertex()
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleSelectTransmissionOil"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSelectTransmissionOil")
            };

            (GraphVertex start, GraphVertex end) transmissionFluidVerts = BuildDripCheckScenario(graph, backhoe.TransmissionDrip);

            UserTaskVertex lastEnginePrompt = CreateWaitTask("lastEnginePrompt", timeBetweenVerts);
            lastEnginePrompt.Title = Localizer.Localize("HeavyEquipment.PreTripTitleEnginePrompt");
            lastEnginePrompt.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionEnginePrompt");

            (GraphVertex start, GraphVertex end) fuelWaterSeperatorVerts = BuildFuelWaterSeperatorScenario(graph);


            UserTaskVertex stage2AirFilterCheck = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.AirFilter.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleAirFilterCheck"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionAirFilterCheck")
            };

            UserTaskVertex HydraulicFluidReservoirCheck = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.HydraulicFluidReservoir.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleHydraulicFluidReservoirCheck"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionHydraulicFluidReservoirCheck")
            };

            UserTaskVertex checkEngineBelt = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.EngineBelt.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckEngineBelt"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckEngineBelt")
            };

            UserTaskVertex checkEngineWiring = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.EngineWiring.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckEngineWiring"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckEngineWiring")
            };

            ///Transitions
            delayBeforeExiting.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
            };

            delayBeforeExiting.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            };

            checkEngineBelt.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.EngineBelt.GroupElements);
                ToggleInteractability(true, backhoe.EngineBelt.GetComponent<Interactable>());
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineBeltPOI));
                backhoe.EngineBelt.EnableLight(true);
            };

            checkEngineBelt.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                backhoe.EngineBelt.EnableLight(false);
            };

            checkEngineWiring.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.EngineWiring.GroupElements);
                ToggleInteractability(false, backhoe.EngineBelt.GetComponent<Interactable>());
                ToggleInteractability(true, backhoe.EngineWiring.GetComponent<Interactable>());
                backhoe.EngineWiring.GetComponent<BoxCollider>().enabled = true;
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineWiringPOI));
            };

            checkEngineWiring.OnLeaveVertex += (_) =>
            {
                backhoe.EngineWiring.GetComponent<BoxCollider>().enabled = false;
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };


            beforeEnterHood.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).GetComponent<DollyWithTargets>().MoveToTarget(0, 2f);
            beforeEnterHood.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview).CanMove = false;

            openSidePanel.OnEnterVertex += (_) =>
            {
                if(!backhoe.LeftSidePanel.IsOpen) backhoe.LeftSidePanel.TogglePanelOpen();
            };

            openSidePanel.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
            };

            openFrontHood.OnEnterVertex += (_) =>
            {
                FrontHood hood = FindObjectOfType<FrontHood>();
                if (!hood.IsOpen) hood.ToggleHoodOpen(0);
            };

            openFrontHood.OnLeaveVertex += (_) =>
            {
            };



            closeSidePanel.OnEnterVertex += (_) =>
            {
                if (backhoe.LeftSidePanel.IsOpen) backhoe.LeftSidePanel.TogglePanelOpen();
            };

            closeSidePanel.OnLeaveVertex += (_) =>
            {

            };

            closeFrontHood.OnEnterVertex += (_) =>
            {
                FrontHood hood = FindObjectOfType<FrontHood>();
                if (hood.IsOpen) hood.ToggleHoodOpen(1);
            };

            closeFrontHood.OnLeaveVertex += (_) =>
            {
                backhoe.TransmissionDrip.SetEnabled(false);
                backhoe.SecondTransmissionDrip.SetEnabled(true);
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

            selectCoolant.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineCoolantPOI));

                module.InspectableManager.AddActiveElement(backhoe.EngineCoolant.InspectableElement);
                ToggleInteractability(true, backhoe.EngineCoolant);
            };

            checkWiperFluidTank.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.WiperFluidReservoir.GroupElements);
                ToggleInteractability(false, backhoe.EngineCoolant, backhoe.EngineCoolant.Fluid);
                ToggleInteractability(true, backhoe.WiperFluidReservoir);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WiperFluidPOI));
            };

            checkWiperFluidTank.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                ToggleInteractability(false, backhoe.WiperFluidReservoir);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
            };

            selectEngineOil.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.EngineDrip);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
            };

            stage2AirFilterCheck.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.AirFilter);
                module.InspectableManager.AddActiveElement(backhoe.AirFilter.InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineFilterPOI));
            };

            selectTransmissionFluid.OnEnterVertex += (_) =>
            {
                backhoe.TransmissionDrip.SetEnabled(true);
                backhoe.SecondTransmissionDrip.SetEnabled(false);

                ToggleInteractability(false, backhoe.EngineDrip);
                ToggleInteractability(true, backhoe.TransmissionDrip);
            };

            stage2AirFilterCheck.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.AirFilter);
                module.InspectableManager.RemoveActiveElement(backhoe.AirFilter.InspectableElement);
                StageCompleted(Stage.EngineBay);
            };

            HydraulicFluidReservoirCheck.OnEnterVertex += (_) => {
                module.InspectableManager.AddActiveElement(backhoe.HydraulicFluidReservoir.InspectableElement);
                ToggleInteractability(true, backhoe.HydraulicFluidReservoir);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.HydraulicFluidReservoirPOI));
            };

            HydraulicFluidReservoirCheck.OnLeaveVertex += (_) => {
                module.InspectableManager.RemoveActiveElement(backhoe.HydraulicFluidReservoir.InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
                ToggleInteractability(false, backhoe.HydraulicFluidReservoir);
            };

            GraphVertex end = new GraphVertex();

            GraphEdge beforeEnterHood_to_openFrontHood = new GraphEdge(beforeEnterHood, openFrontHood, new GraphData(true));
            GraphEdge openFrontHood_to_openSidePanel = new GraphEdge(openFrontHood, openSidePanel, new GraphData(true));
            GraphEdge openSidePanel_to_clearEngineDebris = new GraphEdge(openSidePanel, clearEngineDebris, new GraphData(true));
            GraphEdge clearEngineDebris_to_selectCoolant = new GraphEdge(clearEngineDebris, selectCoolant, new GraphData(true));
            GraphEdge selectCoolant_to_checkCoolant = new GraphEdge(selectCoolant, coolantVerts.start, new GraphData(true));
            GraphEdge checkCoolant_to_checkWiperFluidTank = new GraphEdge(coolantVerts.end, checkWiperFluidTank);
            GraphEdge checkWiperFluidTank_to_selectEngineOil = new GraphEdge(checkWiperFluidTank, selectEngineOil, new GraphData(true));
            GraphEdge selectEngineOil_to_checkEngineFluid = new GraphEdge(selectEngineOil, engineFluidVerts.start, new GraphData(backhoe.EngineDrip));
            GraphEdge checkEngineFluid_to_selectTransmissionFluid = new GraphEdge(engineFluidVerts.end, selectTransmissionFluid);
            GraphEdge selectTransmissionFluid_to_checkTransmissionFluid = new GraphEdge(selectTransmissionFluid, transmissionFluidVerts.start, new GraphData(backhoe.TransmissionDrip));
            GraphEdge transmissionFluid_to_HydraulicFluidReservoirCheck = new GraphEdge(transmissionFluidVerts.end, HydraulicFluidReservoirCheck);
            GraphEdge HydraulicFluidReservoirCheck_to_lastPrompt = new GraphEdge(HydraulicFluidReservoirCheck, lastEnginePrompt, new GraphData(true));
            GraphEdge checkTransmissionFluid_to_selectFuelWaterSeperator = new GraphEdge(lastEnginePrompt, fuelWaterSeperatorVerts.start, new GraphData(true));
            GraphEdge lastPrompt_to_stage2HydraulicFluidCheck = new GraphEdge(fuelWaterSeperatorVerts.end, stage2AirFilterCheck);
            GraphEdge stage2HydraulicFluidCheck_checkEngineBelt = new GraphEdge(stage2AirFilterCheck, checkEngineBelt, new GraphData(true));
            GraphEdge checkEngineBelt_checkEngineWiring = new GraphEdge(checkEngineBelt, checkEngineWiring, new GraphData(true));
            GraphEdge checkEngineWiring_delayBeforeExiting = new GraphEdge(checkEngineWiring, delayBeforeExiting, new GraphData(true));
            GraphEdge delayBeforeExiting_closeSidePanel = new GraphEdge(delayBeforeExiting, closeSidePanel, new GraphData(true));
            GraphEdge closeSidePanel_closeFrontHood = new GraphEdge(closeSidePanel, closeFrontHood, new GraphData(false));
            GraphEdge endOfFirstStage = new GraphEdge(closeFrontHood, end, new GraphData(false));

            graph.AddVertices(
                    beforeEnterHood,
                    openFrontHood,
                    openSidePanel,
                    clearEngineDebris,
                    selectCoolant,
                    checkWiperFluidTank,
                    selectEngineOil,
                    selectTransmissionFluid,
                    HydraulicFluidReservoirCheck,
                    lastEnginePrompt,
                    stage2AirFilterCheck,
                    checkEngineBelt,
                    checkEngineWiring,
                    delayBeforeExiting,
                    closeSidePanel,
                    closeFrontHood,
                    end
                );

            graph.AddEdges(
                beforeEnterHood_to_openFrontHood,
                openFrontHood_to_openSidePanel,
                openSidePanel_to_clearEngineDebris,
                clearEngineDebris_to_selectCoolant,
                selectCoolant_to_checkCoolant,
                checkCoolant_to_checkWiperFluidTank,
                checkWiperFluidTank_to_selectEngineOil,
                selectEngineOil_to_checkEngineFluid,
                checkEngineFluid_to_selectTransmissionFluid,
                selectTransmissionFluid_to_checkTransmissionFluid,
                transmissionFluid_to_HydraulicFluidReservoirCheck,
                HydraulicFluidReservoirCheck_to_lastPrompt,
                checkTransmissionFluid_to_selectFuelWaterSeperator,
                lastPrompt_to_stage2HydraulicFluidCheck,
                stage2HydraulicFluidCheck_checkEngineBelt,
                checkEngineBelt_checkEngineWiring,
                checkEngineWiring_delayBeforeExiting,
                delayBeforeExiting_closeSidePanel,
                closeSidePanel_closeFrontHood,
                endOfFirstStage
            );


            return (beforeEnterHood, end);
        }
    }
}

