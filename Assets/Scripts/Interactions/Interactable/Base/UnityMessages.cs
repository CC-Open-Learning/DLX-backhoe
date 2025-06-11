using UnityEngine;

namespace RemoteEducation.Interactions
{
    partial class Interactable
    {
        // Please do not add <summary> comments to these functions, they inherit xml documentation from Unity's library.
        // If extra information is required, use <remarks>.

        protected virtual void Awake()
        {
            highlight = new SoftEdgeHighlight(gameObject, HighlightObject.OUTLINE_WIDTH_THIN, HighlightObject.Mode.OutlineVisible);
        }

        /// <summary>Called when the object is disabled or no longer able to be interacted with.</summary>
        /// <remarks>This happens when <see cref="Flags.InteractionsDisabled"/> is turned on.
        /// Also if the object is being interacted with when the <see cref="subsetActiveObjects"/> list is updated making the object inactive.</remarks>
        protected virtual void OnDisable()
        {
            if(highlight != null)
            {
                highlight.Highlight(false);
            }

            RemoveFlags(Flags.MouseDown | Flags.MouseOver | Flags.Selected | Flags.Highlighted);

            CurrentSelections.Remove(this);

            if(HasFlags(Flags.CustomCursors))
            {
                CursorIcons.SetCursor(CursorIcons.Cursors.HandPointer);
            }
        }

        protected virtual void OnDestroy()
        {
            CurrentSelections.Remove(this);

            if (HoveringOverInteractable == this)
            {
                HoveringOverInteractable = null;
            }
        }

        public virtual void OnMouseEnter()
        {
            if(!InputManager.MouseOverUI && (HasFlags(Flags.StartInteractMouseDown) || !Input.GetMouseButton(Constants.LEFT_MOUSE_BUTTON)))
            {
                if(IsEnabled && !HasFlags(Flags.MouseOver))
                {
                    AddFlags(Flags.MouseOver);

                    HoveringOverInteractable = this;

                    MouseEnter();
                }
            }
        }

        private void OnMouseOver()
        {
            // This had to be added for a very odd edge case:
            // If the user had their mouse over a disabled interactable it would call OnMouseEnter.
            // If the interactable was changed to be enabled during while the mouse was still over the object, it would not call OnMouseEnter again.
            // This will make sure an object set to enabled will properly catch that it has been enabled and become interactable/highlighted accordingly.
            OnMouseEnter();
        }

        public virtual void OnMouseExit()
        {
            if (HasFlags(Flags.MouseOver))
            {
                RemoveFlags(Flags.MouseOver);

                if (HoveringOverInteractable == this)
                {
                    HoveringOverInteractable = null;
                }

                MouseExit();
            }
        }

        public virtual void OnMouseDown()
        {
            // NOOP if MouseOver Flag is not set.
            // Potential pitfall for Unit Testing since OnMouseDown is a desired test function.

            if (IsEnabled && HasFlags(Flags.MouseOver))
            {
                AddFlags(Flags.MouseDown);

                if (HoveringOverInteractable == this)
                {
                    HoveringOverInteractable = null;
                }

                MouseDown();
            }
        }

        public virtual void OnMouseUpAsButton()
        {
            if (!InputManager.MouseOverUI && IsEnabled)
            {
                MouseClick();
            }
        }

        /// <remarks>This method should fire regardless of if the mouse is over a UI component.</remarks>
        public virtual void OnMouseUp()
        {
            if (HasFlags(Flags.MouseDown))
            {
                RemoveFlags(Flags.MouseDown);

                if (!HasFlags(Flags.InteractionsDisabled))
                {
                    MouseUp();
                }
            }
        }
    }
}
