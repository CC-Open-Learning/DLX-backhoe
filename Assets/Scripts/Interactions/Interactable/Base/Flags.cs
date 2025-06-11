using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RemoteEducation.Interactions
{
    partial class Interactable
    {
        [Flags]
        public enum Flags
        {
            MouseDown = 1,
            MouseOver = 1 << 1,
            Selected = 1 << 2,
            Highlightable = 1 << 3,
            KeyDown = 1 << 4,
            AuxiliaryActivated = 1 << 5,
            ExclusiveInteraction = 1 << 6,
            InteractionsDisabled = 1 << 7,
            CustomCursors = 1 << 8,
            Highlighted = 1 << 9,
            StartInteractMouseDown = 1 << 10,
        }

        private Flags flags = Flags.Highlightable | Flags.CustomCursors | Flags.ExclusiveInteraction; // The flags that an Interactable has by default.

        /// <summary>Gets if this object is currently selected.</summary>
        public bool IsSelected { get { return HasFlags(Flags.Selected); } }

        /// <summary>Gets if this object is currently being highlighted.</summary>
        public bool IsHighlighted { get { return HasFlags(Flags.Highlighted); } }

        /// <summary>Gets if this object is currently able to be interacted with.</summary>
        public virtual bool IsEnabled 
        { 
            get 
            {
                if(subsetActiveObjects == null || subsetActiveObjects.Contains(this))
                {
                    return !HasFlags(Flags.InteractionsDisabled);
                }

                return false;
            } 
        }

        /// <summary>Checks if this object has a specific flag or set of <see cref="Flags"/>.</summary>
        /// <returns><see langword="true"/> if the flag exists within the <see cref="flags"/> variable.</returns>
        /// <param name="flags">Bitwise flags to check against the current <see cref="flags"/>.</param>
        /// <remarks>Multiple flags can be checked in one function call by using the bitwise OR operator '|' between each of the <see cref="Flags"/>.</remarks>
        public bool HasFlags(Flags flags) { return (this.flags & flags) == flags; }

        /// <summary>Checks if this object has at least one flag of the provided <paramref name="flags"/>.</summary>
        /// <returns><see langword="true"/> if any of the flags is set within the <see cref="flags"/> variable.</returns>
        /// <param name="flags">Bitwise flags to check against the current <see cref="flags"/>.</param>
        /// <remarks>Multiple flags can be checked in one function call by using the bitwise OR operator '|' between each of the <see cref="Flags"/>.</remarks>
        public bool HasAnyFlag(Flags flags) { return (this.flags & flags) != 0; }

        /// <summary>Adds the given flag or <see cref="Flags"/> to the <see cref="flags"/> variable.</summary>
        /// <param name="flags">Bitwise flags to add to the current <see cref="flags"/>.</param>
        /// <remarks>Multiple flags can be added in one function call by using the bitwise OR operator '|' between each of the <see cref="Flags"/>.
        /// If the object is disabled by adding <see cref="Flags.InteractionsDisabled"/>, <see cref="OnDisable"/> is called.</remarks>
        public void AddFlags(Flags flags)
        {
            bool wasEnabled = !HasFlags(Flags.InteractionsDisabled);

            this.flags |= flags;

            if (wasEnabled && HasFlags(Flags.InteractionsDisabled))
            {
                OnDisable();
            }
        }

        /// <summary>Removes the given flag or <see cref="Flags"/> from the <see cref="flags"/> variable.</summary>
        /// <param name="flags">Bitwise flags to remove from the current <see cref="flags"/>.</param>
        /// <remarks>Multiple flags can be removed in one function call by using the bitwise OR operator '|' between each of the <see cref="Flags"/>.</remarks>
        public void RemoveFlags(Flags flags) { this.flags &= ~flags; }

        /// <summary>Adds or removes the given flag or <see cref="Flags"/> to or from the <see cref="flags"/> variable.</summary>
        /// <param name="flags">Bitwise flags to add or remove to or from the current <see cref="flags"/>.</param>
        /// <param name="enable">If <see langword="true"/>, adds the <paramref name="flags"/> to the current <see cref="flags"/>. Removes if <see langword="false"/>.</param>
        /// <remarks>Multiple flags can be modified in one function call by using the bitwise OR operator '|' between each of the <see cref="Flags"/>.</remarks>
        public void ToggleFlags(bool enable, Flags flags)
        {
            if (enable)
                AddFlags(flags);
            else
                RemoveFlags(flags);
        }

        public Flags GetFlags()
        {
            return flags;
        }

        /// <summary>Provides a method to set all the <see cref="Flags"/> at once for save and load.</summary>
        /// <remarks>Avoid using this method for any other purpose!</remarks>
        protected void SetFlags(int flags)
        {
            this.flags = (Flags)flags;
        }
    }
}
