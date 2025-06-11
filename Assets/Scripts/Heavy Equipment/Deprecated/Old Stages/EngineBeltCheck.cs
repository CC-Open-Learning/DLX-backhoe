using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using UnityEngine;
using CheckTypes = TaskVertexManager.CheckTypes;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the fluid Engine belt check part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildEngineBeltCheckScenario(CoreGraph graph)
        {
            // Vertices
            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();

            UserTaskVertex openCabDoor = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage21OpenDoor"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage21OpenDoor"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            UserTaskVertex liftFrontLoader = new UserTaskVertex(typeof(LoaderControl), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLiftFrontLoader"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLiftFrontLoader")
            };

            UserTaskVertex lockLoaderLiftArm = new UserTaskVertex(typeof(LiftLock), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLockLoaderLiftArm"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLockLoaderLiftArm")
            };

            UserTaskVertex openFrontHood = new UserTaskVertex(typeof(FrontHood), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleOpenFrontHood"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionOpenFrontHood")
            };

            UserTaskVertex openSidePanel = new UserTaskVertex(typeof(SidePanel), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleOpenSidePanel"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionOpenSidePanel"),
                ComponentIdentifier = SidePanel.PanelSides.Right.ToString()
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

            UserTaskVertex closeSidePanel = new UserTaskVertex(typeof(SidePanel), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCloseSidePanel"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCloseSidePanel"),
                ComponentIdentifier = SidePanel.PanelSides.Right.ToString()
            };

            UserTaskVertex closeFrontHood = new UserTaskVertex(typeof(FrontHood), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCloseFrontHood"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCloseFrontHood")
            };

            UserTaskVertex unlockLoaderLiftArm = new UserTaskVertex(typeof(LiftLock), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleUnlockLoaderLiftArm"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionUnlockLoaderLiftArm")
            };

            UserTaskVertex lowerFrontLoader = new UserTaskVertex(typeof(LoaderControl), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLowerFrontLoader"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLowerFrontLoader")
            };

            UserTaskVertex moveToRightWheel = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.EngineLoaderDolly.ToString()
            };

            // Transitions
            openCabDoor.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            openCabDoor.OnLeaveVertex += (_) => backhoe.CabDoors[0].SetDoorInteractable(false);

            liftFrontLoader.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));
                ToggleInteractability(true, backhoe.LoaderControl);
            };

            lockLoaderLiftArm.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineLoaderDolly));
                ToggleInteractability(true, backhoe.LiftLock);
                ToggleInteractability(false, backhoe.LoaderControl);
            };

            lockLoaderLiftArm.OnLeaveVertex += (_) => ToggleInteractability(false, backhoe.LiftLock);

            openFrontHood.OnEnterVertex += (_) => ToggleInteractability(true, backhoe.FrontHood);

            openSidePanel.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineLoaderDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 2f);
                ToggleInteractability(true, backhoe.RightSidePanel);
                ToggleInteractability(false, backhoe.FrontHood);
            };

            checkEngineBelt.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.EngineBelt.GroupElements);
                ToggleInteractability(false, backhoe.RightSidePanel);
                ToggleInteractability(true, backhoe.EngineBelt.GetComponent<Interactable>());
            };
            checkEngineBelt.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);

            checkEngineWiring.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.EngineWiring.GroupElements);
                ToggleInteractability(false, backhoe.EngineBelt.GetComponent<Interactable>());
                ToggleInteractability(true, backhoe.EngineWiring.GetComponent<Interactable>());
            };

            checkEngineWiring.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);

            closeSidePanel.OnEnterVertex += (_) => StartCoroutine(DelaySideDoorClose());

            closeFrontHood.OnEnterVertex += (_) => ToggleInteractability(true, backhoe.FrontHood);

            unlockLoaderLiftArm.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineLoaderDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 2f);
                ToggleInteractability(true, backhoe.LiftLock);
                ToggleInteractability(false, backhoe.FrontHood);
            };

            lowerFrontLoader.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));
                ToggleInteractability(false, backhoe.LiftLock);
                ToggleInteractability(true, backhoe.LoaderControl);
            };

            lowerFrontLoader.OnLeaveVertex += (_) => ToggleInteractability(false, backhoe.LoaderControl);

            moveToRightWheel.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineLoaderDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineLoaderDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 4f);
                backhoe.CabDoors[0].CloseDoorAfterSeconds(2f);
                backhoe.CabDoors[0].SetDoorInteractable(true);
                backhoe.InteractManager.SetWheelNuts(false);
            };

            moveToRightWheel.OnLeaveVertex += (_) =>
            {
                StageCompleted(Stage.DriveBelt);
            };


            // Define Edges
            GraphEdge start_to_openCabDoor = new GraphEdge(start, openCabDoor, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge openCabDoor_to_liftFrontLoader = new GraphEdge(openCabDoor, liftFrontLoader, new GraphData(1));
            GraphEdge liftFrontLoader_to_lockLoaderLiftArm = new GraphEdge(liftFrontLoader, lockLoaderLiftArm, new GraphData(false));
            GraphEdge lockLoaderLiftArm_to_openFrontHood = new GraphEdge(lockLoaderLiftArm, openFrontHood, new GraphData(false));
            GraphEdge openFrontHood_to_openSidePanel = new GraphEdge(openFrontHood, openSidePanel, new GraphData(true));
            GraphEdge openSidePanel_to_checkEngineBelt = new GraphEdge(openSidePanel, checkEngineBelt, new GraphData(true));
            GraphEdge checkEngineBelt_to_checkEngineWiring = new GraphEdge(checkEngineBelt, checkEngineWiring, new GraphData(true));
            GraphEdge checkEngineWiring_to_closeSidePanel = new GraphEdge(checkEngineWiring, closeSidePanel, new GraphData(true));
            GraphEdge closeSidePanel_to_closeFrontHood = new GraphEdge(closeSidePanel, closeFrontHood, new GraphData(false));
            GraphEdge closeFrontHood_to_unlockLoaderLiftArm = new GraphEdge(closeFrontHood, unlockLoaderLiftArm, new GraphData(false));
            GraphEdge unlockLoaderLiftArm_to_lowerFrontLoader = new GraphEdge(unlockLoaderLiftArm, lowerFrontLoader, new GraphData(true));
            GraphEdge lowerFrontLoader_to_moveToRightWheel = new GraphEdge(lowerFrontLoader, moveToRightWheel, new GraphData(true));
            GraphEdge moveToRightWheel_to_end = new GraphEdge(moveToRightWheel, end, new GraphData(1));

            // Add Vertices and Edges
            graph.AddVertices(
                start,
                end,
                openCabDoor,
                liftFrontLoader,
                lockLoaderLiftArm,
                openFrontHood,
                openSidePanel,
                checkEngineBelt,
                checkEngineWiring,
                closeSidePanel,
                closeFrontHood,
                unlockLoaderLiftArm,
                lowerFrontLoader,
                moveToRightWheel
                );

            graph.AddEdges(
                start_to_openCabDoor,
                openCabDoor_to_liftFrontLoader,
                liftFrontLoader_to_lockLoaderLiftArm,
                lockLoaderLiftArm_to_openFrontHood,
                openFrontHood_to_openSidePanel,
                openSidePanel_to_checkEngineBelt,
                checkEngineBelt_to_checkEngineWiring,
                checkEngineWiring_to_closeSidePanel,
                closeSidePanel_to_closeFrontHood,
                closeFrontHood_to_unlockLoaderLiftArm,
                unlockLoaderLiftArm_to_lowerFrontLoader,
                lowerFrontLoader_to_moveToRightWheel,
                moveToRightWheel_to_end
                );

            return (start, end);
        }

        IEnumerator DelaySideDoorClose()
        {
            yield return new WaitForSeconds(2);

            if (backhoe.RightSidePanel.IsOpen)
                backhoe.RightSidePanel.TogglePanelOpen();
        }
    }
}
