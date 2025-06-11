using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Swing Linkage checks part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildSwingLinkageScenario(CoreGraph graph)
        {
            // Vertices
            UserTaskVertex start = CreateWaitTask("SwingLinkageCheckStart", timeBetweenVerts);
            start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage14Start");
            start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage14Start");

            GraphVertex end = new GraphVertex();

            UserTaskVertex checkSwingLinkageDebris = new UserTaskVertex(typeof(RemovableDebrisGroup), CheckTypes.Bool)
            {
                ComponentIdentifier = RemovableDebrisGroup.DebrisLocations.SwingLinkage.ToString(),
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckSwingLinkageDebris"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckSwingLinkageDebris")
            };

            UserTaskVertex checkSwingLinkageHydraulic = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, sceneData.Stage14Hydraulics.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckSwingLinkageHydraulic"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckSwingLinkageHydraulic")
            };

            UserTaskVertex checkSwingLinkagePinsAndRetainers = new UserTaskVertex(typeof(OtherInspectableObjects), CheckTypes.Int)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckSwingLinkagePinsAndRetainers"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckSwingLinkagePinsAndRetainers")
            };

            UserTaskVertex resetSwingLinkageCamera = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.SwingLinkageDolly.ToString()
            };

            start.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp).GetComponent<DollyWithTargets>().MoveToTarget(1, 1f);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp).GetComponent<DollyWithTargets>().SetUserControl(true);
            };

            // Transitions
            checkSwingLinkageDebris.OnEnterVertex += (_) =>
            {
                //Spawn and remove debris
                backhoe.InteractManager.SetDebris(true);
            };

            checkSwingLinkageDebris.OnLeaveVertex += (_) => backhoe.InteractManager.SetDebris(false);

            checkSwingLinkageHydraulic.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.Stage14Hydraulics.GroupElements);
                sceneData.Stage14Hydraulics.ToggleGroupInteraction(false);
                leanWindow.TurnOn();
            };

            checkSwingLinkageHydraulic.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
                sceneData.Stage14Hydraulics.ToggleGroupInteraction(true);
                leanWindow.TurnOff();
            };

            resetSwingLinkageCamera.OnEnterVertex += (_) => sceneData.GetPOI(PreTripInspectionData.CameraLocations.SwingLinkageCloseUp).GetComponent<DollyWithTargets>().MoveToTarget(0, 3f);
            resetSwingLinkageCamera.OnLeaveVertex += (_) => StageCompleted(Stage.SwingLinkage);

            checkSwingLinkagePinsAndRetainers.OnEnterVertex += (_) => backhoe.InteractManager.SetPins(false);
            checkSwingLinkagePinsAndRetainers.OnLeaveVertex += (_) => backhoe.InteractManager.SetPins(true);

            // Define Edges
            GraphEdge start_to_checkSwingLinkageDebris = new GraphEdge(start, checkSwingLinkageDebris, new GraphData(true));
            GraphEdge checkSwingLinkageDebris_to_checkSwingLinkageHydraulic = new GraphEdge(checkSwingLinkageDebris, checkSwingLinkageHydraulic, new GraphData(true));
            GraphEdge checkSwingLinkageHydraulic_to_checkSwingLinkagePinsAndRetainers = new GraphEdge(checkSwingLinkageHydraulic, checkSwingLinkagePinsAndRetainers, new GraphData(true));
            GraphEdge checkSwingLinkagePinsAndRetainers_to_resetSwingLinkageCamera = new GraphEdge(checkSwingLinkagePinsAndRetainers, resetSwingLinkageCamera, new GraphData(4));
            GraphEdge resetSwingLinkageCamera_to_end = new GraphEdge(resetSwingLinkageCamera, end, new GraphData(0));

            // Add Vertices and Edges
            graph.AddVertices(
                start,
                end,
                checkSwingLinkageDebris,
                checkSwingLinkageHydraulic,
                checkSwingLinkagePinsAndRetainers,
                resetSwingLinkageCamera);

            graph.AddEdges(
                start_to_checkSwingLinkageDebris,
                checkSwingLinkageDebris_to_checkSwingLinkageHydraulic,
                checkSwingLinkageHydraulic_to_checkSwingLinkagePinsAndRetainers,
                checkSwingLinkagePinsAndRetainers_to_resetSwingLinkageCamera,
                resetSwingLinkageCamera_to_end
                );

            return (start, end);
        }
    }
}

