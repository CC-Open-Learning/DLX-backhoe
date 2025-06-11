using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Linq;
using RemoteEducation.Interactions;

public class InspectableUnitTests
{
    private const float TIMESCALE = 30f;
    private GameObject audioListener;

    private InspectableManager inspectableManager;
    

    private AsyncOperationHandle<GameObject> inspectableObjectHandle; //InspectableTestingObject
    private InspectableObject inspectableObject;

    private AsyncOperationHandle<GameObject> elementPrefabsHandle; //InspectableElementTestPrefabHolder
    private ElementPrefabHolder elementPrefabs;

    #region Setup

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // I run once before the first test in this class runs!

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

        elementPrefabs = elementPrefabsHandle.Result.GetComponent<ElementPrefabHolder>();
    }

    [TearDown]
    public void TearDown()
    {
        Addressables.ReleaseInstance(inspectableObjectHandle);
        Addressables.ReleaseInstance(elementPrefabsHandle);
    }

    #endregion

    #region Tests

    /// <summary>
    /// If all the elements are created correctly, check that selecting the good option all elements
    /// means that all the inspections are correct.
    /// </summary>
    [Test]
    public void InspectionOnAllGoodElements()
    {
        inspectableManager.Initialize(0);

        foreach(InspectableElement element in inspectableManager.InspectableElements)
        {
            int goodIdentifier = element.ParentController.IdentifiersByElement[element.ParentController.ElementStates.Find(x => x.State == InspectableState.Good)];

            element.MakeUserSelection(goodIdentifier);
        }

        inspectableManager.EvaluateElements();

        bool allEvaluationsAreCorrect = true;

        foreach(InspectableElement element in inspectableManager.InspectableElements)
        {
            if(element.CurrentStatus != InspectableElement.Status.EvaluatedCorrect)
            {
                allEvaluationsAreCorrect = false;
            }
        }

        Assert.IsTrue(allEvaluationsAreCorrect);
    }

    /// <summary>
    /// Check that when you reload the controllers, that the break mode on the elements changes.
    /// </summary>
    [Test]
    public void ReloadingControllersUpdatesBreakMode()
    {
        inspectableManager.Initialize(0);

        DynamicInspectableElement dynamicBurntOut = elementPrefabs.LightsBurntOut as DynamicInspectableElement;

        List<IBreakable> breakables = inspectableManager.ReloadControllers(new List<DynamicInspectableElement>() { dynamicBurntOut });

        DummyBreakable dummyBreakable = breakables[0] as DummyBreakable;

        Assert.AreEqual(dynamicBurntOut.BadMode, dummyBreakable.CurrentBreakMode);
    }

    /// <summary>
    /// Check that the list of bad elements passed into the inspectable manager actually 
    /// get placed on the elements.
    /// </summary>
    [Test]
    public void BadElementsPlacedCorrectly()
    {
        List<InspectableElement> badElements = new List<InspectableElement>()
        {
            elementPrefabs.LightsBurntOut,
            elementPrefabs.WheelsFlat
        };

        inspectableManager.Initialize(badElements);

        bool allElementsPlaced = true;
        //for each element that was place               (as DynamicInspectableElements)
        foreach(DynamicInspectableElement badElement in from e in badElements select e as DynamicInspectableElement)
        {
            bool elementFound = false;

            //check each controller
            foreach(InspectableController controller in inspectableManager.InspectableControllers)
            {
                //check each element on the controller
                foreach(InspectableElement element in controller.InspectableElements)
                {
                    DynamicInspectableElement dynamicElement = element as DynamicInspectableElement;

                    //check if there is an element with the same values
                    if (dynamicElement.BadMode == badElement.BadMode && dynamicElement.StateMessage.CompareTo(badElement.StateMessage) == 0)
                    {
                        elementFound = true;
                        break;
                    }
                }

                if (elementFound)
                    break;
            }

            if(!elementFound)
            {
                allElementsPlaced = false;
                break;
            }
        }

        Assert.IsTrue(allElementsPlaced);
    }

    /// <summary>
    ///     Ensures that an <see cref="InspectableController"/> provided with 'fake' bad
    ///     states will not try to use those 'fake' bad states when randomly chosing
    ///     states for its <see cref="InspectableElement"/> elements.
    /// </summary>
    [Test]
    public void FakeBadStatesNotInRealStates()
    {
        // Initialize the Inspectable Manager in a completely bad state.
        // Though this test could run just as well with no bad elements
        inspectableManager.Initialize(100);

        // A 'fake' bad state "IE_Wheels_Missing_Fake" has been added to
        // the InspectableElementTestPrefabHolder
        const int expectedControllersWithFakeElements = 1;
        int controllersWithFakeElements = 0;

        // It should be the case that one of the controllers has one 'fake' bad state,
        // and that 'fake' bad state is not present in the 'RealStates' collection
        foreach (InspectableController controller in inspectableManager.InspectableControllers)
        {
            if (controller.ElementStates.Contains(elementPrefabs.FakeElementWheelsMissing)) 
            {
                ++controllersWithFakeElements;
                Assert.IsTrue(!controller.RealStates.Contains(elementPrefabs.FakeElementWheelsMissing));
            }
        }

        Assert.AreEqual(expectedControllersWithFakeElements, controllersWithFakeElements);
    }

    /// <summary>
    /// Check that all the UI events on the <see cref="InspectableManager"/> 
    /// are called correctly. This checks that the expected events are called and
    /// that no other ones are called. 
    /// This test assumes that the first controller on the test object has 2 elements.
    /// This is done so that we can more easily pass in dummy values.
    /// </summary>
    [Test]
    public void AllUIEventsAreFiredCorrectly()
    {
        //indexes for the eventsCalled array
        int OnElementSelected = 0,
            OnElementDeselected = 1,
            OnElementStateChanged = 2,
            OnActiveElementsChanged = 3,
            OnEvaluationCompleted = 4;

        bool anEventWasCalledWrong = false;

        inspectableManager.Initialize(0);

        //keep track of which events are called.
        bool[] eventsCalled = new bool[5];

        inspectableManager.OnElementSelected += (_) => eventsCalled[OnElementSelected] = true;
        inspectableManager.OnElementDeselected += (_) => eventsCalled[OnElementDeselected] = true;
        inspectableManager.OnElementStateChanged += (_) => eventsCalled[OnElementStateChanged] = true;
        inspectableManager.OnActiveElementsChanged += (_) => eventsCalled[OnActiveElementsChanged] = true;
        inspectableManager.OnEvaluationCompleted += () => eventsCalled[OnEvaluationCompleted] = true;


        inspectableManager.InspectableControllers[0].InspectableElements[0].Select(true);
        anEventWasCalledWrong = EventsCalledIncorrectly(eventsCalled, OnElementSelected);

        if (anEventWasCalledWrong)
        {
            Debug.LogError("The wrong events were called when selecting an object");
        }


        if (!anEventWasCalledWrong)
        {
            eventsCalled = new bool[5];

            inspectableManager.InspectableControllers[0].InspectableElements[0].Select(false);
            anEventWasCalledWrong = EventsCalledIncorrectly(eventsCalled, OnElementDeselected);

            if (anEventWasCalledWrong)
            {
                Debug.LogError("The wrong events were called when deselecting an object");
            }
        }

        if (!anEventWasCalledWrong)
        {
            eventsCalled = new bool[5];

            inspectableManager.InspectableControllers[0].InspectableElements[0].Select(false);
            anEventWasCalledWrong = EventsCalledIncorrectly(eventsCalled, OnElementDeselected);

            if (anEventWasCalledWrong)
            {
                Debug.LogError("The wrong events were called when deselecting an object");
            }
        }

        if (!anEventWasCalledWrong)
        {
            eventsCalled = new bool[5];

            inspectableManager.InspectableControllers[0].InspectableElements[0].Select(true);
            inspectableManager.InspectableControllers[0].InspectableElements[1].Select(true);
            anEventWasCalledWrong = EventsCalledIncorrectly(eventsCalled, OnElementSelected, OnElementDeselected);

            if (anEventWasCalledWrong)
            {
                Debug.LogError("The wrong events were called when selecting an element after another was already selected");
            }
        }

        if (!anEventWasCalledWrong)
        {
            eventsCalled = new bool[5];

            inspectableManager.InspectableControllers[0].InspectableElements[1].MakeUserSelection(0);
            anEventWasCalledWrong = EventsCalledIncorrectly(eventsCalled, OnElementStateChanged);

            if (anEventWasCalledWrong)
            {
                Debug.LogError("The wrong events were called when making an inspection on an element");
            }
        }

        if (!anEventWasCalledWrong)
        {
            eventsCalled = new bool[5];

            inspectableManager.EvaluateElements();
            anEventWasCalledWrong = EventsCalledIncorrectly(eventsCalled, OnEvaluationCompleted);

            if (anEventWasCalledWrong)
            {
                Debug.LogError("The wrong events were called when the elements were evaluated");
            }
        }

        if (!anEventWasCalledWrong)
        {
            eventsCalled = new bool[5];

            inspectableManager.UpdateActiveElements(InspectableManager.ContolStates.AllOff);
            anEventWasCalledWrong = EventsCalledIncorrectly(eventsCalled, OnActiveElementsChanged);

            if (anEventWasCalledWrong)
            {
                Debug.LogError("The wrong events were called when the active elements were updated");
            }
        }

        Assert.IsFalse(anEventWasCalledWrong);
    }

    /// <summary>
    /// Used by <see cref="AllUIEventsAreFiredCorrectly"/>.
    /// </summary>
    public bool EventsCalledIncorrectly(bool[] eventsCalled, params int[] eventsThatShouldBeCalled)
    {
        List<int> correctEvents = eventsThatShouldBeCalled.ToList();

        for(int i = 0; i < eventsCalled.Length; i++)
        {
            bool shouldBeCalled = correctEvents.Contains(i);

            if (!((eventsCalled[i] && shouldBeCalled) || (!eventsCalled[i] && !shouldBeCalled)))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}