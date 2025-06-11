using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Grouser checks part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildGrouserCheckScenario(CoreGraph graph)
        {
            UserTaskVertex stage12Start = CreateWaitTask("stage12Start", timeBetweenVerts);
            stage12Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage12Start");
            stage12Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage12Start");
            // Stage 12 End

            UserTaskVertex backLeftTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackLeftTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackLeftTire"),
            };
            UserTaskVertex backRightTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackRightTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackRightTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackRightTire"),
            };
            UserTaskVertex rearAxleCheckResetCamera = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.RearAxle.ToString()
            };

            backRightTireCheck.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().SetupTarget(2);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(2, 1f);
                module.InspectableManager.UpdateActiveElements(sceneData.BackRightTire.GroupElements);
                sceneData.BackRightTire.ToggleGroupInteraction(false);
            };

            backLeftTireCheck.OnEnterVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().SetupTarget(3);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(3, 1f);
                module.InspectableManager.UpdateActiveElements(sceneData.BackLeftTire.GroupElements);
                sceneData.BackLeftTire.ToggleGroupInteraction(false);
            };

            backLeftTireCheck.OnLeaveVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            backRightTireCheck.OnLeaveVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            rearAxleCheckResetCamera.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.UnderRearAxle).GetComponent<DollyWithTargets>().MoveToTarget(2, 2f);
            // inspect back axle
            UserTaskVertex stage12CheckRearAxle = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Axles[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearAxle"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearAxle")
            };

            ///Vertices
            GraphVertex stage13Start = new GraphVertex();
            GraphVertex stage13End = new GraphVertex();

            // Remove Debris
            UserTaskVertex stage13RemoveDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.PadAndGrouser.ToString(),

                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckGrouserDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckGrouserDebris")
            };

            UserTaskVertex disengageSwingLockoutPin = new UserTaskVertex(typeof(SwingLockoutPin), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleDisengageSwingLockoutPin"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionDisengageSwingLockoutPin")
            };

            // Check Left Side
            UserTaskVertex checkLeftGrouser = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.PadGrousers[0].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckLeftGrouser"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckLeftGrouser")
            };

            UserTaskVertex stage13CheckLeftPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckLeftGrouserPins"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckLeftGrouserPins")
            };

            UserTaskVertex stage13CheckLeftCylinder = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Pistons[6].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckLeftStabilizer"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckLeftStabilizer")
            };

            // Check Right Side
            UserTaskVertex checkRightGrouser = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.PadGrousers[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightGrouser"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightGrouser")
            };

            UserTaskVertex stage13CheckRightPins = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightGrouserPins"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightGrouserPins")
            };

            UserTaskVertex stage13CheckRightCylinder = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Pistons[7].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightStabilizer"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightStabilizer")
            };

            // Transitions
            stage12CheckRearAxle.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.UnderRearAxle));
                module.InspectableManager.AddActiveElement(backhoe.Axles[1].InspectableElement);
                backhoe.InteractManager.SetAxles(false);
            };

            disengageSwingLockoutPin.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.SwingLockoutPin);
            };

            disengageSwingLockoutPin.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.SwingLockoutPin);
            };

            stage12CheckRearAxle.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetAxles(true);
                module.InspectableManager.RemoveActiveElement(backhoe.Axles[1].InspectableElement);
            };

            stage13RemoveDebris.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDebris));
                backhoe.InteractManager.SetDebris(true);
            };

            stage13RemoveDebris.OnLeaveVertex += (_) => {
                backhoe.InteractManager.SetDebris(false);
            };

            checkRightGrouser.OnEnterVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().SetupTarget(3);
                module.InspectableManager.AddActiveElement(backhoe.PadGrousers[1].InspectableElement);
                backhoe.InteractManager.SetGrousers(false);
            };

            checkRightGrouser.OnLeaveVertex += (_) => {
                backhoe.InteractManager.SetGrousers(false);
                module.InspectableManager.RemoveActiveElement(backhoe.PadGrousers[1].InspectableElement);
            };

            stage13CheckLeftPins.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 1f);
            stage13CheckLeftPins.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);

            stage13CheckLeftPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            stage13CheckRightPins.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(2, 1f);
            stage13CheckRightPins.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);

            stage13CheckRightPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            stage13CheckLeftCylinder.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.Pistons[6]);
                module.InspectableManager.AddActiveElement(backhoe.Pistons[6].InspectableElement);
            };

            stage13CheckRightCylinder.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.Pistons[7]);
                module.InspectableManager.AddActiveElement(backhoe.Pistons[7].InspectableElement);
            };

            stage13CheckLeftCylinder.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.Pistons[6]);
                module.InspectableManager.RemoveActiveElement(backhoe.Pistons[6].InspectableElement);
            };

            stage13CheckRightCylinder.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.Pistons[7]);
                module.InspectableManager.RemoveActiveElement(backhoe.Pistons[7].InspectableElement);
            };

            checkLeftGrouser.OnEnterVertex += (_) => {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 1f);
                module.InspectableManager.AddActiveElement(backhoe.PadGrousers[0].InspectableElement);
            };

            checkLeftGrouser.OnLeaveVertex += (_) => {
                backhoe.InteractManager.SetGrousers(true);
                module.InspectableManager.RemoveActiveElement(backhoe.PadGrousers[0].InspectableElement);
            };

            checkRightGrouser.OnLeaveVertex += (_) => StageCompleted(Stage.Grousers);

            // Define Edges

            GraphEdge stage12Start_to_backRightTireCheck = new GraphEdge(stage12Start, backRightTireCheck, new GraphData(true));
            GraphEdge backRightTireCheck_to_stage12CheckRearAxle = new GraphEdge(backRightTireCheck, stage12CheckRearAxle, new GraphData(true));
            GraphEdge stage12CheckRearAxle_to_rearAxleCheckResetCamera = new GraphEdge(stage12CheckRearAxle, rearAxleCheckResetCamera, new GraphData(true));
            GraphEdge rearAxleCheckResetCamera_to_stage13RemoveDebris = new GraphEdge(rearAxleCheckResetCamera, stage13RemoveDebris, new GraphData(2));
            GraphEdge stage13RemoveDebris_to_disengageSwingLockoutPin = new GraphEdge(stage13RemoveDebris, disengageSwingLockoutPin, new GraphData(true));
            GraphEdge disengageSwingLockoutPin_to_checkRightGrouser = new GraphEdge(disengageSwingLockoutPin, checkRightGrouser, new GraphData(false));
            GraphEdge checkRightGrouser_to_stage13CheckRightPins = new GraphEdge(checkRightGrouser, stage13CheckRightPins, new GraphData(true));
            GraphEdge stage13CheckRightPins_to_stage13CheckRightCylinder = new GraphEdge(stage13CheckRightPins, stage13CheckRightCylinder, new GraphData(6));
            GraphEdge stage13CheckRightCylinder_to_stage13CheckLeftPins = new GraphEdge(stage13CheckRightCylinder, stage13CheckLeftPins, new GraphData(true));
            GraphEdge stage13CheckLeftPins_to_stage13CheckLeftCylinder = new GraphEdge(stage13CheckLeftPins, stage13CheckLeftCylinder, new GraphData(7));
            GraphEdge stage13CheckLeftCylinder_to_checkLeftGrouser = new GraphEdge(stage13CheckLeftCylinder, checkLeftGrouser, new GraphData(true));
            GraphEdge checkLeftGrouser_to_backLeftTireCheck = new GraphEdge(checkLeftGrouser, backLeftTireCheck, new GraphData(true));
            GraphEdge backLeftTireCheck_to_stage13End = new GraphEdge(backLeftTireCheck, stage13End, new GraphData(true));

            // Add Vertices and Edges
            graph.AddVertices(
                stage12Start, 
                stage12CheckRearAxle, 
                backLeftTireCheck, 
                backRightTireCheck, 
                rearAxleCheckResetCamera,
                stage13Start,
                stage13RemoveDebris,
                disengageSwingLockoutPin,
                checkLeftGrouser,
                stage13CheckLeftPins,
                stage13CheckLeftCylinder,
                checkRightGrouser,
                stage13CheckRightPins,
                stage13CheckRightCylinder,
                stage13End);

            graph.AddEdges(
                stage12Start_to_backRightTireCheck,
                backRightTireCheck_to_stage12CheckRearAxle,
                stage12CheckRearAxle_to_rearAxleCheckResetCamera,
                rearAxleCheckResetCamera_to_stage13RemoveDebris,
                stage13RemoveDebris_to_disengageSwingLockoutPin,
                disengageSwingLockoutPin_to_checkRightGrouser,
                checkRightGrouser_to_stage13CheckRightPins,
                stage13CheckRightPins_to_stage13CheckRightCylinder,
                stage13CheckRightCylinder_to_stage13CheckLeftPins,
                stage13CheckLeftPins_to_stage13CheckLeftCylinder,
                stage13CheckLeftCylinder_to_checkLeftGrouser,
                checkLeftGrouser_to_backLeftTireCheck,
                backLeftTireCheck_to_stage13End
            );

            return (stage12Start, stage13End);
        }
    }
}

