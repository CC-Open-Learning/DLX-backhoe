using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RemoteEducation.Localization;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class ExtraDetails : MonoBehaviour
    {
        public enum Position
        {
            Top,
            Left,
            Center,
            Right,
            Bottom
        }

        public enum Format
        {
            Single,
            Double
        }

        [Tooltip("List of Images for Details Panel to Display. Currently Supports 2 Images")]
        public List<Sprite> contentImage;

        [Tooltip("List of Strings for Details Panel to Display. Currently Supports 2 Strings")]
        public List<string> captionText;

        [Tooltip("Context String")]
        public string contentText;

        
        [Tooltip("Horizontal Size of Panel, Automatic sizing at 0.")]
        public float horizontalContextSize = 0;

        [Tooltip("Vertical Size of Panel, Automatic sizing at 0.")]
        public float verticalContextSize = 0;

        public Position objectPostion;
        public Format objectFormat;

        public string GetContext(string contextString)
        {
            return Localizer.Localize(contextString);
        }
    }
}