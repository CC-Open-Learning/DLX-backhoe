/*
 * FILE: RemovableDebrisGroupTests.cs
 * SUMMARY:
 *  Defines the functional Unit Tests for the RemovableDebrisGroup.cs
 */

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;

public class RemovableDebrisGroupTests
{
    /// <value>The test timescale to decrease the wait delay of each test</value>
    private const float TIMESCALE = 30f;

    /// <value>An AudioListener instance to eliminate warnings</value>
    private GameObject audioListener;

    /// <value>An instance of the RemovableDebrisGroup to test</value>
    private RemovableDebrisGroup rdg;

    AsyncOperationHandle<GameObject> rdgHandle;

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
        rdgHandle = Addressables.InstantiateAsync("Removable Debris");
        yield return rdgHandle;
    }

    [SetUp]
    public void Setup()
    {
        rdg = rdgHandle.Result.GetComponentInChildren<RemovableDebrisGroup>();
        rdg.AttachInspectable(null, false, 0);
        rdg.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        // Deletes the instance of the test RemovableDebrisGroup to prevent duplication
        Addressables.ReleaseInstance(rdgHandle);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // I run once after the last test in this class runs!
        GameObject.Destroy(audioListener);
        Time.timeScale = 1f;
    }

    [Test]
    public void AttachInspectable_NotBroken()
    {
        rdg.AttachInspectable(null, false, 0);

        Assert.IsTrue(rdg.AllCleared);

        bool result = true;

        foreach (RemovableDebris debris in rdg.DebrisList)
        {
            if (debris.IsCleared == false || debris.gameObject.activeSelf == true)
            {
                result = false;
            }
        }

        // Expect at all debris are cleared or not active.
        Assert.IsTrue(result);
    }

    [Test]
    public void AttachInspectable_Broken()
    {
        rdg.AttachInspectable(null, true, 0);

        Assert.IsFalse(rdg.AllCleared);

        bool result = false;

        foreach (RemovableDebris debris in rdg.DebrisList)
        {
            if (debris.IsCleared == false || debris.gameObject.activeSelf == false)
            {
                result = true;
            }
        }

        // Expect at least one debris is not cleared or not active.
        Assert.IsTrue(result);
    }

    [Test]
    public void DebrisCleared_Success()
    {
        rdg.DebrisCleared();

        Assert.IsTrue(rdg.AllCleared);
    }

    [Test]
    public void CheckTask_Bool()
    {
        Assert.AreEqual(rdg.CheckTask((int)TaskVertexManager.CheckTypes.Bool, null), rdg.AllCleared);
    }

    [Test]
    public void CheckTask_InvalidCheckType()
    {
        Assert.IsNull(rdg.CheckTask((int)TaskVertexManager.CheckTypes.Int, null), null);
        LogAssert.Expect(LogType.Error, $"Invalid check type passed into {rdg.GetType().Name}");
    }

}
