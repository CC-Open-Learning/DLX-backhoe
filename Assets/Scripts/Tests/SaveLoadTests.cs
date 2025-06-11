using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VARLab.CloudSave;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using NUnit.Framework.Constraints;
using UnityEngine.Events;
using UnityEditor;

/// <summary>
/// Alot of the setup was taken from InspectableUnitTests.cs as I was unfamiliar with how to setup an inspectable object
/// </summary>
public class SaveLoadTests
{
    //Properties stolen from InspectableUnitTests.cs
    private const float TIMESCALE = 30f;
    private GameObject audioListener;

    private InspectableManager inspectableManager;


    private AsyncOperationHandle<GameObject> inspectableObjectHandle; //InspectableTestingObject
    private InspectableObject inspectableObject;

    private AsyncOperationHandle<GameObject> elementPrefabsHandle; //InspectableElementTestPrefabHolder
    private ElementPrefabHolder elementPrefabs;

    //Properties for Save/Load
    GameObject SaveObj;
    SaveData _SaveData;
    SaveDataSupport SaveSupport;
    TestCloudSave CloudSave;
    List<InspectableElement> InspectableElements;

    #region setup
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SaveObj = new();
        //add components for the save obj
        SaveObj.AddComponent<AzureSaveSystem>();
        CloudSave = SaveObj.AddComponent<TestCloudSave>();
        _SaveData = SaveObj.AddComponent<SaveData>();

        SaveSupport = SaveObj.AddComponent<SaveDataSupport>();
        var so = new SerializedObject(SaveSupport);
        so.FindProperty("_saveData").objectReferenceValue = _SaveData;
        so.ApplyModifiedProperties();

        SaveObj.SetActive(true);

        // This prevents console messages every frame about not having an AudioListener component.
        (audioListener = new GameObject("Audio Listener Dummy")).AddComponent<AudioListener>();
        AudioListener.volume = 0f;

        Time.timeScale = TIMESCALE;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // I run once after the last test in this class runs!
        GameObject.Destroy(audioListener);
        Time.timeScale = 1f;
    }

    [UnitySetUp]
    public IEnumerator UnitySetup()
    {
        inspectableObjectHandle = Addressables.InstantiateAsync("InspectableTestingObject");
        yield return inspectableObjectHandle;

        elementPrefabsHandle = Addressables.InstantiateAsync("InspectableElementTestPrefabHolder");
        yield return elementPrefabsHandle;
    }

    [SetUp]
    public void Setup()
    {
        inspectableManager = new GameObject().AddComponent<InspectableManager>();
        inspectableManager.InspectableObject = inspectableObjectHandle.Result.GetComponent<InspectableObject>();
        inspectableManager.onStoreBadElements = new();

        elementPrefabs = elementPrefabsHandle.Result.GetComponent<ElementPrefabHolder>();

        inspectableManager.onStoreBadElements.AddListener(SaveSupport.GetElementsFromControllerList);
        inspectableManager.Initialize(30);
    }

    [TearDown]
    public void TearDown()
    {
        Addressables.ReleaseInstance(inspectableObjectHandle);
        Addressables.ReleaseInstance(elementPrefabsHandle);
    }

    #endregion

    /// <summary>
    /// This test is to check the SaveData component is getting the information properly set.
    /// Method: <see cref="SaveDataSupport.GetElementsFromControllerList(Dictionary{InspectableController, List{InspectableElement}})"/>
    /// Input: Dictionary of InspectableControllers,List(InspectableElement), this is triggered in the InspectableManager <see cref="InspectableManager.InitializeControllers(float)"/>
    /// output: SaveData component of SaveObj will be updated with a List of strings containing the prefab names
    /// </summary>
    [UnityTest]
    public IEnumerator TestGetElementsFromController()
    {
        yield return null;
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        
        Assert.IsNotNull(_SaveData.badElements);
    }

    /// <summary>
    /// This test is to check the SaveData component is getting the list of inspectables
    /// Method: <see cref="SaveDataSupport.GatherAllInspectableElements(List{InspectableElement})"/>
    /// Input: The list of inspectable elements from the inspectable manager
    /// Output: SaveData component of SaveObj will be updated with a dictionary or our ProgressData
    /// </summary> 
    [Test]
    public void TestGetAllElements()
    {
        SaveSupport.GatherAllInspectableElements(inspectableManager.InspectableElements);

        Assert.IsNotNull(_SaveData.AllElements);
    }

    /// <summary>
    /// This test is simulating a load on start up has triggered and based on the list of elements given it can load the prefabs for the assigned element.
    /// </summary>
    [Test]
    public void TestLoadElements()
    {
        //known prefabs available, if we use the ones from the test prefab that were saved in the above tests the Prefabs will not load
        List<string> elements = new List<string> { "IE_Piston_Leaking", "IE_Piston_Damaged", "IE_Piston_Damaged", "IE_Piston_Damaged" };
        _SaveData.badElements = elements;
        //setup listener
        SaveSupport._onLoadBadElements = new();
        SaveSupport._onLoadBadElements.AddListener(ReturnElements);

        SaveSupport.LoadElementsFromList();

        Assert.IsNotNull(InspectableElements);
    }

    /// <summary>
    /// Support function for the <see cref="TestLoadElements"/> test, added as a callback for  unity event
    /// </summary>
    /// <param name="elements"></param>
    public void ReturnElements(List<InspectableElement> elements)
    { 
        InspectableElements = elements;
        return;
    }
}

/// <summary>
/// This is pulled from another test folder, not referenced because of a cyclic dependency so I'm just slapping it down here
/// </summary>
public class ElementPrefabHolder : MonoBehaviour
{
    public InspectableElement LightsGood;
    public InspectableElement LightsMissing;
    public InspectableElement LightsBurntOut;

    public InspectableElement WheelsGood;
    public InspectableElement WheelsFlat;
    public InspectableElement FakeElementWheelsMissing;
}
