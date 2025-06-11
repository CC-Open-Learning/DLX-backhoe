/*
 * FILE: WheelTests.cs
 * SUMMARY:
 *  Defines the functional Unit Tests for Wheel.cs
 */

using NUnit.Framework;
using RemoteEducation.Localization;
using RemoteEducation.Modules.HeavyEquipment;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI.Tooltip;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;

public class WheelTests
{
    /// <value>The test timescale to decrease the wait delay of each test</value>
    private const float TIMESCALE = 30f;

    /// <value>An AudioListener instance to eliminate warnings</value>
    private GameObject audioListener;

    /// <value>An instance of the Wheel to test</value>
    private Wheel wheel;

    AsyncOperationHandle<GameObject> wheelHandle;

    // Wheel break modes
    private enum WheelBreakMode
    {
        Deflated = 0,
        Loose = 1,
        DamagedRim = 2
    }

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
        wheelHandle = Addressables.InstantiateAsync("Wheels");
        yield return wheelHandle;
    }

    [SetUp]
    public void Setup()
    {
        // Instantiates the test the Wheel
        wheel = wheelHandle.Result.GetComponentInChildren<Wheel>();
    }

    [TearDown]
    public void TearDown()
    {
        // Deletes the instance of the test wheel to prevent duplication
        Addressables.ReleaseInstance(wheelHandle);
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
        GameObject gameObj = new GameObject();
        DynamicInspectableElement newEle = gameObj.AddComponent<DynamicInspectableElement>();
        
        wheel.AttachInspectable(newEle, false, 0);

        bool result = true;

        foreach (WheelNut n in wheel.AllWheelNuts.AllNuts)
        {
            if (n.looseNut)
            {
                result = false;
            }
        }

        Assert.IsTrue(result);
    }

    [Test]
    public void AttachInspectable_SetupToolTips_Success()
    {
        GameObject gameObj = new GameObject();
        DynamicInspectableElement newEle = gameObj.AddComponent<DynamicInspectableElement>();
        
        wheel.AttachInspectable(newEle, false, 0);

        bool result = true;
        const string NUT_TOOLTIP_TEXT = "HeavyEquipment.WheelNutsTooltip";

        foreach (WheelNut n in wheel.AllWheelNuts.AllNuts)
        {
            if (n.gameObject.GetComponent<SimpleTooltip>().tooltipText != NUT_TOOLTIP_TEXT.Localize())
            {
                result = false;
            }
        }

        Assert.IsTrue(result);
    }

    [Test]
    public void AttachInspectable_SetupInteractable_Success()
    {
        GameObject gameObj = new GameObject();
        DynamicInspectableElement newEle = gameObj.AddComponent<DynamicInspectableElement>();
        
        wheel.AttachInspectable(newEle, false, 0);

        bool result = true;
        
        foreach (WheelNut n in wheel.AllWheelNuts.AllNuts)
        {
            if (n.WheelInteractable != newEle)
            {
                result = false;
            }
        }

        Assert.IsTrue(result);
    }

    [Test]
    public void AttachInspectable_Broken_Deflated()
    {
        GameObject gameObj = new GameObject();
        DynamicInspectableElement newEle = gameObj.AddComponent<DynamicInspectableElement>();

        wheel.AttachInspectable(newEle, true, (int)WheelBreakMode.Deflated);

        Assert.AreEqual(wheel.TireMeshFilter.sharedMesh, wheel.DeflatedTire);
    }

    [Test]
    public void AttachInspectable_Broken_Loose()
    {
        GameObject gameObj = new GameObject();
        DynamicInspectableElement newEle = gameObj.AddComponent<DynamicInspectableElement>();

        wheel.AttachInspectable(newEle, true, (int)WheelBreakMode.Loose);

        bool result = false;

        foreach (WheelNut n in wheel.AllWheelNuts.AllNuts)
        {
            if (n.looseNut)
            {
                result = true;
            }
        }

        Assert.IsTrue(result);
    }
}
