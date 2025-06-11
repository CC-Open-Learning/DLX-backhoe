using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentPanel : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public Image Image;
    public TextMeshProUGUI TB1_Title;
    public TextMeshProUGUI TB1_Text;
    public TextMeshProUGUI TB2_Title;
    public TextMeshProUGUI TB2_Text;

    public LeanButton ButtonOne;
    public TextMeshProUGUI ButtonOneText;
    public LeanButton ButtonTwo;
    public TextMeshProUGUI ButtonTwoText;

    public CanvasGroup CanvasGroup;

    [SerializeField]
    private Scrollbar Scrollbar;

    public float ContentOpacity
    {
        get
        {
            return CanvasGroup.alpha;
        }

        set
        {
            CanvasGroup.alpha = value;
        }
    }

    public void SetPageValues(ComponentPanelData content)
    {
        Title.text = content.Title;
        Image.sprite = content.Image;
        TB1_Title.text = content.TB1_Title;
        TB1_Text.text = content.TB1_Text.Replace("\\n", "\n");
        TB2_Title.text = content.TB2_Title;
        TB2_Text.text = content.TB2_Text.Replace("\\n", "\n");

        if (content.ButtonOneIsVisable)
        {
            ButtonOneText.text = content.ButtonOneText;
            
            if(content.ButtonOneAction != null)
            {
                ButtonOne.OnClick.RemoveAllListeners();
                ButtonOne.OnClick.AddListener(content.ButtonOneAction);
            }
        }

        if(content.ButtonTwoIsVisable)
        {
            ButtonTwoText.text = content.ButtonTwoText;

            if(content.ButtonTwoAction != null)
            {
                ButtonTwo.OnClick.RemoveAllListeners();
                ButtonTwo.OnClick.AddListener(content.ButtonTwoAction);
            }
        }

        ButtonOne.gameObject.SetActive(content.ButtonOneIsVisable);
        ButtonTwo.gameObject.SetActive(content.ButtonTwoIsVisable);
    }
}
