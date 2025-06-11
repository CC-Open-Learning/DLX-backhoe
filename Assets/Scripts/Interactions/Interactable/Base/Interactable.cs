using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RemoteEducation.Interactions
{
    public abstract partial class Interactable : MonoBehaviour
    {
        /// <summary>Highlighting implementation to use for this <see cref="Interactable"/>.</summary>
        public IHighlight highlight;

        [Tooltip("Called when a full mouse click (down & up) is performed while the mouse is over this object.")]
        public Action OnClick;
        [Tooltip("Called when the object is selected.")]
        public Action OnSelect;
        [Tooltip("Called when the object is deselected.")]
        public Action OnDeselect;

        /// <summary><see cref="Interactable"/> objects that are currently selected.</summary>
        public static List<Interactable> CurrentSelections { get; protected set; }

        /// <summary>
        /// A list of object that are currently able to be activated.</summary>
        /// <remarks>
        /// If null, all objects are able to be activated. If not null, only objects in the list can be activated.</remarks>
        private static List<Interactable> subsetActiveObjects;

        /// <summary>The <see cref="Interactable"/> the mouse is currently hovering over.</summary>
        /// <remarks>Used for <see cref="UpdateHoveringOnUITransition(bool)"/>.</remarks>
        public static Interactable HoveringOverInteractable { get; protected set; }

        public enum SelectionModes { Mouse, Key, Auxiliary, Internal }

        /// <summary>Exceptions for this object. See <see cref="AddExclusiveException(Interactable)"/> for more details.</summary>
        public List<Interactable> ExclusiveInteractionExceptions { get; set; }

        /// <summary>If this object has any interaction exceptions.</summary>
        /// <value><see langword="true"/> if <see cref="ExclusiveInteractionExceptions"/> contains any elements.</value>
        public bool HasExceptions { get { return ExclusiveInteractionExceptions != null && ExclusiveInteractionExceptions.Count > 0; } }

        static Interactable()
        {
            CurrentSelections = new List<Interactable>();
        }

        /// <summary>Fires when the user's mouse enters the collider after the <see cref="Flags.MouseOver"/> flag has been set.</summary>
        protected virtual void MouseEnter()
        {
            Highlight(true, HighlightObject.HIGHLIGHT_COLOUR);
            UpdateCursor();
        }

        /// <summary>Fires when the user's mouse exits the collider after the <see cref="Flags.MouseOver"/> flag has been removed.</summary>
        protected virtual void MouseExit()
        {
            Highlight(false, Color.black);
            UpdateCursor();
        }

        /// <summary>Fires when the user presses their mouse button after the <see cref="Flags.MouseDown"/> flag has been set.</summary>
        protected virtual void MouseDown()
        {
            UpdateCursor();
        }

        /// <summary>Fires when the user releases their mouse button while the mouse is still on the object's collider.</summary>
        protected virtual void MouseClick()
        {
            OnClick?.Invoke();
        }

        /// <summary>Fires when the user releases their mouse button after the <see cref="Flags.MouseDown"/> flag has been removed.</summary>
        protected virtual void MouseUp()
        {
            UpdateCursor();
        }

        /// <summary>Logic to run when a user selects this <see cref="Interactable"/>.</summary>
        public virtual void Select(bool select, SelectionModes selectionMode)
        {
            if (selectionMode != SelectionModes.Mouse)
            {
                RefreshMouseOnSelect(select, this);
            }
        }

        /// <summary>Enables or disables the highlight for this object if it can be highlighted.</summary>
        /// <param name="arg">Argument to pass into the highlighting interface.</param>
        /// <param name="enable">If <see langword="true"/>, enables the highlight on the object. If <see langword="false"/>, disables the highlight.</param>
        /// <remarks>The specific implementation of <see cref="highlight"/> must handle converting <paramref name="arg"/> into the format required for its <see cref="IHighlight.Highlight(bool)"/> function.</remarks>
        public virtual void Highlight(bool enable, object arg = null)
        {
            if (HasFlags(Flags.Highlightable))
            {
                if (arg != null)
                    highlight.arg = arg;

                highlight.Highlight(enable);

                ToggleFlags(enable, Flags.Highlighted);
            }
        }

        /// <summary>Enables or disables interactions on this <see cref="Interactable"/>.</summary>
        /// <remarks>Sets <see cref="Flags.InteractionsDisabled"/> as needed, handles deselecting objects if interactions are disabled.</remarks>
        /// <param name="disable">If the object should be disabled</param>
        public virtual void ToggleInteractionsDisabled(bool disable)
        {
            if (disable && HasFlags(Flags.Selected))
            {
                Select(false, SelectionModes.Internal);
            }

            ToggleFlags(disable, Flags.InteractionsDisabled);
        }

        /// <summary>Updates the user's <see cref="Cursor"/> based on the state of this <see cref="Interactable"/>.</summary>
        public virtual void UpdateCursor()
        {
            if (HasFlags(Flags.CustomCursors))
            {
                if (HasFlags(Flags.MouseDown))
                {
                    CursorIcons.SetCursor(CursorIcons.Cursors.HandClosed);
                }
                else if (HasFlags(Flags.MouseOver))
                {
                    CursorIcons.SetCursor(CursorIcons.Cursors.HandOpen);
                }
                else
                {
                    CursorIcons.SetCursor(CursorIcons.Cursors.HandPointer);
                }
            }
        }

        /// <summary>Checks if this <see cref="Interactable"/> is being blocked from selection by any of the currently selected <see cref="Interactable"/> objects.</summary>
        /// <returns><see langword="true"/> if this object is being blocked.</returns>
        protected bool IsBlockedByCurrentSelections()
        {
            if (CurrentSelections.Count > 0)
            {
                var a = HasFlags(Flags.ExclusiveInteraction);

                if (!HasExceptions)
                {
                    return a;
                }

                bool b;

                foreach (Interactable current in CurrentSelections)
                {
                    b = ExclusiveInteractionExceptions.Contains(current);

                    if ((a && !b) || (!a && b))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>Checks if two <see cref="Interactable"/> objects, <paramref name="a"/> and <paramref name="b"/>, do not block each other's interactions.</summary>
        /// <param name="a">The base object</param>
        /// <param name="b">Checked if it can be interacted with while <paramref name="a"/> is selected.</param>
        /// <returns><see langword="true"/> if both objects can be interacted with simultaneously.</returns>
        public static bool CanInteractTogether(Interactable a, Interactable b)
        {
            if (b.HasFlags(Flags.ExclusiveInteraction))
            {
                return b.HasExceptions && b.ExclusiveInteractionExceptions.Contains(a);
            }
            else
            {
                return !(b.HasExceptions && b.ExclusiveInteractionExceptions.Contains(a));
            }
        }

        /// <summary> Add an <see cref="Interactable"/> to this object's <see cref="ExclusiveInteractionExceptions"/>.</summary>
        /// <remarks>This list will function differently based on whether <see cref="Flags.ExclusiveInteraction"/> is set.
        /// <para />
        /// If <see cref="Flags.ExclusiveInteraction"/> is set:
        /// This <see cref="Interactable"/> will not be able to be interacted with if any other <see cref="Interactable"/> objects
        /// are already selected, UNLESS those selected objects are in this object's <see cref="ExclusiveInteractionExceptions"/>.
        /// <para />
        /// If <see cref="Flags.ExclusiveInteraction"/> is NOT set:
        /// This <see cref="Interactable"/> will be able to be interacted with even if there are other <see cref="Interactable"/> objects
        /// selected, UNLESS any of the selected interactables are part of this list.</remarks>
        /// <param name="interactable">The interactable to be the exception</param>
        public void AddExclusiveException(Interactable interactable)
        {
            if(ExclusiveInteractionExceptions == null)
            {
                ExclusiveInteractionExceptions = new List<Interactable>() { interactable };
            }
            else
            {
                ExclusiveInteractionExceptions.Add(interactable);
            }
        }

        /// <summary> Called every time the mouse is released.
        /// <see cref="Selectable"/> will check if it can unselect anything.
        /// The object under the mouse may also be updated</summary>
        internal static void GeneralMouseUp()
        {
            Selectable.HandleAllMouseUps();

            PointerUpCheck();
        }

        /// <summary>Handles the case for clicking and dragging on a <see cref="SemiSelectable"/> and releasing the mouse over another <see cref="Interactable"/>. </summary>
        public static void PointerUpCheck()
        {
            Interactable underMouse = InputManager.GetComponentUnderMouse<Interactable>();

            underMouse?.OnMouseEnter();
        }

        /// <summary>Handles the case of if an <see cref="Interactable"/> is over another <see cref="Interactable"/> but then moves over a UI element.</summary>
        /// <remarks>Also handles when the mouse leaves a UI object directly onto an <see cref="Interactable"/>.</remarks>
        /// <param name="mouseNowOverUI"><see langword="true"/> if the mouse just moved onto a UI element, <see langword="false"/> if the mouse just moved off a UI element.</param>
        public static void UpdateHoveringOnUITransition(bool mouseNowOverUI)
        {
            //The mouse just started hovering over a UI object
            if (mouseNowOverUI)
            {
                HoveringOverInteractable?.OnMouseExit();
            }
            else //The mouse just left a UI object
            {
                Interactable underMouse = InputManager.GetComponentUnderMouse<Interactable>();

                underMouse?.OnMouseEnter();
            }
        }

        /// <summary>Update the state of the <see cref="Interactable"/> under the mouse when a selection happens if one exists.</summary>
        /// <remarks>If the object under the mouse has <see cref="Flags.ExclusiveInteraction"/>, then selecting something else should make the object under the mouse lose its highlight.
        /// When an object becomes unselected, check if there is anything under the mouse. That object may have been blocked when the mouse originally moved over it.</remarks>
        /// <param name="selectJustHappened">If the reason that this method was called was because something was selected. <see langword="false"/> means that something was unselected.</param>
        protected static void RefreshMouseOnSelect(bool selectJustHappened, Interactable interactable)
        {
            if (selectJustHappened)
            {
                if (HoveringOverInteractable != null && !CanInteractTogether(interactable, HoveringOverInteractable))
                {
                    HoveringOverInteractable.OnMouseExit();
                }
            }
            else
            {
                PointerUpCheck();
            }
        }

        /// <summary>Unselects all currently selected <see cref="Interactable"/> objects.</summary>
        public static void UnselectCurrentSelections()
        {
            for(int i = CurrentSelections.Count - 1; i >= 0; i--)
            {
                CurrentSelections[i].Select(false, SelectionModes.Internal);
            }
        }
    }
}
