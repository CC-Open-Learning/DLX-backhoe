using RemoteEducation.Helpers.Unity;
using RemoteEducation.Scenarios;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.UI;
using Lean.Gui;

public class ComponentPanelManager : MonoBehaviour
{
    private const float ContentTransitionTime = 0.75f;
    
    [SerializeField]
    private GameObject ContentPanelPrefab;

    private LeanWindow Window;
    private ComponentPanel ComponentPanel;
    private ComponentPanelData CurrentPageData;

    private static ComponentPanelManager Instance;

    public void Initalize()
    {
        Instance = this;

        AttatchToSlideMenuManager();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /* Method Header : AttatchToSlideMenuManager
     * This method will instantiate a the component info panel and attach it to the
     */
    private void AttatchToSlideMenuManager()
    {
        //set up the component info pane
        GameObject componentPanelGO = Instantiate(ContentPanelPrefab);

        Window = componentPanelGO.GetComponent<LeanWindow>();
        ScenarioManager.Instance.WindowManager.Add(Window);

        ComponentPanel = componentPanelGO.GetComponent<ComponentPanel>();
    }

    public static void UpdateComponentPanel(IComponentPanelable component)
    {
        if(Instance != null)
        {
            Instance.ShowComponentPanel(component);
        }
        else
        {
            Debug.LogWarning("Something tried to show a component info panel before the ComponentPanelManager was active.");
        }
    }

    /* Method Header : ShowComponentPanel
     * If the ComponentPanelData passed in isn't null, this method will ensure
     * that the panel is visible and showing the data passed in.
     * If null is passed in, then the panel will be hidden.
     */
    private void ShowComponentPanel(IComponentPanelable component)
    {
        //if we should close the panel
        if(component == null)
        {
            //hide it if it's open
            Window.TurnOff();
        }
        else
        {
            if (Window.On)
            {
                //if the panel is already extended, but the content is different
                // then transition between the 2 data sets 
                if(component.ComponentPanelData != CurrentPageData)
                {
                    StartCoroutine(SwapPanelData(component.ComponentPanelData));
                }
            }
            else
            {
                if(component.ComponentPanelData != CurrentPageData)
                {
                    //update the data in the panel
                    ComponentPanel.SetPageValues(component.ComponentPanelData);
                }

                Window.TurnOn();
            }
        }

        CurrentPageData = component == null ? null : component.ComponentPanelData;
    }

    /* Method Header : SwapPanelData
     * This coroutine will animate the change between 2 different data sets
     * that can be displayed on the panel.
     * This is used when transitioning between 2 pages without having close the panel
     */
    private IEnumerator SwapPanelData(ComponentPanelData panelData)
    {
        //fade out the data
        float startTime = Time.time;
        float endTime = startTime + ContentTransitionTime;

        while (Time.time <= endTime)
        {
            //go from one to zero
            ComponentPanel.ContentOpacity = 1 - (Time.time - startTime) / ContentTransitionTime;
            yield return null;
        }

        ComponentPanel.ContentOpacity = 0;
        ComponentPanel.SetPageValues(panelData);

        //fade the content back in
        startTime = Time.time;
        endTime = startTime + ContentTransitionTime;

        while (Time.time <= endTime)
        {
            ComponentPanel.ContentOpacity = (Time.time - startTime) / ContentTransitionTime;
            yield return null;
        }

        ComponentPanel.ContentOpacity = 1;
    }
}
