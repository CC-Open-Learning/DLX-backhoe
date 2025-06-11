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
        public (GraphVertex start, GraphVertex end) BuildOverallChecks(CoreGraph graph)
        {
            GraphVertex end = new GraphVertex();

            UserTaskVertex inspectLiftArm = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.LiftArm.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleLiftArm"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionLiftArm"),
            };

            UserTaskVertex resetLiftArm = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.FrontLiftArm.ToString()
            };

            UserTaskVertex inspectROPS = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.ROPS.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleROPS"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionROPS"),
            };

            UserTaskVertex resetROPS = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.LightsDolly.ToString()
            };

            UserTaskVertex frontAxleInspectionCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.FrontAxle.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleFrontAxle"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionFrontAxle"),
            };

            UserTaskVertex resetFrontAxle = CreateWaitTask("resetFrontAxle", 2f);

            inspectLiftArm.OnEnterVertex += (_) =>
            {
                backhoe.InteractManager.SetLiftArm(false);
                module.InspectableManager.UpdateActiveElements(sceneData.LiftArm.GroupElements);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().SetUserControl(true);
            };

            inspectLiftArm.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                backhoe.InteractManager.SetLiftArm(true);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 20f;
            };

            resetLiftArm.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().MoveToTarget(0, 3f);

            inspectROPS.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI));
                sceneData.RightDoorAndWindows.ToggleGroupInteraction(true);
                ToggleInteractability(true, backhoe.ROPS.GetComponent<Interactable>());
                module.InspectableManager.UpdateActiveElements(sceneData.ROPS.GroupElements);
            };

            inspectROPS.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, backhoe.ROPS.GetComponent<Interactable>());
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.UnderFrontAxle).GetComponent<PointOfInterest>().VirtualCamera.m_Lens.FieldOfView = 40f;
            };

            resetROPS.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.LightsDollyPOI).GetComponent<DollyWithTargets>().MoveToTarget(0, 3f);

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

            resetFrontAxle.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor)); 
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor).GetComponent<PointOfInterest>().transform.LookAt(backhoe.CabDoors[0].transform);
            };
            
            GraphEdge inspectLiftArm_to_resetLiftArm = new GraphEdge(inspectLiftArm, resetLiftArm, new GraphData(true));
            GraphEdge resetLiftArm_to_inspectROPS = new GraphEdge(resetLiftArm, inspectROPS, new GraphData(0));
            GraphEdge inspectROPS_to_resetROPS = new GraphEdge(inspectROPS, resetROPS, new GraphData(true));
            GraphEdge resetROPS_to_inspectFrontAxle = new GraphEdge(resetROPS, frontAxleInspectionCheck, new GraphData(0));
            GraphEdge inspectFrontAxle_to_resetFrontAxle = new GraphEdge(frontAxleInspectionCheck, resetFrontAxle, new GraphData(true));
            GraphEdge resetFrontAxle_to_end = new GraphEdge(resetFrontAxle, end, new GraphData(true));

            graph.AddVertices(
                inspectLiftArm,
                resetLiftArm,
                inspectROPS,
                resetROPS,
                frontAxleInspectionCheck,
                resetFrontAxle,
                end
            );

            graph.AddEdges(
                inspectLiftArm_to_resetLiftArm,
                resetLiftArm_to_inspectROPS,
                inspectROPS_to_resetROPS,
                resetROPS_to_inspectFrontAxle,
                inspectFrontAxle_to_resetFrontAxle,
                resetFrontAxle_to_end
            );

            return (inspectLiftArm, end);
        }
    }
}


