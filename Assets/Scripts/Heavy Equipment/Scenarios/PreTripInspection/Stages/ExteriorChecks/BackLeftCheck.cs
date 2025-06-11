using Lean.Gui;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using RemoteEducation.Scenarios;
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
        public (GraphVertex start, GraphVertex end) BuildBackLeftCheck(CoreGraph graph)
        {

            GraphVertex end = new GraphVertex();

            UserTaskVertex checkBehindDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.PadAndGrouser.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage8CheckDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage8CheckDebris")
            };

            UserTaskVertex checkBackLeftLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Lights[3].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectBackLeftLights"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectBackLeftLights")
            };

            UserTaskVertex checkBackLeftIndicatorLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[3].InspectableElement)
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

            UserTaskVertex resetCamera = CreateWaitTask("resetCamera", 2f);

            checkBehindDebris.OnEnterVertex += (_) =>
            {
                backhoe.EngineSound.ChangeEngineSoundPosition(EngineSoundController.Position.Back);

                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDebris));
                backhoe.InteractManager.SetDebris(true);
            };

            checkBehindDebris.OnLeaveVertex += (_) => {
                backhoe.InteractManager.SetDebris(false);
            };

            checkBackLeftLight.OnEnterVertex += (_) =>
            {
                backhoe.Lights[3].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.AddActiveElement(backhoe.Lights[3].InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearLeftLightsPOI));

            };
            checkBackLeftLight.OnLeaveVertex += (_) =>
            {
                backhoe.Lights[3].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };
            checkBackLeftIndicatorLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[3].InspectableElement);
                backhoe.IndicatorLights[3].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
            };

            checkBackLeftIndicatorLight.OnLeaveVertex += (_) =>
            {
                backhoe.IndicatorLights[3].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkBackLeftPins.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 1f);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 60f;
            };

            checkBackLeftPins.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);

            checkBackLeftPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            checkBackLeftPistons.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.Pistons[6]);
                module.InspectableManager.AddActiveElement(backhoe.Pistons[6].InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserZoomLeft));
            };

            checkBackLeftPistons.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.Pistons[6]);
                module.InspectableManager.RemoveActiveElement(backhoe.Pistons[6].InspectableElement);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 60f;
            };

            checkBackLeftGrousers.OnEnterVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 1f);
                module.InspectableManager.AddActiveElement(backhoe.PadGrousers[0].InspectableElement);
                backhoe.PadGrousers[0].GetComponent<Interactable>().ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
            };

            checkBackLeftGrousers.OnLeaveVertex += (_) => {
                backhoe.InteractManager.SetGrousers(true);
                module.InspectableManager.RemoveActiveElement(backhoe.PadGrousers[0].InspectableElement);
                backhoe.PadGrousers[0].GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            };

            resetCamera.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomFrontSide));
            };

            GraphEdge checkBehindDebris_to_checkBackLeftLight = new GraphEdge(checkBehindDebris, checkBackLeftLight, new GraphData(true));
            GraphEdge checkBackLeftLight_to_checkBackLeftIndicatorLight = new GraphEdge(checkBackLeftLight, checkBackLeftIndicatorLight, new GraphData(true));
            GraphEdge checkBackLeftIndicatorLight_to_checkBackLeftPins = new GraphEdge(checkBackLeftIndicatorLight, checkBackLeftPins, new GraphData(true));
            GraphEdge checkBackLeftPins_to_checkBackLeftPistons = new GraphEdge(checkBackLeftPins, checkBackLeftPistons, new GraphData(7));
            GraphEdge checkBackLeftPistons_to_checkBackLeftGrousers = new GraphEdge(checkBackLeftPistons, checkBackLeftGrousers, new GraphData(true));
            GraphEdge checkBackLeftGrousers_to_resetCamera = new GraphEdge(checkBackLeftGrousers, resetCamera, new GraphData(true));
            GraphEdge resetCamera_to_end = new GraphEdge(resetCamera, end, new GraphData(true));

            graph.AddVertices(
                checkBehindDebris,
                checkBackLeftLight,
                checkBackLeftIndicatorLight,
                checkBackLeftPins,
                checkBackLeftPistons,
                checkBackLeftGrousers,
                resetCamera,
                end
            );

            graph.AddEdges(
                checkBehindDebris_to_checkBackLeftLight,
                checkBackLeftLight_to_checkBackLeftIndicatorLight,
                checkBackLeftIndicatorLight_to_checkBackLeftPins,
                checkBackLeftPins_to_checkBackLeftPistons,
                checkBackLeftPistons_to_checkBackLeftGrousers,
                checkBackLeftGrousers_to_resetCamera,
                resetCamera_to_end
            );

            return (checkBehindDebris, end);
        }
    }
}


