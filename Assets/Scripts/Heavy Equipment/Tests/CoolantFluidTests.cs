/*
 * FILE: CoolantFluidTests.cs
 * SUMMARY:
 *  Defines the functional Unit Tests for the CoolantFluid.cs
 */

using NUnit.Framework;
using RemoteEducation.Modules.HeavyEquipment;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;

public class CoolantFluidTests
{
    /// <value>The test timescale to decrease the wait delay of each test</value>
    private const float TIMESCALE = 30f;

    /// <value>An AudioListener instance to eliminate warnings</value>
    private GameObject audioListener;

    /// <value>An instance of the CoolantFluid to test</value>
    private CoolantFluid clFluid;

    AsyncOperationHandle<GameObject> clFluidHandle;

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
        clFluidHandle = Addressables.InstantiateAsync("CoolantReservoir");
        yield return clFluidHandle;
    }

    [SetUp]
    public void Setup()
    {
        // Instantiates the test coolant fluid
        clFluid = clFluidHandle.Result.GetComponentInChildren<CoolantFluid>();
        clFluid.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        // Deletes the instance of the test coolant fluid to prevent duplication
        Addressables.ReleaseInstance(clFluidHandle);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // I run once after the last test in this class runs!
        GameObject.Destroy(audioListener);
        Time.timeScale = 1f;
    }

    [Test]
    public void SetFluidState_Full_Clean()
    {
        clFluid.SetFluidState(true, true);

        Assert.AreEqual(clFluid.CurrentFluidState, CoolantFluid.FluidStates.FullClean);
    }

    [Test]
    public void SetFluidState_Full_Dirty()
    {
        clFluid.SetFluidState(true, false);

        Assert.AreEqual(clFluid.CurrentFluidState, CoolantFluid.FluidStates.FullDirty);
    }

    [Test]
    public void SetFluidState_Low_Clean()
    {
        clFluid.SetFluidState(false, true);

        Assert.AreEqual(clFluid.CurrentFluidState, CoolantFluid.FluidStates.LowClean);
    }

    [Test]
    public void SetFluidState_Low_Dirty()
    {
        clFluid.SetFluidState(false, false);

        Assert.AreEqual(clFluid.CurrentFluidState, CoolantFluid.FluidStates.LowDirty);
    }

    [Test]
    public void FillFluid_LowDirty()
    {
        // Set currentFluidState to LowDirty
        clFluid.SetFluidState(false, false);

        clFluid.FillFluid();

        Assert.AreEqual(clFluid.CurrentFluidState, CoolantFluid.FluidStates.FullDirty);
    }

    [Test]
    public void FillFluid_LowClean()
    {
        // Set currentFluidState to LowClean
        clFluid.SetFluidState(false, true);

        clFluid.FillFluid();

        Assert.AreEqual(clFluid.CurrentFluidState, CoolantFluid.FluidStates.FullClean);
    }

    [Test]
    public void CheckTask_Bool()
    {
        // Set currentFluidState to LowDirty
        clFluid.SetFluidState(false, false);

        clFluid.CheckTask(2, null);

        Assert.IsFalse(clFluid.IsFluidFull);
    }

    [Test]
    public void CheckTask_Invalid()
    {
        Assert.IsNull(clFluid.CheckTask(0, null), null);
        LogAssert.Expect(LogType.Error, "CoolantFluid was passed a check type it could not handle");
    }
}
