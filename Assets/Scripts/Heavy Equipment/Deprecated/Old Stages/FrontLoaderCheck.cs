using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Front Loader check part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildFrontLoaderScenario(CoreGraph graph)
        {
            // Vertices
            UserTaskVertex stage6Start = CreateWaitTask("stage6Start", timeBetweenVerts);
            stage6Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage6Start");
            stage6Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage6Start");

            //Checking fasteners
            UserTaskVertex stage6CheckFasteners = new UserTaskVertex(typeof(Bucket), CheckTypes.Bool)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage6CheckFasteners"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage6CheckFasteners")
            };

            UserTaskVertex stage7CheckDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.InsideLoaderBucket.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage7CheckDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage7CheckDebris")
            };

            UserTaskVertex stage7CheckFrontLoader = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.FrontBucket.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage7CheckFrontLoader"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage7CheckFrontLoader")
            };

            UserTaskVertex stage8CheckDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.LoaderBucket.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage8CheckDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage8CheckDebris")
            };

            UserTaskVertex stage8CheckHydraulics = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.Stage8Hydraulics.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage8CheckHydraulics"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage8CheckHydraulics")
            };

            UserTaskVertex stage8CheckPinsAndRetainers = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckStage8CheckPinsAndRetainers"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage8CheckPinsAndRetainers")
            };

            GraphVertex end = new GraphVertex();

            // Transitions
            stage6Start.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Bucket));
            };

            stage7CheckDebris.OnEnterVertex += (_) => backhoe.InteractManager.SetDebris(true);

            stage7CheckDebris.OnLeaveVertex += (_) => backhoe.InteractManager.SetDebris(false);

            stage6CheckFasteners.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.BucketCloseup));
                backhoe.InteractManager.SetBucketFasteners(false);
            };

            stage7CheckFrontLoader.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetBucketFasteners(true);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Bucket));
                module.InspectableManager.UpdateActiveElements(sceneData.FrontBucket.GroupElements);
            };

            stage7CheckFrontLoader.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            stage8CheckDebris.OnEnterVertex += (_) =>
            {
                //Spawn and remove debris
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LoaderBucketBehind));
                backhoe.InteractManager.SetDebris(true);
            };

            stage8CheckHydraulics.OnEnterVertex += (_) =>
            {
                leanWindow.TurnOn();
                backhoe.InteractManager.SetDebris(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LoaderBucketBehind2));
                module.InspectableManager.UpdateActiveElements(sceneData.Stage8Hydraulics.GroupElements);
                sceneData.Stage8Hydraulics.ToggleGroupInteraction(false);
            };
            stage8CheckHydraulics.OnLeaveVertex += (_) =>
            {
                leanWindow.TurnOff();
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Stage8Hydraulics.ToggleGroupInteraction(true);
            };

            stage8CheckPinsAndRetainers.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.BackOfBucket));
                backhoe.InteractManager.SetPins(false);
            };

            stage8CheckPinsAndRetainers.OnLeaveVertex += (_) =>
            {
                StageCompleted(Stage.FrontLoader);
                backhoe.InteractManager.SetPins(true);
            };

            // Define Edges
            GraphEdge stage6_to_Fasteners = new GraphEdge(stage6Start, stage6CheckFasteners, new GraphData(true));
            GraphEdge stage6CheckFasteners_to_stage7CheckDebris = new GraphEdge(stage6CheckFasteners, stage7CheckDebris, new GraphData(true));
            GraphEdge stage7CheckDebris_to_stage7CheckFrontLoader = new GraphEdge(stage7CheckDebris, stage7CheckFrontLoader, new GraphData(true));
            GraphEdge stage7CheckFrontLoader_to_stage8CheckDebris = new GraphEdge(stage7CheckFrontLoader, stage8CheckDebris, new GraphData(true));
            GraphEdge stage8CheckDebris_to_stage8CheckHydraulics = new GraphEdge(stage8CheckDebris, stage8CheckHydraulics, new GraphData(true));
            GraphEdge stage8CheckHydraulics_to_stage8CheckPinsAndRetainers = new GraphEdge(stage8CheckHydraulics, stage8CheckPinsAndRetainers, new GraphData(true));
            GraphEdge stage8CheckHydraulics_to_end = new GraphEdge(stage8CheckPinsAndRetainers, end, new GraphData(1));

            // Add Vertices and Edges
            graph.AddVertices(
                stage6Start,
                end,
                stage6CheckFasteners,
                stage7CheckDebris,
                stage7CheckFrontLoader,
                stage8CheckDebris,
                stage8CheckHydraulics,
                stage8CheckPinsAndRetainers);

            graph.AddEdges(
                stage8CheckHydraulics_to_end,
                stage6_to_Fasteners,
                stage6CheckFasteners_to_stage7CheckDebris,
                stage7CheckDebris_to_stage7CheckFrontLoader,
                stage7CheckFrontLoader_to_stage8CheckDebris,
                stage8CheckDebris_to_stage8CheckHydraulics,
                stage8CheckHydraulics_to_stage8CheckPinsAndRetainers);

            return (stage6Start, end);
        }
    }
}

