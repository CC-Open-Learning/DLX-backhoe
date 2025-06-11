/*
 *  FILE          :	ScenarioPickerItem.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Michael Hilts, Kieron Higgs, David Inglis
 *  FIRST VERSION :	2019-10-20
 *  DESCRIPTION   : This file contains the ScenarioPickerItem class which is responsible for switching
 *					scenarios between selected and available in the new scenario selection screen.
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RemoteEducation.Scenarios
{
    /// <summary>Component for items in the Scenario menu.</summary>
    public class ScenarioPickerItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
        [SerializeField] private TextMeshProUGUI titleField;

        private Scenario scenario;

        public Scenario Scenario
        {
            get { return scenario; }
            set
            {
                scenario = value;
                titleField.text = scenarioTitle;
            }
        }

        [NonSerialized] public bool Selected;
        [NonSerialized] public ScenarioPicker Parent;

		[SerializeField] private Image background;
		[SerializeField] private ColorBlock backgroundColors;

        private string scenarioTitle => (scenario == null) ? "Null" : string.Format("{0} : {1}", scenario.Module, scenario.Title);

        private void Awake()
        {
            Selected = false;

            if (!titleField)
			{ 
				titleField = GetComponentInChildren<TextMeshProUGUI>(); 
			}
        }

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!Selected)
			{
				Highlight();
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!Selected)
			{
				Reset();
			}
		}
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!Selected && Parent)
			{
				Parent.Select(this);
			}
		}

		public void Highlight()
		{
			background.color = backgroundColors.highlightedColor;
		}


		public void Select()
		{
			background.color = backgroundColors.selectedColor;
			Selected = true;
		}

		public void Reset()
		{
			background.color = backgroundColors.normalColor;
			Selected = false;
		}
	}
}