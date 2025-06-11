using RemoteEducation.Localization;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the First Transmission check of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) CheckTransmission(CoreGraph graph, bool firstTransmissionInspection)
        {
            // Vertices
            UserTaskVertex selectTransmissionFluid = new UserTaskVertex()
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleSelectTransmissionOil"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionSelectTransmissionOil")
            };

            (GraphVertex start, GraphVertex end) transmissionFluidVerts = BuildDripCheckScenario(graph, firstTransmissionInspection ? backhoe.TransmissionDrip : backhoe.SecondTransmissionDrip);

            // Only enable this vertex if this is the first time we're checking transmission
            UserTaskVertex lastEnginePrompt = CreateWaitTask("lastEnginePrompt", firstTransmissionInspection ? timeBetweenVerts : 0f);
            lastEnginePrompt.Title = firstTransmissionInspection ? Localizer.Localize("HeavyEquipment.PreTripTitleEnginePrompt") : "";
            lastEnginePrompt.Description = firstTransmissionInspection ? Localizer.Localize("HeavyEquipment.PreTripDescriptionEnginePrompt") : "";

            UserTaskVertex end = new UserTaskVertex();

            // Transitions
            selectTransmissionFluid.OnEnterVertex += (_) =>
            {
                ToggleInteractability(true, firstTransmissionInspection ? backhoe.TransmissionDrip : backhoe.SecondTransmissionDrip);
                ToggleInteractability(false, backhoe.TransmissionOil, backhoe.SecondTransmissionOil);
            };

            selectTransmissionFluid.OnLeaveVertex += (_) =>
            {
                ToggleInteractability(false, firstTransmissionInspection ? backhoe.TransmissionDrip : backhoe.SecondTransmissionDrip);
            };

            end.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            };

            // Define Edges
            GraphEdge selectTransmissionFluid_to_checkTransmissionFluid = new GraphEdge(selectTransmissionFluid, transmissionFluidVerts.start, new GraphData(firstTransmissionInspection ? backhoe.TransmissionDrip : backhoe.SecondTransmissionDrip));
            GraphEdge transmissionFluid_to_lastPrompt = new GraphEdge(transmissionFluidVerts.end, lastEnginePrompt);
            GraphEdge lastPrompt_to_stage2HydraulicFluidCheck = new GraphEdge(lastEnginePrompt, end, new GraphData(true));


            // Add Vertices and Edges
            graph.AddVertices(end, selectTransmissionFluid, lastEnginePrompt);

            graph.AddEdges(
                transmissionFluid_to_lastPrompt,
                lastPrompt_to_stage2HydraulicFluidCheck,
                selectTransmissionFluid_to_checkTransmissionFluid);

            return (selectTransmissionFluid, end);
        }
    }
}
