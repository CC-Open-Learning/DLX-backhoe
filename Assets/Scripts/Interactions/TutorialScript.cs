/*
 *  FILE          :	TutorialScript.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Kastriot Sulejmani
 *  FIRST VERSION :	2020-11-01
 *  
 *  DESCRIPTION   : 
 *		This will be used for the first task in every scene to guide the user through completion
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class TutorialScript : MonoBehaviour
{
    #region References
    public Image statusImg;
    public Sprite[] statusSprites;
    public Color greenColor;

    public static bool tutorialActive;
    //public bool isArrowScaled;
    public bool isUsingCameraAnims;
    public GameObject mainCamGO;
    public Animation camAnim;
    public string[] camString;
    public GameObject sceneCameraGO;

    public Animation blackCircleAnim;
    public Text tutorialText;
    public string beginTutorialString;
    public string sceneTutorialString;
    public string[] tutorialStringTexts;

    public Transform arrowTrans;
    public Transform[] tutorialObj;
    public RectTransform tutorialCircleTrans;
    public GameObject arrowGO;

    private int taskValue = 0;
    public int endTaskValue;

    public float xArrowOffset = 0.1f;
    public float yArrowOffset = 0.2f;

    public Action TutorialFinished;

    // SCENE SPECIFIC STUFF //
    public bool isHydraulics;

    public Action ActionOne;
    public Action ActionTwo;

    #endregion

    #region Properties
    public static bool ShouldTutorialLoad
    {
        get { return PlayerPrefs.GetInt("tutorial") == 1; }
    }
    #endregion

    public void BeginTutorial()
    {
        //if(PlayerPrefs.GetInt("tutorial") == 0)
        //{
        //    sceneCameraGO.SetActive(true);
        //    gameObject.SetActive(false);

        //    tutorialActive = false;
        //} else
        //{
        //    sceneCameraGO.SetActive(false);

        //    tutorialActive = true;
        //}

        sceneCameraGO.SetActive(false);

        tutorialActive = true;

        arrowTrans.localScale = new Vector3(3, 3, 3);

        if(isHydraulics)
        {
            arrowTrans.localScale = new Vector3(30, 30, 30);
        }


        taskValue=0;
        tutorialText.text = "" + beginTutorialString +"\n" + sceneTutorialString;

        // RUN FUNCTION TEST //
        //StartCoroutine(Wait());
    }

    #region TEST PURPOSES
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        CheckTutorialFunc();
        yield return new WaitForSeconds(3);
        CheckTutorialFunc();
    }
    #endregion

    #region Update Arrow / Circle Pos
    // Set arrow position to direct towards current Task object //

    public void UpdateArrowPos()
    {
        arrowGO.transform.position = new Vector3(tutorialObj[taskValue+1].position.x + xArrowOffset, tutorialObj[taskValue + 1].position.y + yArrowOffset, tutorialObj[taskValue + 1].position.z);
        arrowGO.SetActive(true);
    }


    // Set black circle (tutorial) image to direct towards current Task object //
    public void UpdateCanvasCirclePos()
    {
        Vector3 pos = mainCamGO.GetComponent<Camera>().WorldToScreenPoint(tutorialObj[taskValue].position);
        tutorialCircleTrans.transform.position = pos;
    }
    #endregion

    #region Check Tutorial Function
    // Call every time we are moving onto another task //
    public void CheckTutorialFunc()
    {
        if (!checkOnce)
        {
            if (taskValue == endTaskValue)
            {
                //Debug.Log("END");
                StartCoroutine(CheckEnd());
            } else
            {
                blackCircleAnim.Play("ScaleUpTutorialImg02");

                tutorialText.text = "" + tutorialStringTexts[taskValue];

                if (isUsingCameraAnims)
                    camAnim.Play(camString[taskValue]);
 
                UpdateCanvasCirclePos();
                UpdateArrowPos();

                StartCoroutine(WaitCheck());
            }

            taskValue++;
            //Debug.Log("CHECK TUTORIAL FUNC");
            //Debug.Log("TASK VALUE = " + taskValue);
        }

    }

    IEnumerator WaitCheck()
    {
        checkOnce = true;
        yield return new WaitForSeconds(1);

        UpdateCanvasCirclePos();

        if (isHydraulics)
        {
            if (taskValue == 1)
            {
                ActionOne?.Invoke();
            }
            else if (taskValue == 2)
            {
                ActionTwo?.Invoke();
            }
        }

        checkOnce = false;
    }

    private bool checkOnce;

    IEnumerator CheckEnd()
    {

        statusImg.sprite = statusSprites[0];
        statusImg.color = greenColor;

        tutorialText.text = "" + tutorialStringTexts[taskValue];

        if (isUsingCameraAnims)
            camAnim.Play(camString[taskValue]);

        checkOnce = true;

        arrowGO.SetActive(false);

        blackCircleAnim.Play("ScaleUpTutorialImg01");
        //tutorialCanvasAnim.Play("FadeCanvasOut");
        yield return new WaitForSeconds(1);
        sceneCameraGO.SetActive(true);
        gameObject.SetActive(false);

        tutorialActive = false;
        TutorialFinished?.Invoke();
    }
    #endregion
}
