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
        public (GraphVertex start, GraphVertex end) BuildFrontCheck(CoreGraph graph)
        {
            GraphVertex end = new GraphVertex();

            UserTaskVertex checkLights = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Lights[4].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectTopLight"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectTopLight")
            };

            UserTaskVertex checkFrontLeftLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Lights[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectFrontLeftLights"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectFrontLeftLights")
            };

            UserTaskVertex checkFrontLeftIndicatorLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[1].InspectableElement)
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

            checkLights.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.Lights[4].InspectableElement);
                backhoe.Lights[4].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.TopLightPOI));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.TopLightPOI).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 20f;
            };

            checkLights.OnLeaveVertex += (_) =>
            {
                backhoe.Lights[4].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkFrontLeftLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.Lights[1].InspectableElement);
                backhoe.Lights[1].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.FrontLeftLightsPOI));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.FrontLeftLightsPOI).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 40f;
            };

            checkFrontLeftLight.OnLeaveVertex += (_) =>
            {
                backhoe.Lights[1].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkFrontLeftIndicatorLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[1].InspectableElement);
                backhoe.IndicatorLights[1].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
            };

            checkFrontLeftIndicatorLight.OnLeaveVertex += (_) =>
            {
                backhoe.IndicatorLights[1].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            //Need to do all windows
            checkFrontWindow.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.TopLightPOI));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.TopLightPOI).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 40f;
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[2]);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(false);
                sceneData.Task1_WheelsAndWindows.GroupElements[2].GetComponent<CabWindow>().RecallDirty();
            };

            checkFrontWindow.OnLeaveVertex += (_) =>
            {
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
                backhoe.InteractManager.SetDebris(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LoaderBucketBehind2));
                module.InspectableManager.UpdateActiveElements(sceneData.Stage8Hydraulics.GroupElements);
                sceneData.Stage8Hydraulics.ToggleGroupInteraction(false);
            };

            checkHydraulics.OnLeaveVertex += (_) =>
            {
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

            GraphEdge checkLights_to_checkFrontLeftLight = new GraphEdge(checkLights, checkFrontLeftLight, new GraphData(true));
            GraphEdge checkFrontLeftLight_to_checkFrontLeftIndicatorLight = new GraphEdge(checkFrontLeftLight, checkFrontLeftIndicatorLight, new GraphData(true));
            GraphEdge checkFrontLeftIndicatorLight_to_checkFrontWindow = new GraphEdge(checkFrontLeftIndicatorLight, checkFrontWindow, new GraphData(true));
            GraphEdge checkFrontWindow_to_checkFasteners = new GraphEdge(checkFrontWindow, checkFasteners, new GraphData(true));
            GraphEdge checkFasteners_to_checkDebris = new GraphEdge(checkFasteners, checkDebris, new GraphData(true));
            GraphEdge checkDebris_to_checkBucket = new GraphEdge(checkDebris, checkBucket, new GraphData(true));
            GraphEdge checkBucket_to_checkBehindDebris = new GraphEdge(checkBucket, checkBehindDebris, new GraphData(true));
            GraphEdge checkBehindDebris_to_checkHydraulics = new GraphEdge(checkBehindDebris, checkHydraulics, new GraphData(true));
            GraphEdge checkHydraulics_to_checkPins = new GraphEdge(checkHydraulics, checkPins, new GraphData(true));

            GraphEdge checkPins_to_inspectLeftPins = new GraphEdge(checkPins, end, new GraphData(1));

            graph.AddVertices(
                checkLights,
                checkFrontLeftLight,
                checkFrontLeftIndicatorLight,
                checkFrontWindow,
                checkFasteners,
                checkDebris,
                checkBucket,
                checkBehindDebris,
                checkHydraulics,
                checkPins,
                end
            );

            graph.AddEdges(
                checkLights_to_checkFrontLeftLight,
                checkFrontLeftLight_to_checkFrontLeftIndicatorLight,
                checkFrontLeftIndicatorLight_to_checkFrontWindow,
                checkFrontWindow_to_checkFasteners,
                checkFasteners_to_checkDebris,
                checkDebris_to_checkBucket,
                checkBucket_to_checkBehindDebris,
                checkBehindDebris_to_checkHydraulics,
                checkHydraulics_to_checkPins,
                checkPins_to_inspectLeftPins
            );

            return (checkLights, end);
        }
    }
}


