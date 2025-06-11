using Lean.Gui;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Scenarios;
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
        public (GraphVertex start, GraphVertex end) BuildBackRightCheck(CoreGraph graph)
        {

            GraphVertex end = new GraphVertex();

            UserTaskVertex checkBackRightLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Lights[2].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectBackRightLights"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectBackRightLights")
            };

            UserTaskVertex checkBackRightIndicatorLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[2].InspectableElement)
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

            UserTaskVertex resetCamera = CreateWaitTask("resetCamera", 2f);

            checkBackRightLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.Lights[2].InspectableElement);
                backhoe.Lights[2].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomRightSide));
            };

            checkBackRightLight.OnLeaveVertex += (_) =>
            {
                backhoe.Lights[2].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkBackRightIndicatorLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[2].InspectableElement);
                backhoe.IndicatorLights[2].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<DollyWithTargets>().MoveToTarget(2, 1f);
            };

            checkBackRightIndicatorLight.OnLeaveVertex += (_) =>
            {
                backhoe.IndicatorLights[2].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkBackRightPins.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 60f;
                backhoe.InteractManager.SetPins(false);
            };

            checkBackRightPins.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            checkBackRightPistons.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, backhoe.Pistons[7]);
                module.InspectableManager.AddActiveElement(backhoe.Pistons[7].InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserZoomRight));
            };

            checkBackRightPistons.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.Pistons[7]);
                module.InspectableManager.RemoveActiveElement(backhoe.Pistons[7].InspectableElement);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.GrouserDolly).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 60f;
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

            resetCamera.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomFrontSide));
            };

            GraphEdge checkBackRightLight_to_checkBackRightIndicatorLight = new GraphEdge(checkBackRightLight, checkBackRightIndicatorLight, new GraphData(true));
            GraphEdge checkBackRightIndicatorLight_to_checkBackRightPins = new GraphEdge(checkBackRightIndicatorLight, checkBackRightPins, new GraphData(true));
            GraphEdge checkBackRightPins_to_checkBackRightPistons = new GraphEdge(checkBackRightPins, checkBackRightPistons, new GraphData(6));
            GraphEdge checkBackRightPistons_to_checkBackRightGrousers = new GraphEdge(checkBackRightPistons, checkBackRightGrousers, new GraphData(true));
            GraphEdge checkBackRightGrousers_to_resetCamera = new GraphEdge(checkBackRightGrousers, resetCamera, new GraphData(true));
            GraphEdge resetCamera_to_end = new GraphEdge(resetCamera, end, new GraphData(true));

            graph.AddVertices(
                checkBackRightLight,
                checkBackRightIndicatorLight,
                checkBackRightPins,
                checkBackRightPistons,
                checkBackRightGrousers,
                resetCamera,
                end
            );

            graph.AddEdges(
                checkBackRightLight_to_checkBackRightIndicatorLight,
                checkBackRightIndicatorLight_to_checkBackRightPins,
                checkBackRightPins_to_checkBackRightPistons,
                checkBackRightPistons_to_checkBackRightGrousers,
                checkBackRightGrousers_to_resetCamera,
                resetCamera_to_end
            );

            return (checkBackRightLight, end);
        }
    }
}


