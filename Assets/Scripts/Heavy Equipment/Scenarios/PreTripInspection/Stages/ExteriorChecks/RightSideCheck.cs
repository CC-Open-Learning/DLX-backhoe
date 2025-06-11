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
        public (GraphVertex start, GraphVertex end) BuildRightSideCheck(CoreGraph graph)
        {
            GraphVertex end = new GraphVertex();

            UserTaskVertex checkRightDoor = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, backhoe.CabDoors[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRightDoorFrame"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRightDoorFrame"),
            };

            UserTaskVertex checkRightWindows = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.RightDoorAndWindows.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckWheelsAndWindows"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckWheelsAndWindows")
            };

            UserTaskVertex inspectBRWheelNuts = new UserTaskVertex(typeof(WheelNuts), CheckTypes.Bool)
            {
                ComponentIdentifier = WheelNuts.NutLocation.BackRight.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleWheelNuts"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionWheelNuts")
            };

            UserTaskVertex checkBackRightWheel = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackRightTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackRightTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackRightTire"),
            };

            checkRightDoor.OnEnterVertex += (_) => {
                backhoe.EngineSound.ChangeEngineSoundPosition(EngineSoundController.Position.Front);

                module.InspectableManager.AddActiveElement(backhoe.CabDoors[1].InspectableElement);
                backhoe.CabDoors[1].ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RightDoor));
            };

            checkRightDoor.OnLeaveVertex += (_) => {
                module.InspectableManager.RemoveActiveElement(backhoe.CabDoors[1].InspectableElement);
                backhoe.CabDoors[1].ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            };

            //Need to do all windows
            checkRightWindows.OnEnterVertex += (_) =>
            {
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[0]);
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[8]);
                module.InspectableManager.AddActiveElement(sceneData.Task1_WheelsAndWindows.GroupElements[9]);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(false);
            };

            checkRightWindows.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Task1_WheelsAndWindows.ToggleGroupInteraction(true);
            };

            inspectBRWheelNuts.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().SetupTarget(2);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(2, 1f);
                backhoe.InteractManager.SetWheelNuts(false);
            };

            inspectBRWheelNuts.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetWheelNuts(true);
            };

            checkBackRightWheel.OnEnterVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(sceneData.BackRightTire.GroupElements);
                sceneData.BackRightTire.ToggleGroupInteraction(false);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.LiftArmDolly).GetComponent<DollyWithTargets>().MoveToTarget(1, 2f);
            };

            checkBackRightWheel.OnLeaveVertex += (_) => {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.BackRightTire.ToggleGroupInteraction(true);
            };

            GraphEdge checkRightDoor_to_checkRightWindows = new GraphEdge(checkRightDoor, checkRightWindows, new GraphData(true));
            GraphEdge checkRightWindows_to_inspectBRWheelNuts = new GraphEdge(checkRightWindows, inspectBRWheelNuts, new GraphData(true));
            GraphEdge inspectBRWheelNuts_to_checkBackRightWheel = new GraphEdge(inspectBRWheelNuts, checkBackRightWheel, new GraphData(true));
            GraphEdge checkBackRightWheel_to_inspectRightPins = new GraphEdge(checkBackRightWheel, end, new GraphData(true));

            graph.AddVertices(
                checkRightDoor,
                checkRightWindows,
                inspectBRWheelNuts,
                checkBackRightWheel,
                end
            );

            graph.AddEdges(
                checkRightDoor_to_checkRightWindows,
                checkRightWindows_to_inspectBRWheelNuts,
                inspectBRWheelNuts_to_checkBackRightWheel,
                checkBackRightWheel_to_inspectRightPins
            );

            return (checkRightDoor, end);
        }
    }
}


