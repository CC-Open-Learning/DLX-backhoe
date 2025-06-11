using UnityEngine;

namespace RemoteEducation.Interactions
{
    /// <summary>Base class for objects that are selected on mouse down and deselected on mouse up.</summary>
    public abstract class SemiSelectable : Interactable
    {
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

        protected override void MouseExit()
        {
            if (!IsSelected)
            {
                base.MouseExit();
            }
        }

        protected override void MouseDown()
        {
            if (!HasAnyFlag(Flags.KeyDown | Flags.AuxiliaryActivated))
            {
                Select(true, SelectionModes.Mouse);
            }

            base.MouseDown();
        }

        protected override void MouseUp()
        {
            if (!HasAnyFlag(Flags.KeyDown | Flags.AuxiliaryActivated))
            {
                Select(false, SelectionModes.Mouse);
            }

            base.MouseUp();
        }

        public override void Select(bool select, SelectionModes selectionMode)
        {
            if (select && !HasFlags(Flags.Selected))
            {
                AddFlags(Flags.Selected);
                CurrentSelections.Add(this);
                OnSelect?.Invoke();
            }
            else if(!select && HasFlags(Flags.Selected))
            {
                RemoveFlags(Flags.Selected);
                CurrentSelections.Remove(this);
                OnDeselect?.Invoke();
            }

            if (IsEnabled && !HasFlags(Flags.MouseOver))
            {
                Highlight(select, HighlightObject.HIGHLIGHT_COLOUR);
            }

            base.Select(select, selectionMode);
        }

        /// <summary>Sets the <see cref="KeyCode"/> used to select/deselect this <see cref="SemiSelectable"/>.</summary>
        public void SetActivationKey(KeyCode key)
        {
            if (key == KeyCode.None)
            {
                Debug.LogError("SemiSelectable: KeyCode.None passed into SetActivationKey!");
                return;
            }

            InputManager.RegisterKeyDown(key, () => ToggleActivationByKey(true));
            InputManager.RegisterKeyUp(key, () => ToggleActivationByKey(false));
        }

        /// <summary>Selects/Unselects this <see cref="Interactable"/> and sets <see cref="Interactable.Flags.KeyDown"/> on it.</summary>
        /// <remarks>Use this when the object is being selected/unselected by key presses.</remarks>
        /// <param name="select">If the key press is selecting this <see cref="Interactable"/>.</param>
        private void ToggleActivationByKey(bool select)
        {
            if (!IsEnabled || !select && !HasFlags(Flags.KeyDown))
                return;

            if (!HasAnyFlag(Flags.MouseDown | Flags.AuxiliaryActivated))
                Select(select, SelectionModes.Key);

            ToggleFlags(select, Flags.KeyDown);
        }

        /// <summary>Selects/Unselects this <see cref="Interactable"/> and sets <see cref="Interactable.Flags.AuxiliaryActivated"/> on it.</summary>
        /// <remarks>Use this when the object is being selected/unselected by means other than mouse input or keyboard keys such as physics triggers.</remarks>
        /// <param name="select">If the auxiliary action is selecting this <see cref="Interactable"/>.</param>
        protected void ToggleAuxiliaryActivation(bool select)
        {
            if (!IsEnabled || !select && !HasFlags(Flags.AuxiliaryActivated))
                return;

            if (!HasAnyFlag(Flags.MouseDown | Flags.KeyDown))
                Select(select, SelectionModes.Auxiliary);

            ToggleFlags(select, Flags.AuxiliaryActivated);
        }
    }
}
