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
        public (GraphVertex start, GraphVertex end) BuildFrontLeftCornerCheck(CoreGraph graph)
        {

            GraphVertex end = new GraphVertex();

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

            UserTaskVertex inspectFLWheelNuts = new UserTaskVertex(typeof(WheelNuts), CheckTypes.Bool)
            {
                ComponentIdentifier = WheelNuts.NutLocation.FrontLeft.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleWheelNuts"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionWheelNuts")
            };

            UserTaskVertex frontLeftTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontLeftTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontLeftTire"),
            };

            inspectLeftPins.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetPins(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.EngineLoaderLeftSide));
            };

            checkFrontLeftLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.Lights[1].InspectableElement);
                backhoe.Lights[1].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.FrontLeftLightsPOI));
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

            inspectLeftPins.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetPins(true);
                backhoe.LeftHydraulicPin.GetComponent<CapsuleCollider>().enabled = false;
            };

            inspectLeftPiston.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.LeftPiston.GroupElements);
                sceneData.LeftPiston.ToggleGroupInteraction(false);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 2f);
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

            inspectFLWheelNuts.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                backhoe.InteractManager.SetWheelNuts(false);
            };

            inspectFLWheelNuts.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetWheelNuts(true);
            };

            frontLeftTireCheck.OnEnterVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(sceneData.FrontLeftTire.GroupElements);
                sceneData.FrontLeftTire.ToggleGroupInteraction(false);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 1f);
            };

            frontLeftTireCheck.OnLeaveVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.FrontLeftTire.ToggleGroupInteraction(true);
            };

            GraphEdge inspectLeftPins_to_inspectLeftPiston = new GraphEdge(inspectLeftPins, inspectLeftPiston, new GraphData(3));
            GraphEdge inspectLeftPiston_to_inspectLeftHydraulics = new GraphEdge(inspectLeftPiston, inspectLeftHydraulics, new GraphData(true));
            GraphEdge inspectLeftHydraulics_to_inspectFLWheelNuts = new GraphEdge(inspectLeftHydraulics, inspectFLWheelNuts, new GraphData(true));
            GraphEdge inspectFLWheelNuts_to_frontLeftTireCheck = new GraphEdge(inspectFLWheelNuts, frontLeftTireCheck, new GraphData(true));
            GraphEdge frontLeftTireCheck_to_checkFrontLeftLight = new GraphEdge(frontLeftTireCheck, checkFrontLeftLight, new GraphData(true));
            GraphEdge checkFrontLeftLight_to_checkFrontLeftIndicatorLight = new GraphEdge(checkFrontLeftLight, checkFrontLeftIndicatorLight, new GraphData(true));
            GraphEdge checkFrontLeftIndicatorLight_to_end = new GraphEdge(checkFrontLeftIndicatorLight, end, new GraphData(true));

            graph.AddVertices(
                inspectLeftPins,
                inspectLeftPiston,
                inspectLeftHydraulics,
                inspectFLWheelNuts,
                frontLeftTireCheck,
                checkFrontLeftLight,
                checkFrontLeftIndicatorLight,
                end
            );

            graph.AddEdges(
                inspectLeftPins_to_inspectLeftPiston,
                inspectLeftPiston_to_inspectLeftHydraulics,
                inspectLeftHydraulics_to_inspectFLWheelNuts,
                inspectFLWheelNuts_to_frontLeftTireCheck,
                frontLeftTireCheck_to_checkFrontLeftLight,
                checkFrontLeftLight_to_checkFrontLeftIndicatorLight,
                checkFrontLeftIndicatorLight_to_end
            );

            return (inspectLeftPins, end);
        }
    }
}


