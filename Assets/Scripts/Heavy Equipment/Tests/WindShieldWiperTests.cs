/*
 * FILE: WindShieldWiperTests.cs
 * SUMMARY:
 *  Defines the functional Unit Tests for the WindShieldWiperController.cs
 */

using NUnit.Framework;
using RemoteEducation.Modules.HeavyEquipment;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;

public class WindShieldWiperTests
{
    /// <value>The test timescale to decrease the wait delay of each test</value>
    private const float TIMESCALE = 30f;

    /// <value>An AudioListener instance to eliminate warnings</value>
    private GameObject audioListener;

    /// <value>An instance of the WindShieldWiper to test</value>
    private WindShieldWiperController wsWiper;

    AsyncOperationHandle<GameObject> wsWiperHandle;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // I run once before the first test in this class runs!

        // This prevents console messages every frame about not having an AudioListener component.
        (audioListener = new GameObject("Audio Listener Dummy")).AddComponent<AudioListener>();
        AudioListener.volume = 0f;

        Time.timeScale = TIMESCALE;
    }

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        wsWiperHandle = Addressables.InstantiateAsync("Wiper");
        yield return wsWiperHandle;
    }

    [SetUp]
    public void Setup()
    {
        // Instantiates the test wind shield wiper
        wsWiper = wsWiperHandle.Result.GetComponentInChildren<WindShieldWiperController>();
        wsWiper.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        // Deletes the instance of the test wind shield wiper to prevent duplication
        Addressables.ReleaseInstance(wsWiperHandle);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // I run once after the last test in this class runs!
        GameObject.Destroy(audioListener);
        Time.timeScale = 1f;
    }

    [Test]
    public void Initialize_Successfully()
    {
        wsWiper.Initialize();

        Assert.IsNotNull(wsWiper.Taskable);
    }

    [Test]
    public void CheckTask_WiperIsOnOrOff()
    {
        Assert.IsNotNull(wsWiper.CheckTask((int)TaskVertexManager.CheckTypes.Bool, null));
    }

    [Test]
    public void CheckTask_WiperIsInvalid()
    {
        wsWiper.CheckTask((int)TaskVertexManager.CheckTypes.Int, null);

        //Assert.IsNull(wsWiper.CheckTask((int)TaskVertexManager.CheckTypes.Int, null));
        LogAssert.Expect(LogType.Error, "A checktype that is not defined was passed to the " + wsWiper.GetType().ToString());
    }
}
