using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Rear Boom checks part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildRearBoomScenario(CoreGraph graph)
        {
            ///<summary>
            /// * Left side (POI: RearBoomLeftSide)
            ///     - Boom Arm Lower Piston (Boom Arm Bottom Hydraulic)
            ///     - Boom Arm Upper Piston (Boom Arm Top Hydraulic)
            ///     - Rear Arm / Boom Upper Hydraulic Hoses
            ///     - Rear Arm / Boom Lower Hydraulic Hoses
            ///     - Rear Boom Bracket Left Upper
            ///     - Rear Boom Bracket Left Lower
            /// * Rear Bucket linkage area (POI: RearBucketLinkage)
            ///     - Remove any rocks and debris
            ///     - Inspect pins and retainers:
            ///         Bucket Linkage Upper Pins
            ///         Bucket Linkage Lower Pin 1
            ///         Bucket Linkage Lower Pin 2
            ///         Bucket Main Arm Pins
            ///         Bucket Upper Pins
            /// * Right side (POI: RearBoomRightSide)
            ///     - Rear Boom Bracket Right
            ///</ summary >

            // Start
            UserTaskVertex start = CreateWaitTask("RearBoomCheckStart", timeBetweenVerts);
            start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage15Start");
            start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage15Start");

            // Vertices
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

            // Transitions
            /*checkRearBoomLeftSide.OnEnterVertex += (_) =>
            {

                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomLeftSide));
                //module.InspectableManager.UpdateActiveElements(sceneData.RearBoomLeftSide.GroupElements);
                //sceneData.RearBoomLeftSide.ToggleGroupInteraction(false);
                leanWindow.TurnOn();
            };

            checkRearBoomLeftSide.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                //sceneData.RearBoomLeftSide.ToggleGroupInteraction(true);
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

            start.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomRightSide));

            checkRearBoomRightSide.OnEnterVertex += (_) =>
            {
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

            // End
            GraphVertex end = new GraphVertex();

            // Edges
            GraphEdge start_to_checkRearBoomLeftSide = new GraphEdge(start, checkRearBoomRightSide, new GraphData(true));
            GraphEdge checkRearBoomLeftSide_to_checkBucketLinkageDebris = new GraphEdge(checkRearBoomRightSide, checkBucketLinkageDebris, new GraphData(true));
            GraphEdge checkBucketLinkageDebris_to_checkRearBucket = new GraphEdge(checkBucketLinkageDebris, checkRearBucket, new GraphData(true));
            GraphEdge checkRearBucket_to_checkRearBucketPinsAndRetainers = new GraphEdge(checkRearBucket, checkRearBucketPinsAndRetainers, new GraphData(true));
            GraphEdge checkRearBucketPinsAndRetainers_to_checkRearBucketHydraulic = new GraphEdge(checkRearBucketPinsAndRetainers, checkRearBucketHydraulic, new GraphData(5));
            /*GraphEdge checkRearBucketHydraulic_to_checkRearBoomRightSide = new GraphEdge(checkRearBucketHydraulic, checkRearBoomLeftSide, new GraphData(true));
            GraphEdge checkRearBoomRightSide_to_end = new GraphEdge(checkRearBoomLeftSide, end, new GraphData(true));*/

            // Add Vertices and Edges
            graph.AddVertices(
                start,
                //checkRearBoomLeftSide,
                checkBucketLinkageDebris,
                checkRearBucket,
                checkRearBucketPinsAndRetainers,
                checkRearBucketHydraulic,
                checkRearBoomRightSide,
                end);

            graph.AddEdges(
                start_to_checkRearBoomLeftSide,
                checkRearBoomLeftSide_to_checkBucketLinkageDebris,
                checkBucketLinkageDebris_to_checkRearBucket,
                checkRearBucket_to_checkRearBucketPinsAndRetainers,
                checkRearBucketPinsAndRetainers_to_checkRearBucketHydraulic);
                //checkRearBucketHydraulic_to_checkRearBoomRightSide,
                //checkRearBoomRightSide_to_end);

            return (start, end);
        }
    }
}

