/*
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections.Generic;
using UnityEngine;
using Lean.Gui;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public partial class PreTripInspectionGS : MonoBehaviour, ITaskableGraphBuilder
    {
        public delegate void OnStageCompleteEvent(string stageName);
        /// <summary>Event that fires whenever a <see cref="SoundEffect"/> is played.</summary>
        public static event OnStageCompleteEvent OnStageComplete;

        private Dictionary<GraphVertex, EdgeTakenTracker> edgeTakenTrackers;

        private HeavyEquipmentModule module;
        private PreTripInspectionData sceneData;
        private new CoreCameraController camera;
        private BackhoeController backhoe;
        private LeanWindow leanWindow;

        const float timeBetweenVerts = 5f;

        /// <summary>
        /// This graph is visualized at https://lucid.app/lucidchart/invitations/accept/inv_5dc87b21-3d2d-4d02-bd79-ae3db19c3d2e
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="extensionModule"></param>
        /// <returns></returns>
        public CoreGraph BuildGraph(out GraphVertex start, out GraphVertex end, IExtensionModule extensionModule)
        {
            #region Setup

            module = extensionModule as HeavyEquipmentModule;
            sceneData = module.preTripInspectionData;
            camera = module.CoreCameraController;
            backhoe = sceneData.BackhoeController;
            leanWindow = ScenarioManager.Instance.WindowManager.WindowGroup.Find("Checklist Window").GetComponent<LeanWindow>();

            edgeTakenTrackers = new Dictionary<GraphVertex, EdgeTakenTracker>();

            
            module.InspectableManager.OnElementStateChanged += SetElementToReset;



            //create the wait task that will be used for this graph.
            WaitTask.Initialize();

            CoreGraph graph = new CoreGraph();

            start = new GraphVertex();
            start.OnLeaveVertex += (_) => module.InspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            start.OnEnterVertex += (_) => camera.TogglePOIButtonPanelVisability(false);

            end = new GraphVertex();

            // dummyend used for testing purposes, keeping here in case it's needed again
            //UserTaskVertex dummyEnd = new UserTaskVertex()
            //{
            //    Title = "Last Task",
            //    Description = "Last task for debugging, press \"Z\" to continue"
            //};

            //dummyEnd.OnEnterVertex += (_) =>
            //{
            //    camera.SendCameraToPosition(sceneData.GetPOI(PreTripInspectionData.CameraLocations.Overview));
            //    Debug.Log("All Done :D");
            //};

            #endregion

            (GraphVertex start, GraphVertex end) Stage1And2Verts = BuildWheelsAndWindowsEngineScenario(graph);
            (GraphVertex start, GraphVertex end) Stage345Verts = BuildCabChecklistScenario(graph);
            (GraphVertex start, GraphVertex end) Stage678Verts = BuildFrontLoaderScenario(graph);
            (GraphVertex start, GraphVertex end) stage8pVerts = BuildEngineBeltCheckScenario(graph);
            (GraphVertex start, GraphVertex end) stage9Verts = BuildTiresAndSteeringScenario(graph);
            (GraphVertex start, GraphVertex end) stage10Verts = BuildLiftArmScenario(graph);
            (GraphVertex start, GraphVertex end) stage11Verts = BuildROPSScenario(graph);
            (GraphVertex start, GraphVertex end) Stage13Verts = BuildGrouserCheckScenario(graph);
            (GraphVertex start, GraphVertex end) Stage14Verts = BuildSwingLinkageScenario(graph);
            (GraphVertex start, GraphVertex end) Stage15to19Verts = BuildRearBoomScenario(graph);
            (GraphVertex start, GraphVertex end) Stage21Verts = BuildCabScenario(graph);
            (GraphVertex start, GraphVertex end) Stage22Verts = BuildSecondTransmissionCheckScenario(graph);
            (GraphVertex start, GraphVertex end) Stage23Verts = BuildDocumentResultsCheck(graph);

            var Stages = new List<(GraphVertex start, GraphVertex end)>() {
                Stage1And2Verts,
                Stage345Verts,
                Stage678Verts,
                stage8pVerts,
                stage9Verts,
                stage10Verts,
                stage11Verts,
                Stage13Verts,
                Stage14Verts,
                Stage15to19Verts,
                Stage21Verts,
                Stage22Verts,
                Stage23Verts
            };

            for (int i = 1; i < Stages.Count; i++)
            {
                GraphEdge e = new GraphEdge(Stages[i - 1].end, Stages[i].start, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
                graph.AddEdge(e);
            }

            GraphEdge startEdge = new GraphEdge(start, Stages[0].start, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            GraphEdge endEdge = new GraphEdge(Stages[Stages.Count - 1].end, end, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));

            // dummy end testing, keeping this here in case it's needed again
            //GraphEdge endEdge = new GraphEdge(Stages[Stages.Count - 1].end, dummyEnd, new GraphData(GraphData.EdgeSpecialCases.AlwaysPass));
            //GraphEdge dummyEnd_to_end = new GraphEdge(dummyEnd, end, new GraphData("This edge can only be taken by pressing z. Its for debugging purposes"));

            graph.AddVertices(start, end);
            graph.AddEdges(startEdge, endEdge);

            return graph;
        }

        /// <summary>Toggles if <see cref="Interactable"/>s are active.</summary>
        /// <param name="turnOn">If the objects should be active.</param>
        /// <param name="interactables">The objects to toggle.</param>
        private void ToggleInteractability(bool turnOn, params Interactable[] interactables)
        {
            foreach (Interactable interactable in interactables)
            {
                interactable.ToggleFlags(!turnOn, Interactable.Flags.InteractionsDisabled);
            }
        }

        /// <summary>Fires an event broadcasting that the inspection stage <paramref name="stageName"/> has been completed.</summary>
        private void StageCompleted(Stage stage)
        {
            if (OnStageComplete != null)
                OnStageComplete(stage.ToString());
        }

        /// <summary>Deselects object when a state has been selected</summary>
        public void SetElementToReset(InspectableElement element)
        {
            InspectionPanelManager temp = FindObjectOfType<InspectionPanelManager>();
            var panel = temp.GetPanelByElement(element);

            if (panel != null)  
                temp.Release(panel);
            
            Interactable.UnselectCurrentSelections();
        }
    }
}
*/