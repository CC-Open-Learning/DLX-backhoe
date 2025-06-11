using Lean.Gui;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace RemoteEducation.Modules.Inspection.Scenarios
{
    /// <summary>The ReadOnly Inspection Checklist displays a read only list of inspectable items with selected status.
    public class ReadOnlyInspectionChecklist : MonoBehaviour, IInspectableUI
    {
        [SerializeField] private TextMeshProUGUI checklistTitle;

        [SerializeField] private Transform[] columns;

        [SerializeField] private GameObject inspectableGroupPrefab;
        [SerializeField] private GameObject lineItemPrefab;

        /// <summary>Tracks all ChecklistItems in the checklist, so that they can be seen with their status.</summary>
        private List<ReadOnlyChecklistItem> lineItems;

        public void InitializeUI(InspectableManager inspectableManager)
        { 
            lineItems = new List<ReadOnlyChecklistItem>();

            checklistTitle.text = "Engine.InspectionChecklist".Localize();

            GenerateChecklistItems(inspectableManager.InspectableControllers);

            inspectableManager.OnElementStateChanged += RefreshElement;
        }

        /// <summary>Update the status icons for all active elements.</summary>
        public void RefreshActiveElements()
        {
            lineItems.Where(x => x.InspectableElement.CurrentlyInspectable).ToList().ForEach(x => x.UpdateStatusIcon());
        }

        /// <summary>Update the status icon for an Item based on a <see cref="InspectableElement"/></summary>
        /// <param name="inspectableElement">The element that was changed</param>
        public void RefreshElement(InspectableElement inspectableElement)
        {
            ReadOnlyChecklistItem item = lineItems.Find(x => x.InspectableElement == inspectableElement);

            item.UpdateStatusIcon();
        }

        /// <summary>Creates group heading and list of items under each heading.</summary>
        private void GenerateChecklistItems(List<InspectableController> controllers)
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                GameObject container = Instantiate(inspectableGroupPrefab);

                container.transform.SetParent(columns[i % columns.Length], false);
                container.GetComponentInChildren<TextMeshProUGUI>().text = controllers[i].ElementGroupName;

                foreach (InspectableElement element in controllers[i].InspectableElements)
                {
                    var item = Instantiate(lineItemPrefab).GetComponent<ReadOnlyChecklistItem>();

                    item.Setup(container, element);

                    lineItems.Add(item);
                }
            }
        }
    }
}
