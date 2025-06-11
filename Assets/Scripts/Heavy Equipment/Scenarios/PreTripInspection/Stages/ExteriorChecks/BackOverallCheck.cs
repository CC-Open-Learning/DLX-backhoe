using Lean.Gui;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using CheckTypes = TaskVertexManager.CheckTypes;
using RemoteEducation.Scenarios;

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
        public (GraphVertex start, GraphVertex end) BuildBackOverallCheck(CoreGraph graph)
        {
            GraphVertex end = new GraphVertex();

            UserTaskVertex checkBackWindows = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearWindows.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWheelsAndWindows"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWheelsAndWindows")
            };

            UserTaskVertex inspectBackAxle = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Axles[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearAxle"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearAxle")
            };

            UserTaskVertex inspectFluidPuddles = new UserTaskVertex(typeof(Puddles), CheckTypes.ElementsInspected, backhoe.Puddles.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage1CheckPuddles"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage1CheckPuddles")
            };

            UserTaskVertex resetCameraAfterPuddle = CreateWaitTask("resetCameraAfterPuddle", 2f);

            UserTaskVertex checkSwingLinkageDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.SwingLinkage.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckSwingLinkageDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckSwingLinkageDebris")
            };

            UserTaskVertex checkSwingLinkageHydraulic = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.Stage14Hydraulics.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckSwingLinkageHydraulic"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckSwingLinkageHydraulic")
            };

            UserTaskVertex checkSwingLinkagePinsAndRetainers = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckSwingLinkagePinsAndRetainers"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckSwingLinkagePinsAndRetainers")
            };

            UserTaskVertex resetSwingLinkageCamera = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.SwingLinkageDolly.ToString()
            };

            UserTaskVertex disengageSwingLockoutPin = new UserTaskVertex(typeof(SwingLockoutPin), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleDisengageSwingLockoutPin"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionDisengageSwingLockoutPin")
            };

            UserTaskVertex resetAfterSwingLockoutPin = CreateWaitTask("resetAfterSwingLockoutPin", 2f);

            UserTaskVertex checkPistons = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearBoomPistons.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLeftPiston"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLeftPiston"),
            };

            UserTaskVertex checkLines = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearBoomLines.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLeftHydraulics"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLeftHydraulics"),
            };

            UserTaskVertex checkBracketsLeft = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearBoomBrackets.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearBoomRightSide"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearBoomRightSide")
            };

            UserTaskVertex checkBucketLinkageDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.BucketLinkage.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBucketLinkageDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBucketLinkageDebris")
            };

            UserTaskVertex checkRearBucketPinsAndRetainers = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearBucketPinsAndRetainers"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearBucketPinsAndRetainers")
            };

            UserTaskVertex inspectBucketTeeth = new UserTaskVertex(typeof(BoomBucket), CheckTypes.Bool)
            {
                Title = "Inspect the teeth on the boom bucket",
                Description = "Inspect the condition of the teeth on the bucket."
            };

            UserTaskVertex checkRearBucket = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.BoomBucket.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectRearBucket"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectRearBucket")
            };

            UserTaskVertex checkRearBucketHydraulic = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearBucketHydraulic.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearBucketHydraulic"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearBucketHydraulic")
            };

            UserTaskVertex checkRightBrackets = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearBoomRightSide.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearBoomRightSide"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearBoomRightSide")
            };

            //Need to do all windows
            checkBackWindows.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(sceneData.RearWindows.GroupElements[0]);
                module.InspectableManager.AddActiveElement(sceneData.RearWindows.GroupElements[1]);
                sceneData.RearWindows.ToggleGroupInteraction(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearLinkagePOIRight));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearLinkagePOIRight).GetComponent<PointOfInterest>().transform.LookAt(sceneData.RearWindows.GroupElements[0].transform.position);
            };

            checkBackWindows.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearWindows.ToggleGroupInteraction(true);
            };

            inspectBackAxle.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.UnderRearAxle));
                module.InspectableManager.AddActiveElement(backhoe.Axles[1].InspectableElement);
                backhoe.InteractManager.SetAxles(false);
            };

            inspectBackAxle.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetAxles(true);
                module.InspectableManager.RemoveActiveElement(backhoe.Axles[1].InspectableElement);
            };

            inspectFluidPuddles.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.Puddles.InspectableElement);
                backhoe.Puddles.GetComponent<Interactable>().ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
            };

            inspectFluidPuddles.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.RemoveActiveElement(backhoe.Puddles.InspectableElement);
                backhoe.Puddles.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            };

            resetCameraAfterPuddle.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomFrontSide));
            };

            checkSwingLinkageDebris.OnEnterVertex += (_) =>
            {
                //Spawn and remove debris
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp).GetComponent<DollyWithTargets>().MoveToTarget(1, 1f);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp).GetComponent<DollyWithTargets>().SetUserControl(true);
                backhoe.InteractManager.SetDebris(true);
            };

            checkSwingLinkageDebris.OnLeaveVertex += (_) => backhoe.InteractManager.SetDebris(false);

            checkSwingLinkageHydraulic.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.Stage14Hydraulics.GroupElements);
                sceneData.Stage14Hydraulics.ToggleGroupInteraction(false);
            };

            checkSwingLinkageHydraulic.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Stage14Hydraulics.ToggleGroupInteraction(true);
            };

            resetSwingLinkageCamera.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp).GetComponent<DollyWithTargets>().MoveToTarget(0, 3f);
            resetSwingLinkageCamera.OnLeaveVertex += (_) => StageCompleted(Stage.SwingLinkage);

            disengageSwingLockoutPin.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.SwingLockoutPin);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearLinkagePOIRight));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearLinkagePOIRight).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 30f;
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearLinkagePOIRight).GetComponent<PointOfInterest>().transform.LookAt(backhoe.SwingLockoutPin.transform.position);
            };

            disengageSwingLockoutPin.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.SwingLockoutPin);
            };

            resetAfterSwingLockoutPin.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomFrontSide));
            };

            checkSwingLinkagePinsAndRetainers.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);
            checkSwingLinkagePinsAndRetainers.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            checkPistons.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomLeftSide));
                module.InspectableManager.UpdateActiveElements(sceneData.RearBoomPistons.GroupElements);
                sceneData.RearBoomPistons.ToggleGroupInteraction(false);
            };

            checkPistons.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearBoomPistons.ToggleGroupInteraction(true);
            };

            checkLines.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.RearBoomLines.GroupElements);
                sceneData.RearBoomLines.ToggleGroupInteraction(false);
            };

            checkLines.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearBoomLines.ToggleGroupInteraction(true);
            };

            checkBracketsLeft.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.BracketLeft));
                module.InspectableManager.UpdateActiveElements(sceneData.RearBoomBrackets.GroupElements);
                sceneData.RearBoomBrackets.ToggleGroupInteraction(false);
            };

            checkBracketsLeft.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearBoomBrackets.ToggleGroupInteraction(true);
            };

            checkBucketLinkageDebris.OnEnterVertex += (_) =>
            {
                // Spawn and remove debris
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.BucketLinkageDebris));
                backhoe.InteractManager.SetDebris(true);
            };

            checkBucketLinkageDebris.OnLeaveVertex += (_) => backhoe.InteractManager.SetDebris(false);

            checkRearBucketPinsAndRetainers.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);
            checkRearBucketPinsAndRetainers.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview));
                backhoe.InteractManager.SetPins(true);
            };

            checkRearBucketHydraulic.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomFrontSide));
                module.InspectableManager.UpdateActiveElements(sceneData.RearBucketHydraulic.GroupElements);
                sceneData.RearBucketHydraulic.ToggleGroupInteraction(false);
            };

            checkRearBucketHydraulic.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearBucketHydraulic.ToggleGroupInteraction(true);
            };

            checkRightBrackets.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomRightSide));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomRightSide).transform.LookAt(backhoe.InteractManager.brackets[2].transform.position);
                module.InspectableManager.UpdateActiveElements(sceneData.RearBoomRightSide.GroupElements);
                sceneData.RearBoomRightSide.ToggleGroupInteraction(false);
            };

            checkRightBrackets.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearBoomRightSide.ToggleGroupInteraction(true);
                StageCompleted(Stage.RearBoom);
            };

            inspectBucketTeeth.OnEnterVertex += (_) =>
            {
                backhoe.BoomBucket.ToggleWornTeethInteraction(true);
                backhoe.InteractManager.SetBoomBucketTeeth(true);
            };

            inspectBucketTeeth.OnLeaveVertex += (_) =>
            {
                backhoe.BoomBucket.ToggleWornTeethInteraction(false);
                backhoe.InteractManager.SetBoomBucketTeeth(false);
            };

            checkRearBucket.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.BoomBucket.InspectableElement);
            };

            checkRearBucket.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.RemoveActiveElement(backhoe.BoomBucket.InspectableElement);
            };

            GraphEdge checkBackWindows_to_inspectBackAxle = new GraphEdge(checkBackWindows, inspectBackAxle, new GraphData(true));
            GraphEdge inspectBackAxle_to_inspectFluidPuddles = new GraphEdge(inspectBackAxle, inspectFluidPuddles, new GraphData(true));
            GraphEdge inspectFluidPuddles_to_resetCameraAfterPuddle = new GraphEdge(inspectFluidPuddles, resetCameraAfterPuddle, new GraphData(true));
            GraphEdge resetCameraAfterPuddle_to_checkSwingLinkageDebris = new GraphEdge(resetCameraAfterPuddle, checkSwingLinkageDebris, new GraphData(true));
            GraphEdge checkSwingLinkageDebris_to_checkSwingLinkageHydraulic = new GraphEdge(checkSwingLinkageDebris, checkSwingLinkageHydraulic, new GraphData(true));
            GraphEdge checkSwingLinkageHydraulic_to_checkSwingLinkagePinsAndRetainers = new GraphEdge(checkSwingLinkageHydraulic, checkSwingLinkagePinsAndRetainers, new GraphData(true));
            GraphEdge checkSwingLinkagePinsAndRetainers_to_resetSwingLinkageCamera = new GraphEdge(checkSwingLinkagePinsAndRetainers, resetSwingLinkageCamera, new GraphData(4));
            GraphEdge resetSwingLinkageCamera_to_disengageSwingLockoutPin = new GraphEdge(resetSwingLinkageCamera, disengageSwingLockoutPin, new GraphData(0));
            GraphEdge disengageSwingLockoutPin_to_resetAfterSwingLockoutPin = new GraphEdge(disengageSwingLockoutPin, resetAfterSwingLockoutPin, new GraphData(false));
            GraphEdge resetAfterSwingLockoutPin_to_checkPistons = new GraphEdge(resetAfterSwingLockoutPin, checkPistons, new GraphData(true));

            GraphEdge checkPistons_to_checkLines = new GraphEdge(checkPistons, checkLines, new GraphData(true));
            GraphEdge checkLines_to_checkBracketsLeft = new GraphEdge(checkLines, checkBracketsLeft, new GraphData(true));
            GraphEdge checkBracketsLeft_to_checkBucketLinkageDebris = new GraphEdge(checkBracketsLeft, checkBucketLinkageDebris, new GraphData(true));
            GraphEdge checkBucketLinkageDebris_to_inspectBucketTeeth = new GraphEdge(checkBucketLinkageDebris, inspectBucketTeeth, new GraphData(true));
            GraphEdge inspectBucketTeeth_to_checkRearBucket = new GraphEdge(inspectBucketTeeth, checkRearBucket, new GraphData(true));
            GraphEdge checkRearBucket_to_checkRearBucketPinsAndRetainers = new GraphEdge(checkRearBucket, checkRearBucketPinsAndRetainers, new GraphData(true));
            GraphEdge checkRearBucketPinsAndRetainers_to_checkRearBucketHydraulic = new GraphEdge(checkRearBucketPinsAndRetainers, checkRearBucketHydraulic, new GraphData(5));
            GraphEdge checkRearBucketHydraulic_to_checkRearBoomRightSide = new GraphEdge(checkRearBucketHydraulic, checkRightBrackets, new GraphData(true));

            GraphEdge checkRearBoomRightSide_to_checkRightDoor = new GraphEdge(checkRightBrackets, end, new GraphData(true));

            graph.AddVertices(
                checkBackWindows,
                inspectBackAxle,
                inspectFluidPuddles,
                resetCameraAfterPuddle,
                checkSwingLinkageDebris,
                checkSwingLinkageHydraulic,
                checkSwingLinkagePinsAndRetainers,
                resetSwingLinkageCamera,
                disengageSwingLockoutPin,
                resetAfterSwingLockoutPin,
                checkPistons,
                checkLines,
                checkBracketsLeft,
                checkBucketLinkageDebris,
                inspectBucketTeeth,
                checkRearBucket,
                checkRearBucketPinsAndRetainers,
                checkRearBucketHydraulic,
                checkRightBrackets,
                end
            );

            graph.AddEdges(
                checkBackWindows_to_inspectBackAxle,
                inspectBackAxle_to_inspectFluidPuddles,
                inspectFluidPuddles_to_resetCameraAfterPuddle,
                resetCameraAfterPuddle_to_checkSwingLinkageDebris,
                checkSwingLinkageDebris_to_checkSwingLinkageHydraulic,
                checkSwingLinkageHydraulic_to_checkSwingLinkagePinsAndRetainers,
                checkSwingLinkagePinsAndRetainers_to_resetSwingLinkageCamera,
                resetSwingLinkageCamera_to_disengageSwingLockoutPin,
                disengageSwingLockoutPin_to_resetAfterSwingLockoutPin,
                resetAfterSwingLockoutPin_to_checkPistons,
                checkPistons_to_checkLines,
                checkLines_to_checkBracketsLeft,
                checkBracketsLeft_to_checkBucketLinkageDebris,
                checkBucketLinkageDebris_to_inspectBucketTeeth,
                inspectBucketTeeth_to_checkRearBucket,
                checkRearBucket_to_checkRearBucketPinsAndRetainers,
                checkRearBucketPinsAndRetainers_to_checkRearBucketHydraulic,
                checkRearBucketHydraulic_to_checkRearBoomRightSide,
                checkRearBoomRightSide_to_checkRightDoor
            );

            return (checkBackWindows, end);
        }
    }
}


