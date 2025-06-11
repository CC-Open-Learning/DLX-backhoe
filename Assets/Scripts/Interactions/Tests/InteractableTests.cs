using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using RemoteEducation.Interactions;
using Flags = RemoteEducation.Interactions.Interactable.Flags;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RemoteEducation.Interactions.Tests
{
    public delegate bool ExternalIsEnabled();

    public class InteractableTests
    {
        private const float TIMESCALE = 30f;
        private GameObject audioListener;

        private EventSystem eventSystem;
        private Camera camera;

        private TestSelectable Selectable_A;
        private TestSelectable Selectable_B;
        private TestSemiSelectable Semi_A;
        private TestSemiSelectable Semi_B;

        #region Setup/TearDown

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // I run once before the first test in this class runs!

            // This prevents console messages every frame about not having an AudioListener component.
            (audioListener = new GameObject("Audio Listener Dummy")).AddComponent<AudioListener>();
            AudioListener.volume = 0f;

            eventSystem = CreateObject<EventSystem>();

            (camera = new GameObject().AddComponent<Camera>()).gameObject.tag = "MainCamera";

            Time.timeScale = TIMESCALE;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // I run once after the last test in this class runs!
            UnityEngine.Object.Destroy(audioListener);
            UnityEngine.Object.Destroy(eventSystem);
            UnityEngine.Object.Destroy(camera);
            Time.timeScale = 1f;
        }

        [SetUp]
        public void Setup()
        {
            Selectable_A = CreateObject<TestSelectable>();
            Selectable_B = CreateObject<TestSelectable>();
            Semi_A = CreateObject<TestSemiSelectable>();
            Semi_B = CreateObject<TestSemiSelectable>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.Destroy(Selectable_A);
            UnityEngine.Object.Destroy(Selectable_B);
            UnityEngine.Object.Destroy(Semi_A);
            UnityEngine.Object.Destroy(Semi_B);
            
            Interactable.ResetSubsetActivatedObjects();
        }

        private T CreateObject<T>() where T : MonoBehaviour
        {
            return new GameObject().AddComponent<T>();
        }

        #endregion

        #region Simulate Mouse Actions

        /// <summary>
        /// Simulate a full click on an interactable object.
        /// These methods are called in the same order that they are when an actual click happens.
        /// </summary>
        /// <param name="interactable">The interactable to simulate the click on.</param>
        private void EnterClickLeave(Interactable interactable)
        {
            interactable.OnMouseEnter();
            interactable.OnMouseDown();
            interactable.OnMouseUpAsButton();
            interactable.OnMouseUp();
            interactable.OnMouseExit();
        }

        #endregion

        #region Tests

        /// <summary>
        /// Check that clicking on a <see cref="Selectable"/> will select it.
        /// </summary>
        [Test]
        public void ClickingSelectsSelectables()
        {
            EnterClickLeave(Selectable_A);
            Assert.IsTrue(Selectable_A.IsSelected);
        }

        /// <summary>
        /// Check that clicking on a <see cref="SemiSelectable"/> doesn't select it.
        /// </summary>
        [Test]
        public void ClickingDoesntSelectSemiSelectable()
        {
            EnterClickLeave(Semi_A);
            Assert.IsTrue(!Semi_A.IsSelected);
        }

        /// <summary>
        /// Check that other objects can't be interacted with when another object is selected.
        /// </summary>
        [Test]
        public void SelectedObjectsBlocksOtherInteractions()
        {
            EnterClickLeave(Selectable_A);

            Selectable_B.OnMouseEnter();

            Assert.IsTrue(!Selectable_B.HasFlags(Flags.Highlighted));
        }

        /// <summary>
        /// Add objB to the list of Exclusive Exceptions on objA. Then when objB is selected,
        /// you should still be able to interact with objA;
        /// </summary>
        [Test]
        public void CanInteractWithObjectsInExclusiveInteractionExceptions()
        {
            Semi_A.AddExclusiveException(Selectable_A);

            Semi_A.OnSelect += () => Semi_A.ATestBool = true;

            EnterClickLeave(Selectable_A);
            EnterClickLeave(Semi_A);

            Assert.IsTrue(Semi_A.ATestBool);
        }

        /// <summary>
        /// If an object doesn't have <see cref="Flags.ExclusiveInteraction"/>, it can be interacted 
        /// with even if other objects are selected. If that object has other objects in its list of 
        /// Exclusive Exceptions, it can't be interacted with if any objects in that list are selected.
        /// Check that this works.
        /// </summary>
        [Test]
        public void ExclusiveExpectionsBlockWhenNoExclusiveInteractions()
        {
            Semi_A.RemoveFlags(Flags.ExclusiveInteraction);
            Semi_A.AddExclusiveException(Selectable_A);

            Semi_A.OnSelect += () => Semi_A.ATestBool = true;

            EnterClickLeave(Selectable_A);
            EnterClickLeave(Semi_A);

            Assert.IsFalse(Semi_A.ATestBool);
        }

        /// <summary>
        /// Check that overriding IsEnabled will be able to block interactions.
        /// </summary>
        [Test]
        public void IsEnabledBlocksSelection()
        {
            Selectable_A.SetExternalIsEnabled(() => { return Selectable_A.ATestInt > 5; });

            Selectable_A.ATestInt = 4;

            EnterClickLeave(Selectable_A);

            Assert.IsFalse(Selectable_A.IsSelected);
        }

        /// <summary>
        /// Check that if you disable a <see cref="SemiSelectable"/> in its on select event,
        /// that the highlight gets removed.
        /// </summary>
        [Test]
        public void DeactivatingWhileSelectedRemovesHighlight()
        {
            Semi_A.OnSelect += () => Semi_A.AddFlags(Flags.InteractionsDisabled);
            EnterClickLeave(Semi_A);

            Assert.IsFalse(Semi_A.HasFlags(Flags.Highlighted));
        }

        /// <summary>
        /// Check that if the subset of active object is not null, that objects in
        /// the list can be interacted with.
        /// </summary>
        [Test]
        public void SubsetAllowsInteractionOnActiveObjects()
        {
            Interactable.AddSubsetActivated(Selectable_A);

            EnterClickLeave(Selectable_A);

            Assert.IsTrue(Selectable_A.IsSelected);
        }

        /// <summary>
        /// Check that if the subset of active objects has elements, that objects not in 
        /// that list cant be interacted with.
        /// </summary>
        [Test]
        public void SubsetBlocksInteractionsOnInactiveObjects()
        {
            Interactable.AddSubsetActivated(Selectable_B);

            EnterClickLeave(Selectable_A);

            Assert.IsFalse(Selectable_A.IsSelected);
        }

        /// <summary>
        /// Check that if the subset of active objects is empty, nothing can be selected.
        /// </summary>
        [Test]
        public void SettingSubsetEmptyBlocksAllInteraction()
        {
            Semi_A.AddExclusiveException(Selectable_A);

            Interactable.SetSubsetActivated(new List<Interactable>());

            EnterClickLeave(Selectable_A);
            Semi_A.OnMouseEnter();
            Semi_A.OnMouseDown();

            Assert.IsFalse(Selectable_A.IsSelected || Semi_A.IsSelected);
        }

        /// <summary>
        /// Check that <see cref="Selectable"/> is still an abstract class.
        /// </summary>
        [Test]
        public void CannotAddSelectableToGameObject()
        {
            Assert.IsNull(Selectable_A.gameObject.AddComponent<Selectable>());
        }

        /// <summary>
        /// Check that <see cref="SemiSelectable"/> is still an abstract class.
        /// </summary>
        [Test]
        public void CannotAddSemiSelectableToGameObject()
        {
            Assert.IsNull(Selectable_A.gameObject.AddComponent<SemiSelectable>());
        }

        /// <summary>
        /// Check that <see cref="Interactable"/> is still an abstract class.
        /// </summary>
        [Test]
        public void CannotAddInteractableToGameObject()
        {
            Assert.IsNull(Selectable_A.gameObject.AddComponent<Interactable>());
        }



        #endregion
    }
}