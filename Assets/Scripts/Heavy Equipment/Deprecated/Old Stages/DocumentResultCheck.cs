using RemoteEducation.Localization;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the document result check part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildDocumentResultsCheck(CoreGraph graph)
        {
            // Vertices
            UserTaskVertex stage23OpenDoor = new UserTaskVertex(typeof(CabDoor), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage23OpenDoor"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage23OpenDoor"),
                ComponentIdentifier = CabDoor.DoorPositions.Left.ToString()
            };

            UserTaskVertex documentResult = CreateWaitTask("documentResult", timeBetweenVerts);
            documentResult.Title = Localizer.Localize("HeavyEquipment.PreTripTitleDocumentResult");
            documentResult.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionDocumentResult");

            GraphVertex end = new GraphVertex();

            // Transitions
            stage23OpenDoor.OnEnterVertex += (_) =>
            {
                // Close front hood if it's opened
                FrontHood hood = FindObjectOfType<FrontHood>();

                if (hood.IsOpen)
                    hood.ToggleHoodOpen(1);

                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.OutsideDoor));
            };

            stage23OpenDoor.OnLeaveVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.InsideCab));
            };

            documentResult.OnEnterVertex += (_) =>
            {
                backhoe.EngineSound.StopEngine();
            };

            // Define Edges
            GraphEdge stage23OpenDoor_to_documentResult = new GraphEdge(stage23OpenDoor, documentResult, new GraphData(1));
            GraphEdge documentResult_to_dummyEnd = new GraphEdge(documentResult, end, new GraphData(true));

            // Add Edges and Vertices
            graph.AddVertices(
               stage23OpenDoor,
               documentResult,
               end);

            graph.AddEdges(
                stage23OpenDoor_to_documentResult,
                documentResult_to_dummyEnd);

            return (stage23OpenDoor, end);
        }
    }
}

