    using Lean.Transition.Method;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ViewModesPanel : MonoBehaviour
{
    [SerializeField]
    private LeanRectTransformSizeDelta EnterTransition;

    public GameObject ButtonPanelParent;

    public CanvasGroup CanvasGroup;

    public int BottomPadding = 6;

    public float TransitionDuration = 0.5f;

    public void Initialize()
    {
        StartCoroutine(InitCoRoutine());
    }

    private IEnumerator InitCoRoutine()
    {
        if (ButtonPanelParent.transform.childCount == 0)
        {
            //if there are no buttons, hide the panel
            gameObject.SetActive(false);
            yield break;
        }

        yield return new WaitForEndOfFrame();

        RectTransform transformSelf = transform as RectTransform;
        RectTransform transformButtonPanel = ButtonPanelParent.transform as RectTransform;
        // enter transition size delta Y = original height + buttons transform height + padding (6)


        bool startState = transformButtonPanel.gameObject.activeSelf;

        transformButtonPanel.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        transformButtonPanel.gameObject.SetActive(false);
        
        yield return new WaitForEndOfFrame();

        transformButtonPanel.gameObject.SetActive(startState);


        //This will ensure that the content size fitter has set the size of the rect transform
        // before the hight is used. This loop hardly ever gets passed the first time through, but 
        // limit it to 5 times so that if something weird happens, we don't get an infinite loop
        for (int i = 0; i < 5; i++)
        {
            if(transformButtonPanel.rect.height == 0)
            {
                yield return null;
            }
            else
            {
                break;
            }
        }


        float sizeDeltaY = transformSelf.rect.height + transformButtonPanel.rect.height + BottomPadding;

        EnterTransition.Data = new LeanRectTransformSizeDelta.State()
        {
            Target = transformSelf,
            SizeDelta = new Vector2(transformSelf.rect.width, sizeDeltaY),
            Duration = TransitionDuration
        };

        ButtonPanelParent.SetActive(false);
    }
}
