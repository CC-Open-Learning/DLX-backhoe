using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using VARLab.CloudSave;
using UnityEngine.Events;
using RemoteEducation.Modules.HeavyEquipment;
using System.Linq;

public class SaveDataSupport : MonoBehaviour
{
    [SerializeField]
    private SaveData _saveData;

    [SerializeField]
    private BackhoeController _backhoeController;

    /// <summary>
    /// This was changed to be an interface to allow testing, this member will be getting the component off the object as the scripts will both be attached to the
    /// same object allowing the reference to be made since when the type is changed to an interface the serialize field disappears in the editor.
    /// </summary>
    [SerializeField]
    private ICloudSaving _cloudSave;

    [SerializeField]
    private AzureSaveSystem _saveSystem;


    public UnityEvent<bool> _onProgressSaveSuccess;

    public UnityEvent<bool> _onLoadEvent;

    public UnityEvent<List<InspectableElement>> _onLoadBadElements;

    [SerializeField]
    private GameObject _removableDebrisGroups;

    private void Start()
    {
        if(_saveSystem == null)
        {
            _ = TryGetComponent<AzureSaveSystem>(out _saveSystem);
        }
        _saveSystem.RequestCompleted += _saveSystem_RequestCompleted;
        if(_cloudSave == null)
        {
            _ = TryGetComponent<ICloudSaving>(out _cloudSave);
        }
    }

    private void _saveSystem_RequestCompleted(object sender, AzureSaveSystem.RequestCompletedEventArgs args)
    {
        if (args.Action == AzureSaveSystem.RequestAction.Save)
        {
            bool success = args.Success;
            _onProgressSaveSuccess?.Invoke(success);
        }  
    }


    /// <summary>
    /// This is a generic method that is meant to grab all the elements in a dictionary of InspectableControllers that has a corresponding inspectable list
    /// for each controller
    /// </summary>
    /// <typeparam name="InspectableController"></typeparam>
    /// <typeparam name="InspectableElement"></typeparam>
    /// <param name="elementsByController"></param>
    /// <returns></returns>
    public void GetElementsFromControllerList(Dictionary<InspectableController, List<InspectableElement>> elementsByController)
    {
        List<string> badElementList = new();
        foreach (List<InspectableElement> elementList in elementsByController.Values)
        {
            foreach (InspectableElement element in elementList)
            {
                string prefabName = element.ToString().Remove(element.ToString().IndexOf(' '));
                if(prefabName.Contains("Good") || prefabName.Contains("Full") || prefabName.Contains("No_Debris")) //this is too disregard "good" or correct states and only save bad ones.
                {
                    continue;
                }
                badElementList.Add(prefabName);
                Debug.Log("Bad Element List: " + element.ToString());
            }
        }

        _saveData.badElements = badElementList;
        _cloudSave.Save();
    }

    /// <summary>
    /// This is a method to pull the necessary information from an inspectable element list to track state and save it to our save data object to be serialized.
    /// </summary>
    /// <param name="elements"> The list of inspectable elements </param>
    /// <returns></returns>
    public void GatherAllInspectableElements(List<InspectableElement> elements)
    {
        List<ProgressData> completeInspectableList = new List<ProgressData>();

        foreach (InspectableElement element in elements)
        {
            ProgressData elementData = new ProgressData();

            elementData.ElementName = element.GetComponent<ElementSpawningData>().ElementName;
            elementData.CurrentStatus = (int)element.CurrentStatus;
            elementData.currentlyInspectable = element.CurrentlyInspectable;
            elementData.UserSelectedIdentifier = element.UserSelectedIdentifier;
            elementData.StateIdentifier = element.StateIdentifier;
            elementData.State = (int)element.State;

            completeInspectableList.Add(elementData);
        }

        SaveDebrisGroupStates();

        _saveData.AllElements = completeInspectableList;
        _cloudSave.Save();
    }

    /// <summary>
    /// Simple method for passing along the vertexID currently does not call _cloudSave.Save <see cref="CloudSaving"/> as where it is passed here in 
    /// <see cref="HeavyEquipmentModule"/> has the call to <see cref="GatherAllInspectableElements(List{InspectableElement})"/> directly after which triggers the save.
    /// </summary>
    /// <param name="vertexID"></param>
    public void UpdateVertexID(int vertexID)
    {
        _saveData.vertexID = vertexID;
    }

    /// <summary>
    /// A callback function that uses the save point vertex ID
    /// </summary>
    /// <param name="callback"></param>
    public void OnLoadSetSavePointVertexID(Action<int> callback)
    {
        OnLoadOpenEngineCover();
        OnLoadToggleLights();
        OnLoadStartEngineSound();
        OnLoadSetDebrisGroups();
        callback(_saveData.vertexID);
    }

    /// <summary>
    /// Creates a list of inspectable elements from the saved list of prefab names, this is done by loading the prefab for the corresponding saved name 
    /// it will then pass that information along via a unity event.
    /// </summary>
    public void LoadElementsFromList()
    {
        List<InspectableElement> elements = new();
        foreach (string savedElement in _saveData.badElements)
        {
            GameObject elementObject = (GameObject)Resources.Load("InspectablePrefabs/InspectableElements/" + savedElement);
            elements.Add(elementObject.GetComponent<InspectableElement>());
        }

        _onLoadBadElements?.Invoke(elements);
    }

    /// <summary>
    /// This method exists to communicate the outcome of a load attempt via unity events, it is called from <see cref="CloudSaving"/> which does the actual loading.
    /// </summary>
    /// <param name="success"></param>
    public void WasLoadSuccessful(bool success)
    {
        if (success)
        {
            LoadElementsFromList(); 
        }
        _onLoadEvent?.Invoke(success);
    }

    /// <summary>
    /// This method take the backhoe states from save data then it invoke a callback function to use the backhoe states list.
    /// </summary>
    /// <param name="callback">A callback function to use the backhoe states list</param>
    public void LoadBackhoeState(Action<List<Tuple<string, int>>> callback)
    {
        List<ProgressData> allElements = _saveData.AllElements;

        if (allElements == null) return;

        List<Tuple<string, int>> backhoeStates = new();

        for (int i = 0; i < allElements.Count; i++)
        {
            backhoeStates.Add(new Tuple<string, int>(allElements[i].ElementName, allElements[i].StateIdentifier));
        }
        callback.Invoke(backhoeStates);
    }

    /// <summary>
    /// This method takes an initialized InspectableElement list and load it with saved progress data.
    /// Then it invoke a callback function to use the loaded list.
    /// </summary>
    /// <param name="callback">A callback function to use the loaded InspectableElement list</param>
    /// <param name="elements">Initialized InspectableElement list</param>
    public void LoadSaveToInspectableElements(Action<List<InspectableElement>> callback, List<InspectableElement> elements)
    {
        List<ProgressData> AllElements = _saveData.AllElements;

        if (AllElements == null) return;

        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].CurrentStatus = (InspectableElement.Status)AllElements[i].CurrentStatus;
            elements[i].MakeUserSelection(AllElements[i].UserSelectedIdentifier);
            elements[i].State = (InspectableState)AllElements[i].State;
            elements[i].SetCurrentlyInspectable(AllElements[i].currentlyInspectable);
        }
        callback.Invoke(elements);
    }

    public void OnLoadToggleLights()
    {
        //_backhoeLightController = FindObjectOfType<BackhoeLightController>();
        if (_saveData.vertexID > 60)
        {
            _backhoeController.BackhoeLightController.ToggleLights(BackhoeLightController.Light.Front);
            _backhoeController.BackhoeLightController.ToggleLights(BackhoeLightController.Light.Rear);
            _backhoeController.BackhoeLightController.ToggleLights(BackhoeLightController.Light.Indicator);
            _backhoeController.BackhoeLightController.ToggleLights(BackhoeLightController.Light.Top);
        }

    }

    public void OnLoadStartEngineSound()
    {
        if(_saveData.vertexID > 60)
        {
            _backhoeController.EngineSound.StartEngine();
        }
    }

    public void OnLoadOpenEngineCover()
    {
        if (_saveData.vertexID == 41)
        {
            if (!_backhoeController.FrontHood.IsOpen) _backhoeController.FrontHood.ToggleHoodOpen(0);
            if (!_backhoeController.LeftSidePanel.IsOpen) _backhoeController.LeftSidePanel.TogglePanelOpen();
        }
    }

    public void SaveDebrisGroupStates()
    {
        List<RemovableDebrisGroup> removableDebrisGroups = _removableDebrisGroups.GetComponentsInChildren<RemovableDebrisGroup>().ToList();

        if (removableDebrisGroups == null) return;

        List<bool> debrisStateList = new();

        //if no save data for debris, just read the states from RemovableDebrisGroup
        if (_saveData.debrisGroupStates.Count == 0)
        {
            for (int i = 0; i < removableDebrisGroups.Count; i++)
            {
                debrisStateList.Add(removableDebrisGroups[i].AllCleared);
            }
        }
        else
        {
            for (int i = 0; i < removableDebrisGroups.Count; i++)
            {
                //if save file said it has been cleared, then it is correct, else read states from RemovableDebrisGroup
                if (_saveData.debrisGroupStates[i] == true)
                    debrisStateList.Add(true);
                else
                    debrisStateList.Add(removableDebrisGroups[i].AllCleared);
            }
        }

        _saveData.debrisGroupStates = debrisStateList;
    }

    public void OnLoadSetDebrisGroups()
    {
        if(_saveData.debrisGroupStates.Count == 0) return;

        List<RemovableDebrisGroup> removableDebrisGroups = _removableDebrisGroups.GetComponentsInChildren<RemovableDebrisGroup>().ToList();

        for (int i = 0; i < removableDebrisGroups.Count; i++)
        {
            if (_saveData.debrisGroupStates[i] == true)
            {
                removableDebrisGroups[i].DebrisList.ForEach(x => x.gameObject.SetActive(false));
            }
        }
    }

    public void DeleteSave()
    {
        _saveData.vertexID = 0;
        _saveData.debrisGroupStates.Clear();
        _cloudSave.Delete();
    }
}
