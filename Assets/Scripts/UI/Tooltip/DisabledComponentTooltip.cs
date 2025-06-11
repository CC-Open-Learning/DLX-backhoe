using UnityEngine;
using System;
using TMPro;

namespace RemoteEducation.UI.Tooltip
{

    [Serializable]
    [CreateAssetMenu]
    public class DisabledComponentTooltip : ScriptableObject
    {
        [System.Serializable]
        public struct Style
        {
            public string tag;
            public Color color;
            public bool bold;
            public bool italic;
            public bool underline;
            public bool strikethrough;
        }

        [Header("Tooltip Panel")]
        public Sprite slicedSprite;
        public Color color = Color.red;

        [Header("Font")]
        public TMP_FontAsset fontAsset;
        public Color defaultColor = Color.white;

        [Header("Formatting")]
        public Style[] fontStyles;
    }
}