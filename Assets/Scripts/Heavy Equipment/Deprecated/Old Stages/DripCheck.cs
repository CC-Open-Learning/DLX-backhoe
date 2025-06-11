using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using System.Diagnostics;
using CheckTypes = TaskVertexManager.CheckTypes;
using HeavyEquipmentCheckTypes = RemoteEducation.Modules.HeavyEquipment.HeavyEquipmentModule.HeavyEquipmentCheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>
        /// Build the graph for the fluid (Engine/Transmission) checks part of the scenario.
        /// </summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        public (GraphVertex start, GraphVertex end) BuildDripCheckScenario(CoreGraph graph, DripCheck dripCheck)
        {
            string identifier = dripCheck.DripCheckType.ToString();

            // Vertices
            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();

            UserTaskVertex removeDipstick = new UserTaskVertex(typeof(DripCheck), (int)HeavyEquipmentCheckTypes.DipStickIsRemoved)
            {
                ComponentIdentifier = identifier
            };

            UserTaskVertex wipeStick = new UserTaskVertex(typeof(DripCheck), (int)HeavyEquipmentCheckTypes.DipStickIsWiped)
            {

                ComponentIdentifier = identifier
            };

            UserTaskVertex getFluidLevel = new UserTaskVertex(typeof(DripCheck), (int)HeavyEquipmentCheckTypes.DipStickIsRemoved)
            {

                ComponentIdentifier = identifier
            };

            UserTaskVertex inspectFluidLevel = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, dripCheck.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleInspectFluidLevel"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionInspectFluidLevel")
            };

            UserTaskVertex secondWipe = new UserTaskVertex(typeof(DripCheck), (int)HeavyEquipmentCheckTypes.DipStickIsWiped)
            {

                ComponentIdentifier = identifier
            };

            UserTaskVertex returnDipStick = new UserTaskVertex(typeof(DripCheck), (int)HeavyEquipmentCheckTypes.DipStickIsReturned)
            {

                ComponentIdentifier = identifier
            };

            // Transitions
            start.OnEnterVertex += (_) =>       
            {
                backhoe.TransmissionDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                backhoe.SecondTransmissionDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                backhoe.EngineDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

                PreTripInspectionData.CameraLocations location = dripCheck.DripCheckType == DripCheck.DripCheckTypes.EngineFluid ? PreTripInspectionData.CameraLocations.DipstickPOI : PreTripInspectionData.CameraLocations.TransmissionDipstickPOI;
                camera.SendCameraToPosition(sceneData.GetPOI(location));
            };

            end.OnLeaveVertex += (_) => Interactable.ResetSubsetActivatedObjects();

            removeDipstick.OnEnterVertex += (_) => dripCheck.TakeOutStick();
            removeDipstick.OnLeaveVertex += (_) => dripCheck.WipeStain();
            getFluidLevel.OnEnterVertex += (_) => dripCheck.ReturnDipstick();
            secondWipe.OnEnterVertex += (_) => dripCheck.WipeStain();
            returnDipStick.OnEnterVertex += (_) => dripCheck.ReturnDipstick();

            inspectFluidLevel.OnEnterVertex += (_) =>
            {
                // Resetting the inspection status of the dripcheck to make sure it is never skipped/locked for the learner upon loading the save.
                dripCheck.resetInspection();
                dripCheck.TakeOutStick();
                module.InspectableManager.AddActiveElement(dripCheck.InspectableElement);

                backhoe.TransmissionDrip.ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                backhoe.SecondTransmissionDrip.ToggleFlags(false, Interactable.Flags.InteractionsDisabled);
                backhoe.EngineDrip.ToggleFlags(false, Interactable.Flags.InteractionsDisabled);

                // enable oil objects interactivity
                ToggleInteractability(true, backhoe.EngineOil);
                ToggleInteractability(true, backhoe.TransmissionOil);
                ToggleInteractability(true, backhoe.SecondTransmissionOil);
                backhoe.EngineOil.ToggleTooltip();
                backhoe.TransmissionOil.ToggleTooltip();
                backhoe.SecondTransmissionOil.ToggleTooltip();
                ToggleInteractability(true, backhoe.EngineOil, backhoe.TransmissionOil, backhoe.SecondTransmissionOil);
            };

            inspectFluidLevel.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.RemoveActiveElement(dripCheck.InspectableElement);
                Interactable.UnselectCurrentSelections();

                backhoe.TransmissionDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                backhoe.SecondTransmissionDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
                backhoe.EngineDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

                // disable oil objects interactivity
                ToggleInteractability(false, backhoe.EngineOil);
                ToggleInteractability(false, backhoe.TransmissionOil);
                ToggleInteractability(false, backhoe.SecondTransmissionOil);
                backhoe.EngineOil.ToggleTooltip();
                backhoe.TransmissionOil.ToggleTooltip();
                backhoe.SecondTransmissionOil.ToggleTooltip();
                ToggleInteractability(false, backhoe.EngineOil, backhoe.TransmissionOil, backhoe.SecondTransmissionOil);
            };



            end.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));
            };

            // Defines Edges
            GraphEdge start_to_removeDipstick = new GraphEdge(start, removeDipstick);
            GraphEdge removeDipstick_to_wipeStick = new GraphEdge(removeDipstick, wipeStick, new GraphData(true));
            GraphEdge wipeStick_to_getFluidLevel = new GraphEdge(wipeStick, getFluidLevel, new GraphData(true));
            GraphEdge getFluidLevel_to_inspectFluidLevel = new GraphEdge(getFluidLevel, inspectFluidLevel, new GraphData(false));
            GraphEdge inspectFluidLevel_to_secondWipe = new GraphEdge(inspectFluidLevel, secondWipe, new GraphData(true));
            GraphEdge secondWipe_to_returnDipStick = new GraphEdge(secondWipe, returnDipStick, new GraphData(true));
            GraphEdge returnDipStick_to_end = new GraphEdge(returnDipStick, end, new GraphData(true));

            // Add Vertices and Edges
            graph.AddVertices(
                start,
                end,
                removeDipstick,
                wipeStick,
                getFluidLevel,
                inspectFluidLevel,
                secondWipe,
                returnDipStick);

            graph.AddEdges(
                start_to_removeDipstick,
                removeDipstick_to_wipeStick,
                wipeStick_to_getFluidLevel,
                getFluidLevel_to_inspectFluidLevel,
                inspectFluidLevel_to_secondWipe,
                secondWipe_to_returnDipStick,
                returnDipStick_to_end);

            return (start, end);
        }
    }
}
