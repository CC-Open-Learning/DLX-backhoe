/*
*  FILE          :	TextBasedElement.cs
*  PROGRAMMER    :	Leon Vong
*  FIRST VERSION :	2021-19-08
*  DESCRIPTION   :  This file is used to describe an object as an element to examine for our UserInputWindow
*/

using UnityEngine;
using RemoteEducation.Interactions;
using RemoteEducation.UI.Tooltip;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class TextBasedElement : Selectable, IInitializable
    {
        /// <summary> Element name that panel is reading </summary>
        [SerializeField]
        private string elementName; 
        public string ElementName 
        { 
            get { return elementName; } 
            set { elementName = value; } 
        }

        /// <summary> Prompt that user reads on panel </summary>
        [SerializeField] 
        private string userPrompt; 
        public string UserPrompt
        { 
            get { return userPrompt; } 
            set { userPrompt = value; } 
        }

        /// <summary> Completion status of task </summary>
        [SerializeField] 
        private bool completionStatus; 
        public bool CompletionStatus
        { 
            get { return completionStatus; } 
            set { completionStatus = value; } 
        }

        /// <summary> Event for MouseEnter </summary>
        public new Action OnMouseEnter;

        /// <summary> Initializing Functionality </summary>
        public void Initialize(object input = null)
        {
            //Mouse Interactions
            OnSelect += () => UserInputManager.Instance.ShowPanel(this);
            OnDeselect += () => UserInputManager.Instance.HidePanel(this);
            OnMouseEnter += () => UserInputManager.Instance.UpdateContents(this);
        }

        protected override void MouseEnter()
        {
            base.MouseEnter();

            OnMouseEnter?.Invoke();
        }
    }
}