using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using RemoteEducation.Interactions;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace RemoteEducation.Interactions.Tests
{
    /// <summary>
    /// Tests for the <see cref="AuxiliaryHighlight"/> system
    /// </summary>
    public class AuxiliaryHighlightTests
    {

        private TestSelectable selectable;
        private AuxiliaryHighlight auxHighlight;

        private EventSystem eventSystem;
        private Camera camera;

        private HighlightObject highlight;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            eventSystem = new GameObject().AddComponent<EventSystem>();

            (camera = new GameObject().AddComponent<Camera>()).gameObject.tag = "MainCamera";
            HighlightObject.CreateHighlightCamera(camera);
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            //create an object that can be highlighted.
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            selectable = gameObject.AddComponent<TestSelectable>();

            auxHighlight = AuxiliaryHighlight.AddAuxiliaryHighlight(gameObject, Color.red);
            auxHighlight.Attach();

            //let everything finish setting up so that we can start highlighting objects
            yield return new WaitForSeconds(0.5f);

            highlight = selectable.gameObject.GetComponent<HighlightObject>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(selectable.gameObject);

            Interactable.ResetSubsetActivatedObjects();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.Destroy(eventSystem.gameObject);
            Object.Destroy(camera.gameObject);
        }

        /// <summary>
        /// Test that the auxiliary highlight can be applied
        /// </summary>
        [Test]
        public void AuxHighlightApplysCorrectly()
        {
            auxHighlight.EnableAuxilaryHighlight();
            Assert.IsTrue(auxHighlight.HighlightIsAuxiliary && highlight.IsHighlighted);
        }

        /// <summary>
        /// Test that the auxiliary highlight can be removed
        /// </summary>
        [Test]
        public void AuxHighlightRemovesCorrectly()
        {
            auxHighlight.EnableAuxilaryHighlight();
            auxHighlight.DisableAuxilaryHighlight();
            Assert.IsTrue(!highlight.IsHighlighted);
        }

        /// <summary>
        /// If you enable the auxiliary highlight when the object is already highlighted,
        /// the aux highlight should not override the highlight.
        /// </summary>
        [Test]
        public void IfObjectHighlightedAuxHighlightWaits()
        {
            selectable.OnMouseEnter();
            selectable.OnMouseDown();
            selectable.OnMouseUpAsButton();

            auxHighlight.EnableAuxilaryHighlight();

            Assert.IsTrue(selectable.HasFlags(Interactable.Flags.Highlighted) && highlight.IsHighlighted && !auxHighlight.HighlightIsAuxiliary);
        }

        /// <summary>
        /// If you enable the aux highlight when the object is already highlighted,
        /// the aux highlight should not override the highlight.
        /// </summary>
        [Test]
        public void AuxHighlightAppliesAfterDefaultIsGone()
        {
            selectable.OnMouseEnter();
            selectable.OnMouseDown();
            selectable.OnMouseUpAsButton();

            auxHighlight.EnableAuxilaryHighlight();

            Interactable.UnselectCurrentSelections();

            Assert.IsTrue(auxHighlight.HighlightIsAuxiliary && highlight.IsHighlighted);
        }

        /// <summary>
        /// Check that you can remove the auxiliary highlight while the default highlight is on.
        /// </summary>
        [Test]
        public void AuxHighlightCanBeRemovedWithDefualtHighlightOn()
        {
            auxHighlight.EnableAuxilaryHighlight();

            selectable.OnMouseEnter();
            selectable.OnMouseDown();
            selectable.OnMouseUpAsButton();

            auxHighlight.DisableAuxilaryHighlight();

            Interactable.UnselectCurrentSelections();

            Assert.IsTrue(!auxHighlight.HighlightIsAuxiliary && !highlight.IsHighlighted);
        }

        /// <summary>
        /// Check that the auxiliary highlight will stay turned on after the
        /// default highlight is turn on and then off again.
        /// </summary>
        [Test]
        public void DefaultHighlightRemainsAfterAuxIsDisabled()
        {
            auxHighlight.EnableAuxilaryHighlight();

            selectable.OnMouseEnter();
            selectable.OnMouseDown();
            selectable.OnMouseUpAsButton();

            Interactable.UnselectCurrentSelections();

            Assert.IsTrue(auxHighlight.HighlightIsAuxiliary && highlight.IsHighlighted);
        }
    }
}