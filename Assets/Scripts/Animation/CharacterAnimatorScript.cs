/*
 *  FILE          :	CharacterAnimatorScript.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Kastriot Sulejmani
 *  FIRST VERSION :	2020-11-05
 *  
 *  DESCRIPTION   : 
 *		This will be used for our butler / tutorial guy in order to animate him in the scene
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorScript : MonoBehaviour
{
    #region Animations, GameObjects
    // Character Animator //
    public Animator m_animator;

    // Other Animations //
    public Animation characterParentAnim;
    public Animation chatBoxAnim;

    public GameObject chatBoxGO;
    public GameObject worldCanvasGO;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Start Character Animations //
        StartCoroutine(EnterCharacter_Enum());
    }

    #region CharacterEnum
    IEnumerator EnterCharacter_Enum()
    {
        // Play character parent move anim into scene and animator walk anim //
        characterParentAnim.Play("MoveCharacter01");
        Character_Walk();

        // Wait until character walks into scene //
        yield return new WaitForSeconds(2.5f);

        // chat box gameobject activate true //
        chatBoxGO.SetActive(true);
        worldCanvasGO.SetActive(true);

        // Play character wave anim and chatbox anim //
        m_animator.ResetTrigger("isWalking");
        Character_Wave();

        chatBoxAnim.Play("ChatBoxScaleInAnim01");

        yield return new WaitForSeconds(3.5f);

        // Stop waving //
        m_animator.ResetTrigger("isWaving");
        chatBoxAnim.Play("ChatBoxScaleOutAnim01");

        Character_Texting();
    }
    #endregion

    #region Animations
    // Play Character Walk Anim //
    public void Character_Walk()
    {
        Debug.Log("isWalking");
        m_animator.SetTrigger("isWalking");
    }

    // Play Character Wave Anim //
    public void Character_Wave()
    {
        Debug.Log("isWaving");
        m_animator.SetTrigger("isWaving");
    }

    // Play Character Texting Anim //
    public void Character_Texting()
    {
        Debug.Log("isTexting");
        m_animator.SetTrigger("isTexting");
    }
    #endregion
}
