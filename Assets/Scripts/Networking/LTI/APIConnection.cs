/*
 *  FILE          :	APIConnection.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Jacob Nelson
 *  FIRST VERSION :	2020-11-25
 *  DESCRIPTION   : 
 */

using System.Collections;
using UnityEngine;
using RemoteEducation.Networking.LTI;

public class APIConnection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GradeButtonPress()
    {
        StartCoroutine(SendGradeAsync());
    }

    public IEnumerator SendGradeAsync()
    {
        yield return null;

        System.Random rnd = new System.Random();
        APIGrades.SendTestGrade(rnd.Next(0, 99));
    }
}