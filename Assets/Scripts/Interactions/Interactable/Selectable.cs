using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RemoteEducation.Interactions
{
    /// <summary>Base class for objects that are selected when a full mouse click (down and up) occurs while the mouse is over the object.</summary>
    public abstract class Selectable : Interactable
    {
        private const float MOUSE_DOWN_MAX_TIME = 0.25f;

        public override bool IsEnabled
        {
            get
            {
                if(!base.IsEnabled)
                {
                    return false;
                }

                if (!IsSelected && IsBlockedByCurrentSelections())
                {
                    return false;
                }

                return true;
            }
        }

        protected override void MouseEnter()
        {
            if(IsSelected)
            {
                UpdateCursor();
            }
            else
            {
                base.MouseEnter();
            }
        }

        protected override void MouseExit()
        {
            if(!(HasFlags(Flags.MouseDown) || IsSelected))
            {
                base.MouseExit();
            }
            else
            {
                UpdateCursor();
            }
        }

        protected override void MouseClick()
        {
            base.MouseClick();

            if(!IsSelected)
            {
                Select(true, SelectionModes.Mouse);
            }
        }

        protected override void MouseUp()
        {
            if(!IsSelected && !HasFlags(Flags.MouseOver))
            {
                Highlight(false, Color.black);
            }

            base.MouseUp();
        }

        public override void Select(bool select, SelectionModes selectionMode = SelectionModes.Mouse)
        {
            //if this object is being selected, unselect all other objects other than its exceptions
            if(select && CurrentSelections.Count > 0)
            {
                List<Interactable> toUnselect = new List<Interactable>(CurrentSelections.Where(x => x is Selectable || !CanInteractTogether(this, x)).ToList());

                toUnselect.ForEach(x => x.Select(false, selectionMode));
            }

            if(select)
            {
                AddFlags(Flags.Selected);
                CurrentSelections.Add(this);
                OnSelect?.Invoke();
            }
            else
            {
                RemoveFlags(Flags.Selected);
                CurrentSelections.Remove(this);
                OnDeselect?.Invoke();
            }

            Highlight(select, HighlightObject.SELECT_COLOUR);

            UpdateCursor();

            base.Select(select, selectionMode);
        }

        /// <summary>Sets the <see cref="KeyCode"/> used to select/deselect this <see cref="Selectable"/>.</summary>
        public void SetActivationKey(KeyCode key)
        {
            InputManager.RegisterKeyUp(key, () =>
            {
                if(IsEnabled)
                {
                    Select(!IsSelected, SelectionModes.Key);
                }
            });
        }

        /// <summary>Unselects <see cref="Interactable.CurrentSelections"/> if it should be unselected.</summary>
        /// <remarks>Used by the <see cref="InputManager"/> for when any mouse up event occurs.</remarks>
        internal static void HandleAllMouseUps()
        {
            for(int i = CurrentSelections.Count - 1; i >= 0; i--)
            {
                Interactable selection = CurrentSelections[i];

                if(!(selection is Selectable))
                {
                    continue;
                }

                Interactable[] undermouse = InputManager.GetComponentsUnderMouse<Interactable>();
                
                if(undermouse != null)
                {
                    bool shouldContinue = false;

                    //check all interactables on the object under the mouse
                    foreach (Interactable interactable in undermouse)
                    {
                        //The selected object was clicked on again
                        if (interactable == selection)
                        {
                            shouldContinue = true;
                            break;
                        }

                        //The clicked on object can be interacted with at the same time
                        if (interactable.IsEnabled && CanInteractTogether(selection, interactable))
                        {
                            shouldContinue = true;
                            break;
                        }
                    }

                    if(shouldContinue)
                    {
                        continue;
                    }
                }

                if (InputManager.MouseWentDownOnUI)
                {
                    continue;
                }

                //this is used to sorta detect if the mouse was being used to pan the 
                // camera. It's not a perfect implementation though. We should actually 
                // check if the camera was panned.
                if (Time.time - InputManager.TimeOnMouseDown > MOUSE_DOWN_MAX_TIME)
                {
                    continue;
                }

                selection.Select(false, SelectionModes.Mouse);
            }
        }
    }
}
