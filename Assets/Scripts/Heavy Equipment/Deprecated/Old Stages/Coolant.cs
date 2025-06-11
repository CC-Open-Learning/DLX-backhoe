using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    partial class PreTripInspectionGS
    {
        /// <summary>Build the graph for the coolant tank part of the scenario.</summary>
        /// <param name="graph">The graph in the scene.</param>
        /// <returns>
        /// Start = A dummy vertex at the start of this section of the scenario. 
        ///   End = A dummy vertex at the end of this section of the scenario.
        /// </returns>
        float coolantTimedVerts = 2.5f;
        public (GraphVertex start, GraphVertex end) BuildCoolantScenario(CoreGraph graph)
        {
            // Vertices
            GraphVertex start = new GraphVertex();
            GraphVertex end = new GraphVertex();


            // Check the coolant level
            UserTaskVertex checkCoolantLevel = new UserTaskVertex(typeof(InspectableManager), CheckTypes.ElementsInspected, backhoe.EngineCoolant.InspectableElement)
            {
                Title = Localizer.Localize("HeavyEquipment.PreTripTitleCheckCoolantLevel"),
                Description = Localizer.Localize("HeavyEquipment.PreTripDescriptionCheckCoolantLevel")
            };

            // Transitions
            start.OnEnterVertex += (_) =>
            {
                camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.CoolantCloseup));
                backhoe.EngineCoolant.AddFlags(Interactable.Flags.InteractionsDisabled);
                Interactable.AddSubsetActivated(backhoe.EngineCoolant.InspectableElement, backhoe.EngineCoolant.Cap, backhoe.EngineCoolant.Fluid);
            };

            checkCoolantLevel.OnLeaveVertex += (_) =>
            {
                module.InspectableManager.RemoveActiveElement(backhoe.EngineCoolant.InspectableElement);
                ToggleInteractability(false, backhoe.EngineCoolant);
            };

            end.OnLeaveVertex += (_) => Interactable.ResetSubsetActivatedObjects();
            end.OnEnterVertex += (_) => camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Engine));

            // Define Edges
            GraphEdge start_to_checkCoolantLevel = new GraphEdge(start, checkCoolantLevel);
            GraphEdge checkCoolantLevel_to_end = new GraphEdge(checkCoolantLevel, end, new GraphData(true));

            // Add Vertices and Edges
            graph.AddVertices(
                start,
                end,
                checkCoolantLevel
                );

            graph.AddEdges(
                    start_to_checkCoolantLevel, 
                    checkCoolantLevel_to_end
                );

            return (start, end);
        }
    }
}
