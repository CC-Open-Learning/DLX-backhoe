using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using CheckTypes = TaskVertexManager.CheckTypes;
using HeavyEquipmentCheckTypes = RemoteEducation.Modules.HeavyEquipment.HeavyEquipmentModule.HeavyEquipmentCheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Fuel Water check part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildFuelWaterSeperatorScenario(CoreGraph graph)
        {
            // Vertices
            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();

            UserTaskVertex inspectBowl = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.FuelWaterSeparator.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.checkBowlTitle"),
                Description = Localizer.Localize("HeavyEquipment.checkBowlDescription"),
            };

            UserTaskVertex drainBowl = new UserTaskVertex(typeof(FuelWaterSeparator), (int)HeavyEquipmentCheckTypes.FuelWaterSeparatorDrained)
            {
                Title = Localizer.Localize("HeavyEquipment.drainFuelWaterSeparatorTitle"),
                Description = Localizer.Localize("HeavyEquipment.drainFuelWaterSeparatorDescription"),
            };

            // Transitions
            start.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.FuelWaterSeparatorCloseUp));

            inspectBowl.OnEnterVertex += (_) => module.InspectableManager.AddActiveElement(backhoe.FuelWaterSeparator.InspectableElement);
            inspectBowl.OnLeaveVertex += (_) => module.InspectableManager.RemoveActiveElement(backhoe.FuelWaterSeparator.InspectableElement);

            drainBowl.OnEnterVertex += (_) => StartCoroutine(backhoe.FuelWaterSeparator.DrainPress());

            // Define Edges
            GraphEdge start_to_inspectBowl = new GraphEdge(start, inspectBowl);
            GraphEdge inspectBowl_to_drainBowl = new GraphEdge(inspectBowl, drainBowl, new GraphData(true));
            GraphEdge drainBowl_to_end = new GraphEdge(drainBowl, end, new GraphData(true));

            // Add Vertices and Edges
            graph.AddVertices(start, end, inspectBowl, drainBowl);

            graph.AddEdges(start_to_inspectBowl, inspectBowl_to_drainBowl, drainBowl_to_end);

            return (start, end);
        }
    }
}
