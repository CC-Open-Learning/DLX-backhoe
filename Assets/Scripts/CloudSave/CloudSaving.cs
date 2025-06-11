using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VARLab.CloudSave;
using Newtonsoft.Json;
using RemoteEducation.Networking.SCORM;

/// <summary>
/// Contains the calls to trigger Save/Load/Delete functionality of the save system.
/// </summary>
public class CloudSaving : MonoBehaviour , ICloudSaving
{
    [SerializeField] private AzureSaveSystem saveSystem;

    private string _fileName = "TESTING/NOT_SET"; //This is now actually set via the LearnerID which is set to testing if in unity editor

    [SerializeField] private string filepath;

    [SerializeField] private SaveDataSupport _saveDataSupport;

    private bool _loadSuccess;

    private ICloudSerializer _serializer;

    private CoroutineWithData _load;

    private void Awake()
    {
        _serializer = new JsonCloudSerializer();
        _loadSuccess = false;
        _fileName = ScormIntegrator.LearnerId;
        filepath = "heavyequipmentbackhoe/" + _fileName + ".txt";
    }

    public void Save()
    {
        var data = _serializer.Serialize();
        saveSystem.Save(filepath, data);
    }

    public void Load()
    {
        StartCoroutine(_Load());
    }

    private IEnumerator _Load()
    {
        _load = saveSystem.Load(filepath);

        yield return _load.Routine;

        var loadedData = (string)_load.Result;

        if (loadedData != null)
        {
            _loadSuccess = true;
            _serializer.Deserialize(loadedData);
        }
        _saveDataSupport.WasLoadSuccessful(_loadSuccess);
        Debug.Log(loadedData);
    }

    public void Delete()
    {
        StartCoroutine(_Delete());
    }

    private IEnumerator _Delete()
    {
        var delete = saveSystem.Delete(filepath);

        yield return delete;

        Debug.Log("Delete finished.");
    }
}
