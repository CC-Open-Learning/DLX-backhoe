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
        public (GraphVertex start, GraphVertex end) BuildRearBoomLockScenario(CoreGraph graph)
        {
            GraphVertex start = new GraphVertex();

            UserTaskVertex checkRearLock = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.BoomLockLatch.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearBoomLockLatch"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearBoomLockLatch")
            };

            UserTaskVertex zoomOutFromRearLock = CreateWaitTask("zoomOutFromRearLock", 2f);

            GraphVertex end = new GraphVertex();

            start.OnEnterVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InspectionStartPOI));
            };

            zoomOutFromRearLock.OnEnterVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.RearBoomLockZoomedOutPOI));

            };
            zoomOutFromRearLock.OnLeaveVertex += (_) => {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            };

            checkRearLock.OnEnterVertex += (_) => {
                ToggleInteractability(true, backhoe.BoomLockLatch);
                module.InspectableManager.AddActiveElement(backhoe.BoomLockLatch.InspectableElement);
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.BoomLatchLeverPOI));

            };
            checkRearLock.OnLeaveVertex += (_) => {
                ToggleInteractability(false, backhoe.BoomLockLatch);
                module.InspectableManager.RemoveActiveElement(backhoe.BoomLockLatch.InspectableElement);
            };

            GraphEdge start_checkRearLock = new GraphEdge(start, checkRearLock, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge checkRearLock_zoomOutFromRearLock = new GraphEdge(checkRearLock, zoomOutFromRearLock, new GraphData(true));
            GraphEdge zoomOutFromRearLock_end = new GraphEdge(zoomOutFromRearLock, end, new GraphData(true));

            graph.AddVertices(
                start,
                checkRearLock,
                zoomOutFromRearLock,
                end
            );

            graph.AddEdges(
                start_checkRearLock,
                checkRearLock_zoomOutFromRearLock,
                zoomOutFromRearLock_end
            );

            return (start, end);
        }
    }
}

