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
        public (GraphVertex start, GraphVertex end) BuildLeftSideCheck(CoreGraph graph)
        {

            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();

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

            UserTaskVertex inspectBLWheelNuts = new UserTaskVertex(typeof(WheelNuts), CheckTypes.Bool)
            {
                ComponentIdentifier = WheelNuts.NutLocation.BackLeft.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleWheelNuts"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionWheelNuts")
            };

            UserTaskVertex checkBackLeftWheel = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackLeftTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackLeftTire"),
            };

            checkLeftDoor.OnEnterVertex += (_) => {
                backhoe.EngineSound.ChangeEngineSoundPosition(EngineSoundController.Position.Front);

                module.InspectableManager.AddActiveElement(backhoe.CabDoors[0].InspectableElement);
                backhoe.CabDoors[0].SetDoorInteractable(false);
                backhoe.CabDoors[0].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor).GetComponent<PointOfInterest>().transform.LookAt(sceneData.LeftDoorAndWindows.GroupElements[2].transform.position);
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
            };

            checkLeftWindows.OnLeaveVertex += (_) =>
            {
                backhoe.CabDoors[0].SetDoorInteractable(true);

                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(true);
            };

            inspectBLWheelNuts.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().SetupTarget(3);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(3, 1f);
                backhoe.InteractManager.SetWheelNuts(false);
            };

            inspectBLWheelNuts.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetWheelNuts(true);
            };

            checkBackLeftWheel.OnEnterVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(sceneData.BackLeftTire.GroupElements);
                sceneData.BackLeftTire.ToggleGroupInteraction(false);
            };

            checkBackLeftWheel.OnLeaveVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.BackLeftTire.ToggleGroupInteraction(true);
            };

            GraphEdge start_to_checkLeftDoor = new GraphEdge(start, checkLeftDoor, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge checkLeftDoor_to_checkLeftWindows = new GraphEdge(checkLeftDoor, checkLeftWindows, new GraphData(true));
            GraphEdge checkBackLeftWheel_to_inspectBLWheelNuts = new GraphEdge(checkLeftWindows, inspectBLWheelNuts, new GraphData(true));
            GraphEdge inspectBLWheelNuts_to_checkBackLeftWheel = new GraphEdge(inspectBLWheelNuts, checkBackLeftWheel, new GraphData(true));
            GraphEdge checkBackLeftWheel_to_checkBackLeftLight = new GraphEdge(checkBackLeftWheel, end, new GraphData(true));

            graph.AddVertices(
                start,
                checkLeftDoor,
                checkLeftWindows,
                inspectBLWheelNuts,
                checkBackLeftWheel,
                end
            );

            graph.AddEdges(
                start_to_checkLeftDoor,
                checkLeftDoor_to_checkLeftWindows,
                checkBackLeftWheel_to_inspectBLWheelNuts,
                inspectBLWheelNuts_to_checkBackLeftWheel,
                checkBackLeftWheel_to_checkBackLeftLight
            );

            return (start, end);
        }
    }
}


