/*
*  FILE          :	MagnifierTexture.cs
*  PROJECT       :	Core Engine 
*  PROGRAMMER    :	Vivek Savsaiya
*  FIRST VERSION :	2020-8-31
*  DESCRIPTION   :  This file contains the MagnifierManager class. 
*  
*  Class Description: This class manage magnifier and interact with GameObjects and MagnifierTexture to enable and disable magnifier.
*/

using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RemoteEducation.Interactions
{
    public class MagnifierManager : MonoBehaviour
    {
        public static MagnifierManager Instance;

        [SerializeField]
        int magnifierCount = 3;

        [SerializeField]
        GameObject magnifierTexturePrefab;

        [SerializeField]
        TextMeshProUGUI toggleButtonText;

        [SerializeField]
        private Transform magnifierPanel;

        private List<MagnifierTexture> m_magnifierTextureControllers = new List<MagnifierTexture>();

        // No references, commented out to suppress warning.
        //private bool magnifierManagerIsActive = true;

        private int numActiveMagnifiers = 0;

        [Tooltip("The LeanHover animation to play when a new magnifier is added")]
        [SerializeField] private LeanHover newMagnifierEffect;

        public void Awake()
        {
            Instance = this;
            GenerateMagnifierTexture();
        }

        /// <summary>
        /// Hides the magnified form of the passed component.
        /// This is done by comparing the component passed with each component in the m_magnifierTextureControllers list. 
        /// </summary>
        /// <param name="component">The component to hide</param>
        /// <param name="isForced">When passed true, locked components will be hidden.</param>
        public void DisableMagnifierTexture(GameObject component, bool isForced = false)
        {
            //How it works:
            //This loops through each component in the m_magnifierTextureControllers list, if it finds a matching component to hydraulicComponentController it 
            // removes it by setting the values of this component to null. 
            foreach (MagnifierTexture m_magnifierTexture in m_magnifierTextureControllers)
            {
                if (m_magnifierTexture.currentComponent != null)
                {
                    if (m_magnifierTexture.currentComponent.gameObject == component.gameObject)
                    {
                        m_magnifierTexture.DisableMagnifierTexture(isForced);
                    }
                }
            }

            RefreshMagnifierCount();
        }

        /// <summary>
        /// Makes the passed component appear magnified on the screen. 
        /// </summary>
        /// <param name="component">The component to magnify</param>
        /// <param name="magnifierParent"></param>
        public void EnableMagnifierTexture(GameObject component, GameObject magnifierParent)
        {
            //How it works:
            //Calls GetMagnifierTexture to get a null or blank ManifierTexture from the m_magnifierTextureControllers list
            //Checks if null but honestly is never null
            //Sets the values of this MagnifierTexture (the one from the list) to be equal to the hydraulicComponent that was passed in
            //Through this, the magnified hydraulicComponent is added to the m_magnifierTextureControllers list 

            MagnifierTexture m_magnifierTexture = GetMagnifierTexture(component);
            if (m_magnifierTexture != null)
            {
                m_magnifierTexture.SetMagnifierTexture(component, magnifierParent);
            }

            RefreshMagnifierCount();
        }

        /// <summary>
        /// Generates effectively blank values for the m_magnifierTextureControllers list. 
        /// These blank values can then be updated with real values to display magnified components. 
        /// </summary>
        private void GenerateMagnifierTexture()
        {
            m_magnifierTextureControllers.Clear();
            for (int i = 0; i < magnifierCount; i++)
            {
                GameObject m_magnifierTexture = Instantiate(magnifierTexturePrefab, magnifierPanel);
                //m_magnifierTexture.transform.SetParent(magnifierPanel);
                m_magnifierTextureControllers.Add(m_magnifierTexture.GetComponent<MagnifierTexture>());
            }

            RefreshMagnifierCount();
        }

        /// <summary>
        /// Tries to find the associated texture for the passed component in the m_magnifierTextureControllers list.
        /// Is a helper to EnableMagnifierTexture.
        /// </summary>
        /// <param name="component">The value / component to find</param>
        /// <returns>
        /// Either the matching texture for the passed component or a blank component when no match is found.
        /// Returns null when the MagnifierManager is full.
        /// </returns>
        private MagnifierTexture GetMagnifierTexture(GameObject component)
        {
            //See if there is a matching component in the m_magnifierTextureControllers which matches the passed component, if so return it
            foreach (MagnifierTexture m_magnifierTexture in m_magnifierTextureControllers)
            {
                if (m_magnifierTexture.currentComponent != null)
                {
                    if (m_magnifierTexture.currentComponent.gameObject == component.gameObject)
                    {
                        return m_magnifierTexture;
                    }
                }
            }

            //By this point no matching component was found so iterate the list again to return a null component
            //This null component can then be used to add the new (not found) component to the m_magnifierTextureControllers list
            foreach (MagnifierTexture m_magnifierTexture in m_magnifierTextureControllers)
            {
                if (m_magnifierTexture.currentComponent == null)
                {
                    return m_magnifierTexture;
                }
            }

            //If both attempts don't return, there's no more room for maximized components so return null
            return null;
        }

        #region UIRelated

        /// <summary>
        /// Iterates through the m_magnifierTextureControllers list counting the number of active texture controllers.
        /// This count is then reflected in the text of the toggle button. 
        /// </summary>
        public void RefreshMagnifierCount()
        {
            int oldNumActive = numActiveMagnifiers;
            numActiveMagnifiers = 0;

            foreach (MagnifierTexture texture in m_magnifierTextureControllers)
            {
                if (texture.currentComponent != null)
                {
                    numActiveMagnifiers++;
                }
            }

            if (oldNumActive < numActiveMagnifiers)
            {
                //Whenever a new item is added make the toggle button for the magnifier manager appear as if it is blinking
                //Start the enter transitions for the button to change the colour

                StartCoroutine(BlinkButton());
            }

            toggleButtonText.SetText(numActiveMagnifiers.ToString());
        }

        private IEnumerator BlinkButton()
        {
            if (newMagnifierEffect)
            {
                newMagnifierEffect.EnterTransitions.Begin();
                yield return new WaitForSeconds(0.5f);
                newMagnifierEffect.ExitTransitions.Begin();
                yield return null;
            }
        }

        public void HighlightMagnifier(GameObject gameObj, bool isHighlighted)
        {
            foreach (MagnifierTexture m_magnifierTexture in m_magnifierTextureControllers)
            {
                if (m_magnifierTexture.currentComponent != null)
                {
                    if (m_magnifierTexture.currentComponent.gameObject == gameObj)
                    {
                        m_magnifierTexture.SetHighlightedRing(isHighlighted);
                    }
                }
            }
        }

        #endregion
    }
}