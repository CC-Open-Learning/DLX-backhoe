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
        public (GraphVertex start, GraphVertex end) BuildFrontRightCheck(CoreGraph graph)
        {
            GraphVertex end = new GraphVertex();

            UserTaskVertex checkFrontRightLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Lights[0].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectFrontRightLights"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectFrontRightLights")
            };

            UserTaskVertex checkFrontRightIndicatorLight = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.IndicatorLights[0].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontRightIndicator"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontRightIndicator")
            };

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

            UserTaskVertex inspectFRWheelNuts = new UserTaskVertex(typeof(WheelNuts), CheckTypes.Bool)
            {
                ComponentIdentifier = WheelNuts.NutLocation.FrontRight.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleWheelNuts"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionWheelNuts")
            };

            UserTaskVertex frontRightTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontRightTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckFrontLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckFrontLeftTire"),
            };

            checkFrontRightLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.Lights[0].InspectableElement);
                backhoe.Lights[0].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.FrontRightLightsPOI));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.FrontRightLightsPOI).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 40f;
            };

            checkFrontRightLight.OnLeaveVertex += (_) =>
            {
                backhoe.Lights[0].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            checkFrontRightIndicatorLight.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(backhoe.IndicatorLights[0].InspectableElement);
                backhoe.IndicatorLights[0].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
            };

            checkFrontRightIndicatorLight.OnLeaveVertex += (_) =>
            {
                backhoe.IndicatorLights[0].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            };

            inspectRightPins.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetPins(false);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RightLiftInspection));
            };

            inspectRightPins.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetPins(true);
                backhoe.RightHydraulicPin.GetComponent<CapsuleCollider>().enabled = false;
            };

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
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 1f);
            };

            inspectRightHydraulics.OnLeaveVertex += (_) =>
            {
                sceneData.RightHydraulics.ToggleGroupInteraction(true);
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff); 
            };

            inspectFRWheelNuts.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                backhoe.InteractManager.SetWheelNuts(false);
            };

            inspectFRWheelNuts.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetWheelNuts(true);
            };

            frontRightTireCheck.OnEnterVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(sceneData.FrontRightTire.GroupElements);
                sceneData.FrontRightTire.ToggleGroupInteraction(false);
            };

            frontRightTireCheck.OnLeaveVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.FrontRightTire.ToggleGroupInteraction(true);
            };

            GraphEdge inspectRightPins_to_inspectRightPiston = new GraphEdge(inspectRightPins, inspectRightPiston, new GraphData(2));
            GraphEdge inspectRightPiston_to_inspectRightHydraulics = new GraphEdge(inspectRightPiston, inspectRightHydraulics, new GraphData(true));
            GraphEdge inspectRightHydraulics_to_inspectFRWheelNuts = new GraphEdge(inspectRightHydraulics, inspectFRWheelNuts, new GraphData(true));
            GraphEdge inspectFRWheelNuts_to_frontRightTireCheck = new GraphEdge(inspectFRWheelNuts, frontRightTireCheck, new GraphData(true));
            GraphEdge frontRightTireCheck_to_checkFrontRightLight = new GraphEdge(frontRightTireCheck, checkFrontRightLight, new GraphData(true));
            GraphEdge checkFrontRightLight_to_checkFrontRightIndicatorLight = new GraphEdge(checkFrontRightLight, checkFrontRightIndicatorLight, new GraphData(true));
            GraphEdge checkFrontRightIndicatorLight_to_end = new GraphEdge(checkFrontRightIndicatorLight, end, new GraphData(true));

            graph.AddVertices(
                inspectRightPins,
                inspectRightPiston,
                inspectRightHydraulics,
                inspectFRWheelNuts,
                frontRightTireCheck,
                checkFrontRightLight,
                checkFrontRightIndicatorLight,
                end
            );

            graph.AddEdges(
                inspectRightPins_to_inspectRightPiston,
                inspectRightPiston_to_inspectRightHydraulics,
                inspectRightHydraulics_to_inspectFRWheelNuts,
                inspectFRWheelNuts_to_frontRightTireCheck,
                frontRightTireCheck_to_checkFrontRightLight,
                checkFrontRightLight_to_checkFrontRightIndicatorLight,
                checkFrontRightIndicatorLight_to_end
            );

            return (inspectRightPins, end);
        }
    }
}


