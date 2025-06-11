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
        public (GraphVertex start, GraphVertex end) BuildOldExteriorInspectionScenario(CoreGraph graph)
        {
            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();
            //BuildFrontLoaderScenario
            //BuildROPSScenario
            //BuildLiftArmScenario
            //BuildTiresAndSteeringScenario

            #region LeftSideCheck
            //Left Door, Windows, Wheels
            UserTaskVertex checkLeftDoor = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, backhoe.CabDoors[0].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckLeftDoorFrame"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckLeftDoorFrame"),
            };

            UserTaskVertex checkLeftWindows = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.LeftDoorAndWindows.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWheelsAndWindows"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWheelsAndWindows")
            };

            UserTaskVertex checkBackLeftWheel = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackLeftTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackLeftTire"),
            };

            #endregion

            #region BackLeftCheck
            //Back Left Light, Back Left Pins, Pistons, Grousers
            UserTaskVertex checkBackLeftLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[3].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackLeftIndicator"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackLeftIndicator")
            };

            UserTaskVertex checkBackLeftPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckLeftGrouserPins"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckLeftGrouserPins")
            };

            UserTaskVertex checkBackLeftPistons = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Pistons[6].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckLeftStabilizer"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckLeftStabilizer")
            };

            UserTaskVertex checkBackLeftGrousers = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.PadGrousers[0].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckLeftGrouser"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckLeftGrouser")
            };

            #endregion

            #region BackRightCheck
            //Back Right Light, Back Right Pins, Pistons, Grousers
            UserTaskVertex checkBackRightLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[2].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackRightIndicator"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackRightIndicator")
            };


            UserTaskVertex checkBackRightPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightGrouserPins"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightGrouserPins")
            };

            UserTaskVertex checkBackRightPistons = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Pistons[7].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightStabilizer"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightStabilizer")
            };

            UserTaskVertex checkBackRightGrousers = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.PadGrousers[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightGrouser"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightGrouser")
            };

            #endregion

            #region BackOverallCheck
            //Windows, Swing Lockout, Back Axle, Swing Linkage, Rear Boom Bucket, Pins, Pistons, Hydraulics, Brackets
            UserTaskVertex checkBackWindows = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearWindows.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWheelsAndWindows"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWheelsAndWindows")
            };

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

            /*UserTaskVertex checkRearBoomLeftSide = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearBoomLeftSide.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearBoomLeftSide"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearBoomLeftSide")
            };*/

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

            UserTaskVertex checkRearBoomRightSide = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RearBoomRightSide.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearBoomRightSide"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearBoomRightSide")
            };


            #endregion

            #region RightSideCheck
            //Check Right Door, Windows, and Wheels
            UserTaskVertex checkRightDoor = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, backhoe.CabDoors[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightDoorFrame"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightDoorFrame"),
            };

            UserTaskVertex checkRightWindows = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RightDoorAndWindows.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWheelsAndWindows"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWheelsAndWindows")
            };

            UserTaskVertex checkBackRightWheel = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackRightTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackRightTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackRightTire"),
            };

            #endregion

            #region FrontRightCheck
            //Check for Pins, Pistons, Hoses, Wheels
            UserTaskVertex inspectRightPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectLeftPins"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectLeftPins")
            };

            UserTaskVertex inspectRightPiston = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.RightPiston.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLeftPiston"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLeftPiston"),
            };

            UserTaskVertex inspectRightHydraulics = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.RightHydraulics.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLeftHydraulics"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLeftHydraulics"),
            };

            UserTaskVertex frontRightTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontRightTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontLeftTire"),
            };

            #endregion

            #region FrontCheck
            //Check Front Loader Bucket, Front Window, and Front Lights
            UserTaskVertex checkLights = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontLeftIndicator"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontLeftIndicator")
            };

            UserTaskVertex checkFrontWindow = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.Task1_WheelsAndWindows.GroupElements[2])
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWheelsAndWindows"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWheelsAndWindows")
            };

            UserTaskVertex checkDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.InsideLoaderBucket.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage7CheckDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage7CheckDebris")
            };

            UserTaskVertex checkFasteners = new UserTaskVertex(typeof(Bucket), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage6CheckFasteners"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage6CheckFasteners")
            };

            UserTaskVertex checkBucket = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.FrontBucket.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage7CheckFrontLoader"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage7CheckFrontLoader")
            };

            UserTaskVertex checkBehindDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.LoaderBucket.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage8CheckDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage8CheckDebris")
            };

            UserTaskVertex checkHydraulics = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.Stage8Hydraulics.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage8CheckHydraulics"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage8CheckHydraulics")
            };

            UserTaskVertex checkPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckStage8CheckPinsAndRetainers"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage8CheckPinsAndRetainers")
            };

            #endregion

            #region FrontLeftCornerCheck
            //Check for Pins, Pistons, Lines, Wheels
            UserTaskVertex inspectLeftPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectLeftPins"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectLeftPins")
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

            UserTaskVertex frontLeftTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontLeftTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontLeftTire"),
            };
            #endregion

            #region overallChecks
            //Check for ROPS, and Lift Arm, and Steering Linkage
            UserTaskVertex inspectLiftArm = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.LiftArm.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLiftArm"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLiftArm"),
            };

            UserTaskVertex inspectROPS = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.ROPS.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleROPS"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionROPS"),
            };

            UserTaskVertex frontAxleInspectionCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontAxle.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleFrontAxle"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionFrontAxle"),
            };

            #endregion


            checkLeftDoor.OnEnterVertex += (_) => {
                module.InspectableManager.AddActiveElement(backhoe.CabDoors[0].InspectableElement);
                backhoe.CabDoors[0].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LeftDoor));
            };

            checkLeftDoor.OnLeaveVertex += (_) => {
                module.InspectableManager.RemoveActiveElement(backhoe.CabDoors[0].InspectableElement);
                backhoe.CabDoors[0].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            };

            checkLeftWindows.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[5]);
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[7]);
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[1]);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(false);
                leanWindow.TurnOn();
            };

            checkLeftWindows.OnLeaveVertex += (_) =>
            {
                if(backhoe.Windows[3].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[3].SetClean();

                if (backhoe.Windows[5].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[5].SetClean();

                if (backhoe.Windows[7].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[7].SetClean();

                backhoe.Windows[3].WindowsCleanedToast();
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(true);
            };

            checkBackLeftWheel.OnEnterVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().SetupTarget(3);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(3, 1f);
                module.InspectableManager.UpdateActiveElements(sceneData.BackLeftTire.GroupElements);
                sceneData.BackLeftTire.ToggleGroupInteraction(false);
                backhoe.InteractManager.SetWheelNuts(false);
            };

            checkBackLeftWheel.OnLeaveVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                backhoe.InteractManager.SetWheelNuts(true);
            };

            checkBackLeftLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[3].InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomLeftSide));

            };
            checkBackLeftLight.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);

            checkBackLeftPins.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 1f);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly));
            };

            checkBackLeftPins.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);

            checkBackLeftPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            checkBackLeftPistons.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.Pistons[6]);
                module.InspectableManager.AddActiveElement(backhoe.Pistons[6].InspectableElement);
            };

            checkBackLeftPistons.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.Pistons[6]);
                module.InspectableManager.RemoveActiveElement(backhoe.Pistons[6].InspectableElement);
            };

            checkBackLeftGrousers.OnEnterVertex += (_) => {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 1f);
                module.InspectableManager.AddActiveElement(backhoe.PadGrousers[0].InspectableElement);
                backhoe.PadGrousers[0].GetComponent<Interactable>().ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
            };

            checkBackLeftGrousers.OnLeaveVertex += (_) => {
                backhoe.InteractManager.SetGrousers(true);
                module.InspectableManager.RemoveActiveElement(backhoe.PadGrousers[0].InspectableElement);
                backhoe.PadGrousers[0].GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            };

            checkBackRightLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[2].InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomRightSide));
            };

            checkBackRightLight.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);

            checkBackRightPins.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(2, 1f);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly));
                backhoe.InteractManager.SetPins(false);
            };

            checkBackRightPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            checkBackRightPistons.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.Pistons[7]);
                module.InspectableManager.AddActiveElement(backhoe.Pistons[7].InspectableElement);
            };

            checkBackRightPistons.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.Pistons[7]);
                module.InspectableManager.RemoveActiveElement(backhoe.Pistons[7].InspectableElement);
            };

            checkBackRightGrousers.OnEnterVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().SetupTarget(3);
                module.InspectableManager.AddActiveElement(backhoe.PadGrousers[1].InspectableElement);
                backhoe.InteractManager.SetGrousers(false);
            };

            checkBackRightGrousers.OnLeaveVertex += (_) => {
                backhoe.InteractManager.SetGrousers(true);
                module.InspectableManager.RemoveActiveElement(backhoe.PadGrousers[1].InspectableElement);
            };

            //Need to do all windows
            checkBackWindows.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(sceneData.RearWindows.GroupElements[0]);
                module.InspectableManager.AddActiveElement(sceneData.RearWindows.GroupElements[1]);
                sceneData.RearWindows.ToggleGroupInteraction(false);
                leanWindow.TurnOn();
            };

            checkBackWindows.OnLeaveVertex += (_) =>
            {
                if (backhoe.Windows[0].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[0].SetClean();

                if (backhoe.Windows[1].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[1].SetClean();

                backhoe.Windows[0].WindowsCleanedToast();
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearWindows.ToggleGroupInteraction(true);
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
                leanWindow.TurnOn();
            };

            checkSwingLinkageHydraulic.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Stage14Hydraulics.ToggleGroupInteraction(true);
                leanWindow.TurnOff();
            };

            resetSwingLinkageCamera.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp).GetComponent<DollyWithTargets>().MoveToTarget(0, 3f);
            resetSwingLinkageCamera.OnLeaveVertex += (_) => StageCompleted(Stage.SwingLinkage);

            checkSwingLinkagePinsAndRetainers.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);
            checkSwingLinkagePinsAndRetainers.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            /*checkRearBoomLeftSide.OnEnterVertex += (_) =>
            {

                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomLeftSide));
                module.InspectableManager.UpdateActiveElements(sceneData.RearBoomLeftSide.GroupElements);
                sceneData.RearBoomLeftSide.ToggleGroupInteraction(false);
                leanWindow.TurnOn();
            };

            checkRearBoomLeftSide.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearBoomLeftSide.ToggleGroupInteraction(true);
                leanWindow.TurnOff();
            };*/

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

            checkRearBoomRightSide.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomRightSide));
                module.InspectableManager.UpdateActiveElements(sceneData.RearBoomRightSide.GroupElements);
                sceneData.RearBoomRightSide.ToggleGroupInteraction(false);
            };

            checkRearBoomRightSide.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.RearBoomRightSide.ToggleGroupInteraction(true);
                StageCompleted(Stage.RearBoom);
            };

            checkRearBucket.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.BoomBucket.InspectableElement);
                backhoe.InteractManager.SetBoomBucketTeeth(true);
            };

            checkRearBucket.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.RemoveActiveElement(backhoe.BoomBucket.InspectableElement);
                backhoe.InteractManager.SetBoomBucketTeeth(false);
            };

            checkRightDoor.OnEnterVertex += (_) => {
                module.InspectableManager.AddActiveElement(backhoe.CabDoors[1].InspectableElement);
                backhoe.CabDoors[1].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RightDoor));
            };

            checkRightDoor.OnLeaveVertex += (_) => {
                module.InspectableManager.RemoveActiveElement(backhoe.CabDoors[1].InspectableElement);
                backhoe.CabDoors[1].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            };

            //Need to do all windows
            checkRightWindows.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[0]);
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[8]);
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[9]);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(false);
                leanWindow.TurnOn();
            };

            checkRightWindows.OnLeaveVertex += (_) =>
            {
                if (backhoe.Windows[4].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[4].SetClean();

                if (backhoe.Windows[6].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[6].SetClean();

                if (backhoe.Windows[8].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[8].SetClean();

                backhoe.Windows[3].WindowsCleanedToast();
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(true);
            };

            checkBackRightWheel.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().SetupTarget(2);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(2, 1f);
                module.InspectableManager.UpdateActiveElements(sceneData.BackRightTire.GroupElements);
                sceneData.BackRightTire.ToggleGroupInteraction(false);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 2f);
            };

            checkBackRightWheel.OnLeaveVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.BackRightTire.ToggleGroupInteraction(true);
            };

            inspectRightPins.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetPins(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly));
            };

            inspectRightPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            inspectRightPiston.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.RightPiston.GroupElements);
                sceneData.RightPiston.ToggleGroupInteraction(false);
            };

            inspectRightPiston.OnLeaveVertex += (_) =>
            {
                sceneData.RightPiston.ToggleGroupInteraction(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            inspectRightHydraulics.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.RightHydraulics.GroupElements);
                sceneData.RightHydraulics.ToggleGroupInteraction(false);
            };

            inspectRightHydraulics.OnLeaveVertex += (_) =>
            {
                sceneData.RightHydraulics.ToggleGroupInteraction(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            frontRightTireCheck.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.FrontRightTire.GroupElements);
                sceneData.FrontRightTire.ToggleGroupInteraction(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 1f);
                backhoe.InteractManager.SetWheelNuts(false);
            };

            frontRightTireCheck.OnLeaveVertex += (_) =>
            {
                sceneData.FrontRightTire.ToggleGroupInteraction(true);
                backhoe.InteractManager.SetWheelNuts(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkLights.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[1].InspectableElement); 
                module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[0].InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Bucket));
            };

            checkLights.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);

            //Need to do all windows
            checkFrontWindow.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[2]);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(false);
                leanWindow.TurnOn();
            };

            checkFrontWindow.OnLeaveVertex += (_) =>
            {
                if (backhoe.Windows[2].WState == CabWindow.WindowStates.Dirty)
                    backhoe.Windows[2].SetClean();

                backhoe.Windows[0].WindowsCleanedToast();
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(true);
            };

            checkDebris.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetDebris(true);
            };

            checkDebris.OnLeaveVertex += (_) => backhoe.InteractManager.SetDebris(false);

            checkFasteners.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.BucketCloseup));
                backhoe.InteractManager.SetBucketFasteners(false);
            };

            checkBucket.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetBucketFasteners(true);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Bucket));
                module.InspectableManager.UpdateActiveElements(sceneData.FrontBucket.GroupElements);
            };

            checkBucket.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkBehindDebris.OnEnterVertex += (_) =>
            {
                //Spawn and remove debris
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LoaderBucketBehind));
                backhoe.InteractManager.SetDebris(true);
            };

            checkHydraulics.OnEnterVertex += (_) =>
            {
                leanWindow.TurnOn();
                backhoe.InteractManager.SetDebris(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LoaderBucketBehind2));
                module.InspectableManager.UpdateActiveElements(sceneData.Stage8Hydraulics.GroupElements);
                sceneData.Stage8Hydraulics.ToggleGroupInteraction(false);
            };

            checkHydraulics.OnLeaveVertex += (_) =>
            {
                leanWindow.TurnOff();
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Stage8Hydraulics.ToggleGroupInteraction(true);
            };

            checkPins.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.BackOfBucket));
                backhoe.InteractManager.SetPins(false);
            };

            checkPins.OnLeaveVertex += (_) =>
            {
                StageCompleted(Stage.FrontLoader);
                backhoe.InteractManager.SetPins(true);
            };

            inspectLeftPins.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetPins(false);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().SetupTarget(0);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly));
            };

            inspectLeftPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            inspectLeftPiston.OnEnterVertex += (_) =>
            {
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

            frontLeftTireCheck.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.FrontLeftTire.GroupElements);
                sceneData.FrontLeftTire.ToggleGroupInteraction(false);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 2f);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                backhoe.InteractManager.SetWheelNuts(false);
            };

            frontLeftTireCheck.OnLeaveVertex += (_) =>
            {
                sceneData.FrontLeftTire.ToggleGroupInteraction(true);
                backhoe.InteractManager.SetWheelNuts(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

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
            
            inspectROPS.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LeftDoor));
                sceneData.RightDoorAndWindows.ToggleGroupInteraction(true);
                ToggleInteractability(true, backhoe.ROPS.GetComponent<Interactable>());
                module.InspectableManager.UpdateActiveElements(sceneData.ROPS.GroupElements);
            };

            inspectROPS.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.ROPS.GetComponent<Interactable>());
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                //StageCompleted(Stage.LightsAndROPS);
            };

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

            GraphEdge start_to_checkLeftDoor = new GraphEdge(start, checkLeftDoor, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge checkLeftDoor_to_checkLeftWindows = new GraphEdge(checkLeftDoor, checkLeftWindows, new GraphData(true));
            GraphEdge checkLeftWindows_to_checkBackLeftWheel = new GraphEdge(checkLeftWindows, checkBackLeftWheel, new GraphData(true));
            GraphEdge checkBackLeftWheel_to_checkBackLeftLight = new GraphEdge(checkBackLeftWheel, checkBackLeftLight, new GraphData(true));
            GraphEdge checkBackLeftLight_to_checkBackLeftPins = new GraphEdge(checkBackLeftLight, checkBackLeftPins, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge checkBackLeftPins_to_checkBackLeftPistons = new GraphEdge(checkBackLeftPins, checkBackLeftPistons, new GraphData(7));
            GraphEdge checkBackLeftPistons_to_checkBackLeftGrousers = new GraphEdge(checkBackLeftPistons, checkBackLeftGrousers, new GraphData(true));
            GraphEdge checkBackLeftGrousers_to_checkBackRightLight = new GraphEdge(checkBackLeftGrousers, checkBackRightLight, new GraphData(true));
            GraphEdge checkBackRightLight_to_checkBackRightPins = new GraphEdge(checkBackRightLight, checkBackRightPins, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge checkBackRightPins_to_checkBackRightPistons = new GraphEdge(checkBackRightPins, checkBackRightPistons, new GraphData(6));
            GraphEdge checkBackRightPistons_to_checkBackRightGrousers = new GraphEdge(checkBackRightPistons, checkBackRightGrousers, new GraphData(true));
            GraphEdge checkBackRightGrousers_to_checkBackWindows = new GraphEdge(checkBackRightGrousers, checkBackWindows, new GraphData(true));

            GraphEdge checkBackWindows_to_checkSwingLinkageDebris = new GraphEdge(checkBackWindows, checkSwingLinkageDebris, new GraphData(true));
            GraphEdge checkSwingLinkageDebris_to_checkSwingLinkageHydraulic = new GraphEdge(checkSwingLinkageDebris, checkSwingLinkageHydraulic, new GraphData(true));
            GraphEdge checkSwingLinkageHydraulic_to_checkSwingLinkagePinsAndRetainers = new GraphEdge(checkSwingLinkageHydraulic, checkSwingLinkagePinsAndRetainers, new GraphData(true));
            GraphEdge checkSwingLinkagePinsAndRetainers_to_resetSwingLinkageCamera = new GraphEdge(checkSwingLinkagePinsAndRetainers, resetSwingLinkageCamera, new GraphData(4));
            //GraphEdge resetSwingLinkageCamera_to_checkRearBoomLeftSide = new GraphEdge(resetSwingLinkageCamera, checkRearBoomLeftSide, new GraphData(0));

            //GraphEdge checkRearBoomLeftSide_to_checkBucketLinkageDebris = new GraphEdge(checkRearBoomLeftSide, checkBucketLinkageDebris, new GraphData(true));
            GraphEdge checkBucketLinkageDebris_to_checkRearBucket = new GraphEdge(checkBucketLinkageDebris, checkRearBucket, new GraphData(true));
            GraphEdge checkRearBucket_to_checkRearBucketPinsAndRetainers = new GraphEdge(checkRearBucket, checkRearBucketPinsAndRetainers, new GraphData(true));
            GraphEdge checkRearBucketPinsAndRetainers_to_checkRearBucketHydraulic = new GraphEdge(checkRearBucketPinsAndRetainers, checkRearBucketHydraulic, new GraphData(5));
            GraphEdge checkRearBucketHydraulic_to_checkRearBoomRightSide = new GraphEdge(checkRearBucketHydraulic, checkRearBoomRightSide, new GraphData(true));

            GraphEdge checkRearBoomRightSide_to_checkRightDoor = new GraphEdge(checkRearBoomRightSide, checkRightDoor, new GraphData(true));
            GraphEdge checkRightDoor_to_checkRightWindows = new GraphEdge(checkRightDoor, checkRightWindows, new GraphData(true));
            GraphEdge checkRightWindows_to_checkBackRightWheel = new GraphEdge(checkRightWindows, checkBackRightWheel, new GraphData(true));
            GraphEdge checkBackRightWheel_to_inspectRightPins = new GraphEdge(checkBackRightWheel, inspectRightPins, new GraphData(true));
            GraphEdge inspectRightPins_to_inspectRightPiston = new GraphEdge(inspectRightPins, inspectRightPiston, new GraphData(2));
            GraphEdge inspectRightPiston_to_inspectRightHydraulics = new GraphEdge(inspectRightPiston, inspectRightHydraulics, new GraphData(true));
            GraphEdge inspectRightHydraulics_to_frontRightTireCheck = new GraphEdge(inspectRightHydraulics, frontRightTireCheck, new GraphData(true));
            GraphEdge frontRightTireCheck_to_checkLights = new GraphEdge(frontRightTireCheck, checkLights, new GraphData(true));


            GraphEdge checkLights_to_checkFrontWindow = new GraphEdge(checkLights, checkFrontWindow, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge checkFrontWindow_to_checkFasteners = new GraphEdge(checkFrontWindow, checkFasteners, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge checkFasteners_to_checkDebris = new GraphEdge(checkFasteners, checkDebris, new GraphData(true));
            GraphEdge checkDebris_to_checkBucket = new GraphEdge(checkDebris, checkBucket, new GraphData(true));
            GraphEdge checkBucket_to_checkBehindDebris = new GraphEdge(checkBucket, checkBehindDebris, new GraphData(true));
            GraphEdge checkBehindDebris_to_checkHydraulics = new GraphEdge(checkBehindDebris, checkHydraulics, new GraphData(true));
            GraphEdge checkHydraulics_to_checkPins = new GraphEdge(checkHydraulics, checkPins, new GraphData(true));

            GraphEdge checkPins_to_inspectLeftPins = new GraphEdge(checkPins, inspectLeftPins, new GraphData(1));
            GraphEdge inspectLeftPins_to_inspectLeftPiston = new GraphEdge(inspectLeftPins, inspectLeftPiston, new GraphData(3));
            GraphEdge inspectLeftPiston_to_inspectLeftHydraulics = new GraphEdge(inspectLeftPiston, inspectLeftHydraulics, new GraphData(true));
            GraphEdge inspectLeftHydraulics_to_frontLeftTireCheck = new GraphEdge(inspectLeftHydraulics, frontLeftTireCheck, new GraphData(true));
            GraphEdge frontLeftTireCheck_to_inspectLiftArm = new GraphEdge(frontLeftTireCheck, inspectLiftArm, new GraphData(true));

            GraphEdge inspectLiftArm_to_inspectROPS = new GraphEdge(inspectLiftArm, inspectROPS, new GraphData(true));
            GraphEdge inspectROPS_to_inspectFrontAxle = new GraphEdge(inspectROPS, frontAxleInspectionCheck, new GraphData(true));
            GraphEdge inspectFrontAxle_to_end = new GraphEdge(frontAxleInspectionCheck, end, new GraphData(true));

            graph.AddVertices(
                start,
                checkLeftDoor,
                checkLeftWindows,
                checkBackLeftWheel,
                checkBackLeftLight,
                checkBackLeftPins,
                checkBackLeftPistons,
                checkBackLeftGrousers,
                checkBackRightLight,
                checkBackRightPins,
                checkBackRightPistons,
                checkBackRightGrousers,
                checkBackWindows,
                checkSwingLinkageDebris,
                checkSwingLinkageHydraulic,
                checkSwingLinkagePinsAndRetainers,
                resetSwingLinkageCamera,
                //checkRearBoomLeftSide,
                checkBucketLinkageDebris,
                checkRearBucket,
                checkRearBucketPinsAndRetainers,
                checkRearBucketHydraulic,
                checkRearBoomRightSide,
                checkRightDoor,
                checkRightWindows,
                checkBackRightWheel,
                inspectRightPins,
                inspectRightPiston,
                inspectRightHydraulics,
                frontRightTireCheck,
                checkLights,
                checkFrontWindow,
                checkFasteners,
                checkDebris,
                checkBucket,
                checkBehindDebris,
                checkHydraulics,
                checkPins,
                inspectLeftPins, 
                inspectLeftPiston, 
                inspectLeftHydraulics,
                frontLeftTireCheck,
                inspectLiftArm,
                inspectROPS,
                frontAxleInspectionCheck,
                end
            );

            graph.AddEdges(
                start_to_checkLeftDoor,
                checkLeftDoor_to_checkLeftWindows,
                checkLeftWindows_to_checkBackLeftWheel,
                checkBackLeftWheel_to_checkBackLeftLight,
                checkBackLeftLight_to_checkBackLeftPins,
                checkBackLeftPins_to_checkBackLeftPistons,
                checkBackLeftPistons_to_checkBackLeftGrousers,
                checkBackLeftGrousers_to_checkBackRightLight,
                checkBackRightLight_to_checkBackRightPins,
                checkBackRightPins_to_checkBackRightPistons,
                checkBackRightPistons_to_checkBackRightGrousers,
                checkBackRightGrousers_to_checkBackWindows,
                checkBackWindows_to_checkSwingLinkageDebris,
                checkSwingLinkageDebris_to_checkSwingLinkageHydraulic,
                checkSwingLinkageHydraulic_to_checkSwingLinkagePinsAndRetainers,
                checkSwingLinkagePinsAndRetainers_to_resetSwingLinkageCamera,
                //resetSwingLinkageCamera_to_checkRearBoomLeftSide,
                //checkRearBoomLeftSide_to_checkBucketLinkageDebris,
                checkBucketLinkageDebris_to_checkRearBucket,
                checkRearBucket_to_checkRearBucketPinsAndRetainers,
                checkRearBucketPinsAndRetainers_to_checkRearBucketHydraulic,
                checkRearBucketHydraulic_to_checkRearBoomRightSide,
                checkRearBoomRightSide_to_checkRightDoor,
                checkRightDoor_to_checkRightWindows,
                checkRightWindows_to_checkBackRightWheel,
                checkBackRightWheel_to_inspectRightPins,
                inspectRightPins_to_inspectRightPiston,
                inspectRightPiston_to_inspectRightHydraulics,
                inspectRightHydraulics_to_frontRightTireCheck,
                frontRightTireCheck_to_checkLights,
                checkLights_to_checkFrontWindow,
                checkFrontWindow_to_checkFasteners,
                checkFasteners_to_checkDebris,
                checkDebris_to_checkBucket,
                checkBucket_to_checkBehindDebris,
                checkBehindDebris_to_checkHydraulics,
                checkHydraulics_to_checkPins,
                checkPins_to_inspectLeftPins,
                inspectLeftPins_to_inspectLeftPiston,
                inspectLeftPiston_to_inspectLeftHydraulics,
                inspectLeftHydraulics_to_frontLeftTireCheck,
                frontLeftTireCheck_to_inspectLiftArm,
                inspectLiftArm_to_inspectROPS,
                inspectROPS_to_inspectFrontAxle,
                inspectFrontAxle_to_end
            );

            return (start, end);
        }
    }
}

