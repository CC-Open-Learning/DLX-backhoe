/*
 *  FILE          :	FadeCanvasOut.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Kastriot Sulejmani
 *  FIRST VERSION :	2020-11-07
 *  
 *  DESCRIPTION   : 
 *		Play "FadeCanvasOut" animation ... you need a canvas group component on this object in order for fade to work
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvasOut : MonoBehaviour
{
    // Get currently attached gameobject animation //
    private Animation attachedCanvasAnim;

    // Start is called before the first frame update
    void Start()
    {
        // Set currently attached gameobject anim //
        attachedCanvasAnim = gameObject.GetComponent<Animation>();

        // Play FadeCanvasIn anim //
        attachedCanvasAnim.Play("FadeCanvasOut");
    }
}
