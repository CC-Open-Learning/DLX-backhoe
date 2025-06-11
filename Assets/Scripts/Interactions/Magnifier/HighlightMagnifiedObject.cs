using UnityEngine;
using UnityEngine.EventSystems;
using static RemoteEducation.Interactions.Interactable;

namespace RemoteEducation.Interactions
{
    /// <summary>
    /// This class is to highlight the associated component to the magnified item that has been added 
    /// to the magnifier panel whenever user hovers the mouse on the item on the panel.
    /// This feature is designed for distinguishing multiple components when they are added to the
    /// magnifier panel at the same time.
    /// </summary>
    public class HighlightMagnifiedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary><see cref="Interactable"/> data for the magnified item shown in the <see cref="magnifierTexture"/></summary>
        private Interactable interactable;

        [Tooltip("MagnifierTexture object for retrieving magnified object data.")]
        [SerializeField] private MagnifierTexture magnifierTexture;

        void Start()
        {
            GetInteractable();
        }

        /// <summary>Update the <see cref="Interactable"/> everytime an item is removed from the magnifier panel.</summary>
        void Update()
        {
            if (magnifierTexture != null)
                interactable = magnifierTexture.currentComponent.GetComponent<Interactable>();
        }

        /// <summary>Gets the <see cref="Interactable"/> magnified by the <see cref="magnifierTexture"/> and stores it in <see cref="interactable"/>.</summary>
        /// <remarks>If there is no <see cref="Interactable"/> attached to the magnified object, this script will destroy itself as it would serve no purpose.</remarks>
        private void GetInteractable()
        {
            if (!magnifierTexture.currentComponent.TryGetComponent(out interactable))
            {
                Destroy(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            interactable.Highlight(true, HighlightObject.HIGHLIGHT_COLOUR);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var isSelected = interactable.HasFlags(Flags.Selected);
            interactable.Highlight(isSelected, HighlightObject.SELECT_COLOUR);
        }
    }
}