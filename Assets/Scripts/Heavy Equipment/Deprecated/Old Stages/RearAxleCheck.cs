using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the Rear axle check part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildRearAxleScenario(CoreGraph graph)
        {
            // Vertices

            // Stage 12 Start
            UserTaskVertex stage12Start = CreateWaitTask("stage12Start", timeBetweenVerts);
            stage12Start.Title = Localizer.Localize("HeavyEquipment.PreTripTitleStage12Start");
            stage12Start.Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionStage12Start");
            // Stage 12 End
            GraphVertex stage12End = new GraphVertex();

            UserTaskVertex backLeftTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackLeftTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackLeftTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackLeftTire"),
            };
            UserTaskVertex backRightTireCheck = new UserTaskVertex(typeof(InspectableManager), TaskVertexManager.CheckTypes.ElementsInspected, sceneData.BackRightTire.GetElementsToCheck())
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckBackRightTire"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckBackRightTire"),
            };
            UserTaskVertex rearAxleCheckResetCamera = new UserTaskVertex(typeof(DollyWithTargets), CheckTypes.Int)
            {
                ComponentIdentifier = DollyWithTargets.Positions.RearAxle.ToString()
            };

            backLeftTireCheck.OnEnterVertex += (_) =>
            {
                module.InspectableManager.UpdateActiveElements(sceneData.BackLeftTire.GroupElements);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(3, 2f);
                sceneData.BackLeftTire.ToggleGroupInteraction(false);
            };

            backLeftTireCheck.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);

            backRightTireCheck.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly));
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().SetupTarget(2);
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.WheelsDolly).GetComponent<DollyWithTargets>().MoveToTarget(2, 1f);
                module.InspectableManager.UpdateActiveElements(sceneData.BackRightTire.GroupElements);
                sceneData.BackRightTire.ToggleGroupInteraction(false);
            };

            backRightTireCheck.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);

            rearAxleCheckResetCamera.OnEnterVertex += (_) =>
            {
                sceneData.GetPOI(PreTripInspectionData.CameraLocations.UnderRearAxle).GetComponent<DollyWithTargets>().MoveToTarget(0, 2f);
            };

            // inspect back axle
            UserTaskVertex stage12CheckRearAxle = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.Axles[1].InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckRearAxle"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckRearAxle")
            };

            // Transitions
            stage12CheckRearAxle.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.UnderRearAxle));
                module.InspectableManager.AddActiveElement(backhoe.Axles[1].InspectableElement);
                backhoe.InteractManager.SetAxles(false);
            };

            stage12CheckRearAxle.OnLeaveVertex += (_) =>
            {
                backhoe.InteractManager.SetAxles(true);
                module.InspectableManager.RemoveActiveElement(backhoe.Axles[1].InspectableElement);
            };


            GraphEdge stage12Start_to_backRightTireCheck = new GraphEdge(stage12Start, backRightTireCheck, new GraphData(true));
            GraphEdge backRightTireCheck_to_backLeftTireCheck = new GraphEdge(backRightTireCheck, backLeftTireCheck, new GraphData(true));
            GraphEdge backLeftTireCheck_to_stage12CheckRearAxle = new GraphEdge(backLeftTireCheck, stage12CheckRearAxle, new GraphData(true));
            GraphEdge stage12CheckRearAxle_to_rearAxleCheckResetCamera = new GraphEdge(stage12CheckRearAxle, rearAxleCheckResetCamera, new GraphData(true));
            GraphEdge rearAxleCheckResetCamera_to_stage12End = new GraphEdge(rearAxleCheckResetCamera, stage12End, new GraphData(0));

            graph.AddVertices(stage12End, stage12Start, stage12CheckRearAxle, backLeftTireCheck, backRightTireCheck, rearAxleCheckResetCamera);
            graph.AddEdges(
                stage12Start_to_backRightTireCheck,
                backRightTireCheck_to_backLeftTireCheck,
                backLeftTireCheck_to_stage12CheckRearAxle,
                stage12CheckRearAxle_to_rearAxleCheckResetCamera,
                rearAxleCheckResetCamera_to_stage12End
                );

            return (stage12Start, stage12End);
        }
    }
}

