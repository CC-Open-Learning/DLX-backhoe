/*
*  FILE          :	InspectableElement.cs
*  PROJECT       :	CORE Engine Inspection Module
*  PROGRAMMER    :	Duane Cressman, Kieron Higgs
*  FIRST VERSION :	2020-11-02
*  DESCRIPTION   :  This file contains the InspectableElement class. 
*                   This class will be used to define a single element that can be inspected
*                   in a inspection scenario. This class is in charge of spawning in the actual
*                   components that are visible to the user. The amount and location of component
*                   to be loaded in is defined by all the ElementAnchor gameobjects below this class.
*                   
*                   This class is used to hold which components are in which state. There is a prefab 
*                   with this class in it for each type of component. At runtime the InspectableController 
*                   will choose of these prefabs and copy the InspectableElement onto a gameobject in 
*                   the scene.
*                   
*                   The Gameobject structure should go:
*                  
*                    >InspectableController
*                        >ElementSpawnningData (the InspectableElement will be added on this gameobject by the InspectableController
*                            >ElementAnchor
*                                >(the component will be added here)            
*/

using UnityEngine;
using RemoteEducation.Interactions;
using RemoteEducation.UI.Tooltip;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using RemoteEducation.Localization;

namespace RemoteEducation.Scenarios.Inspectable
{
    public abstract class InspectableElement : Selectable
    {
        public const int NO_STATE_SELECTION = -1;

        public enum Status
        {
            None = 0,
            InspectedPositive = 1,
            InspectedNegitive = 2,
            Warning = 3,
            EvaluatedCorrect = 4,
            EvaluatedIncorrect = 5
        }

        [SerializeField, Tooltip("The state that this component is in. If \"Good\", then it will be automatically set")]
        public string StateMessage;
        [SerializeField, Tooltip("Optional field, the descriptive text for the state")]
        public string StateDescriptionToken;

        [Tooltip("Whether the element is in a good or bad state.")]
        public InspectableState State;

        /// <summary>
        /// The name shown in the checklist panel. This name is to be used with the 
        /// group name set in the <see cref="InspectableController"/>
        /// </summary>
        [HideInInspector]
        public string ChecklistName;

        /// <summary>The name shown in the result scene and tool tip.</summary>
        [HideInInspector] public string FullName;
        
        /// <summary>The feedback shown in the comments area of result scene.</summary>
        [HideInInspector] public string FeedbackDetail;

        /// <summary>The description for state shown in the description area of result scene.</summary>
        public string StateDescription => StateDescriptionToken != ""? Localizer.Localize(StateDescriptionToken) : null;

        public InspectableController ParentController { get; private set; }
        public InspectableManager InspectableManager => ParentController.InspectableManager;

        private InspectableObject InspectableObject { get; set; }

        /// <summary>Whether this inspectable should use the default highlight colors from <see cref="RemoteEducation.Scenarios.Inspectable.InspectableObject"/>.</summary>
        /// <remarks>Set to <see langword="false"/> to use custom colors instead.</remarks>
        public bool UseDefaultHighlightColor = true;
        /// <summary>Correct state <see cref="Color"/> to use for highlighting this inspectable if <see cref="UseDefaultHighlightColor"/> is <see langword="false"/>.</summary>
        public Color CorrectHighlightColor = new Color(0.5803922f, 0.8470588f, 0.5607843f);
        /// <summary>Incorrect state <see cref="Color"/> to use for highlighting this inspectable if <see cref="UseDefaultHighlightColor"/> is <see langword="false"/>.</summary>
        public Color IncorrectHighlightColor = new Color(0.8470588f, 0.5803922f, 0.5607843f);
        /// <summary>Normal <see cref="Color"/> to use for highlighting this inspectable if <see cref="UseDefaultHighlightColor"/> is <see langword="false"/>.</summary>
        public Color NormalHighlightColor = new Color(0.8470588f, 0.8313726f, 0.5607843f);

        public bool CurrentlyInspectable
        {
            get
            {
                return currentlyInspectable;
            }

            private set
            {
                currentlyInspectable = value;
                OnCurrentlyInspectableStateChanged?.Invoke(value);
            }
        }

        protected bool currentlyInspectable;

        public Action<bool> OnCurrentlyInspectableStateChanged;

        [HideInInspector]
        public SimpleTooltip Tooltip;

        // This may need to be differentiated from the MouseInteraction OnSelect
        // It is meant to be invoked when the user indicates an inspection selection
        public Action OnInspect;

        //These events should eventually be implemented in Interactable.
        public new Action OnMouseEnter;
        public new Action OnMouseExit;

        public override bool IsEnabled
        {
            get
            {
                if(!base.IsEnabled)
                {
                    return false;
                }

                return CurrentlyInspectable;
            }
        }

        /// <summary>
        /// The state that this inspectable is in.
        /// This state is only used internally to compare which state the user selected.
        /// This value corresponds the to int in <see cref="InspectableController.IdentifiersByElement"/>
        /// </summary>
        /// <remarks>This value corresponds the to int in <see cref="InspectableController.IdentifiersByElement"/></remarks>
        public int StateIdentifier { get; set; }

        /// <summary>
        /// The state that the user selected for this element. 
        /// </summary>
        /// <remarks>This value corresponds the to int in <see cref="InspectableController.IdentifiersByElement"/></remarks>
        public int UserSelectedIdentifier { get; protected set; }
        public Status CurrentStatus { get; set; }

        public bool HasBeenEvaluated => CurrentStatus == Status.EvaluatedCorrect || CurrentStatus == Status.EvaluatedIncorrect;
        public bool HasBeenInspected => UserSelectedIdentifier != NO_STATE_SELECTION;

        public bool InspectionIsCorrect => StateIdentifier == UserSelectedIdentifier;


        #region Initialization Methods

        protected override void Awake()
        {
            highlight = new SoftEdgeHighlight(gameObject, HighlightObject.OUTLINE_WIDTH_THIN, HighlightObject.Mode.OutlineVisible);
        }

        public virtual void Initialize(InspectableController parentController, InspectableObject parentObject, string checkListName, string fullName, int stateIdentifier)
        {
            ChecklistName = checkListName;
            FullName = fullName;

            CurrentlyInspectable = true;
            ParentController = parentController;
            InspectableObject = parentObject;

            StateIdentifier = stateIdentifier;
            UserSelectedIdentifier = NO_STATE_SELECTION;
            CurrentStatus = Status.None;

            OnSelect += () => InspectableManager.UpdateCurrentlySelectedElement(this, true);
            OnDeselect += () => InspectableManager.UpdateCurrentlySelectedElement(this, false);

            OnClick += () => InspectableManager.ElementClicked(this);
            OnMouseEnter += () => InspectableManager.UpdateElementUnderMouse(this);
            OnMouseExit += () => InspectableManager.UpdateElementUnderMouse(this);

            RemoveFlags(Flags.ExclusiveInteraction);
        }

        /* Method Header : AddElementToGameObject
         * This method will add a the InspectableElement to the GameObject passed in.
         * Since you can't directly add a component from one gameobject to another, this 
         * is done by copying all the values.
         */
        public abstract InspectableElement AddElementToGameObject(GameObject gameObject);

        public void SetCurrentlyInspectable(bool currentlyInspectable)
        {
            if (Tooltip)
            {
                Tooltip.ToolTipActive = currentlyInspectable;
            }

            CurrentlyInspectable = currentlyInspectable;
        }

        #endregion

        protected override void MouseEnter()
        {
            base.MouseEnter();
            RefreshHighlightColour();

            OnMouseEnter?.Invoke();
        }

        protected override void MouseExit()
        {
            base.MouseExit();

            OnMouseExit?.Invoke();
        }

        public override void Select(bool select, SelectionModes selectionMode = SelectionModes.Mouse)
        {
            base.Select(select, selectionMode);
            RefreshHighlightColour();
        }

        /// <summary>Get the colour that the highlight should be based on how the object has been inspected</summary>
        private Color GetHighlightColour()
        {
            if (UseDefaultHighlightColor)
            {
                if (UserSelectedIdentifier == NO_STATE_SELECTION)
                {
                    return IsSelected ? InspectableObject.Unchecked_Select : InspectableObject.Unchecked_Highlight;
                }

                if (ParentController.GetStateByIdentifer(UserSelectedIdentifier).State == InspectableState.Good)
                {
                    return IsSelected ? InspectableObject.Good_Select : InspectableObject.Good_Highlight;
                }

                return IsSelected ? InspectableObject.Bad_Select : InspectableObject.Bad_Highlight;
            }

            if(UserSelectedIdentifier != NO_STATE_SELECTION)
            {
                if (InspectionIsCorrect)
                {
                    return CorrectHighlightColor;
                }
                else
                {
                    return IncorrectHighlightColor;
                }
            }
            return NormalHighlightColor;
            
        }


        /// <summary>If already highlighted, update the colour using <see cref="GetHighlightColour"/> </summary>
        public void RefreshHighlightColour()
        {
            if(HasFlags(Flags.Highlighted))
                Highlight(true, GetHighlightColour());
        }

        /// <summary>
        /// Do the inspection on this object. The state that the user selected will
        /// be saved to this element.
        /// </summary>
        /// <param name="stateIdentifier">The identifier for the state of the element</param>
        public void MakeUserSelection(int stateIdentifier)
        {
            UserSelectedIdentifier = stateIdentifier;

            if(stateIdentifier == NO_STATE_SELECTION)
            {
                CurrentStatus = Status.None;
            }
            else
            {
                CurrentStatus = ParentController.GetStateByIdentifer(stateIdentifier).State == InspectableState.Good ? Status.InspectedPositive : Status.InspectedNegitive;
            }

            RefreshHighlightColour();

            OnInspect?.Invoke();

            InspectableManager.RefreshElementState(this);
        }

        /// <summary>
        /// Update the status of this object to one of the evaluated states.
        /// </summary>
        /// <returns>The status of the object.</returns>
        public Status Evaluate()
        {
            if(UserSelectedIdentifier == NO_STATE_SELECTION)
            {
                CurrentStatus = Status.Warning;
            }
            else
            {
                CurrentStatus = InspectionIsCorrect ? Status.EvaluatedCorrect : Status.EvaluatedIncorrect;
            }

            return CurrentStatus;
        }

        /// <summary>
        /// Update the status of this object to one of the evaluated states. If the selection is blank, the selection will be considered as InspectableState.Good.
        /// </summary>
        /// <returns>The status of the object.</returns>
        public Status EvaluateNoSelection()
        {
            if (UserSelectedIdentifier == NO_STATE_SELECTION)
            {
                CurrentStatus = GetCorrectState().State == InspectableState.Good ? Status.EvaluatedCorrect : Status.EvaluatedIncorrect;
            }
            else
            {
                CurrentStatus = InspectionIsCorrect ? Status.EvaluatedCorrect : Status.EvaluatedIncorrect;
            }

            return CurrentStatus;
        }

        /// <summary>
        /// Get the state that the user has selected.
        /// </summary>
        /// <returns>The <see cref="InspectableElement"/> for the state the user selected.</returns>
        public InspectableElement GetSelectedState()
        {
            return ParentController.GetStateByIdentifer(UserSelectedIdentifier);
        }

        /// <summary>
        /// Get the correct state of this element.
        /// </summary>
        /// <returns>The <see cref="InspectableElement"/> for the correct state.</returns>
        public InspectableElement GetCorrectState()
        {
            return ParentController.GetStateByIdentifer(StateIdentifier);
        }

        /// <summary>
        ///     Predicate to determine whether an <see cref="InspectableElement"/>
        ///     is considered 'real'<br/><br/>
        ///     A 'real' element is one that has an <see cref="State"/> of <see cref="InspectableState.Good"/>,
        ///     or a <see cref="DynamicInspectableElement"/> with a <see cref="State"/> of <see cref="InspectableState.Bad"/>
        ///     and a <see cref="DynamicInspectableElement.BadMode"/> which is non-negative.
        /// </summary>
        /// <param name="element">The <see cref="InspectableElement"/> or <see cref="DynamicInspectableElement"/> to check</param>
        /// <returns><see langword="true" /> if the element is considered 'real', <see langword="false"/> otherwise</returns>
        public static bool IsElementReal(InspectableElement element)
        {
            if (element is DynamicInspectableElement dynamic)
            {
                if (dynamic.State == InspectableState.Good)
                {
                    return true;
                }
                else
                {
                    // "Real" states must have a non-negative BadMode
                    return dynamic.BadMode >= 0;
                }
            }
            else
            {
                // A standard InspectableElement is just considered to be real
                return true;
            }
        }
    }
}