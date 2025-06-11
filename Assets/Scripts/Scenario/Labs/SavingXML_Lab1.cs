/*
 *  FILE          :	SavingXML_Lab2.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Slightly modified by Chowon Jung, based on Ivan Granic's SavingXML_Lab2.cs
 *  FIRST VERSION :	2020-08-11
 *  DESCRIPTION   : This file saves the contents of the menu for the Lab #1, including P2 and FR for each values required.
 *                  This file takes in raw text input from the sliding menu through each TextInput's Text 
 *                  game object and then taken from the text component.
 *                  
 *                  *NOTE*The resulting file should be used to send data to D2L, and should be loaded into scenarios 
 *                  separately from game data.
 *                  
 *                  *OVERLY CAUTIOUS/PARANOID NOTE* The XML should be checked before being uploaded to D2L as XML can 
 *                  be tampered with possibly causing unwanted XSS or other security problems, or should be encrypted
 *                  before saving and uploading to D2L.
 */

#region Using Statements

using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

#endregion

/*
 * CLASS:       SaveFile
 * DESCRIPTION: This class is a data structure used to hold an array of results called Results, 
 *              and sets the XML properties.
 */
public class SaveFile_Lab1
{
    [XmlArrayItem(Type = typeof(Result_Lab1))]
    public Result_Lab1[] Results;//breaking naming convention for array Results for easier XML reading. 
}
/*
 * CLASS:       Result
 * DESCRIPTION: This class is a data structure used to hold the 3 required 
 *              strings: GA1,GA2, and the comments from the user/student.
 */
public class Result_Lab1
{
    public string P2FieldString;
    public string FRFieldString;
}

/*
 * CLASS:       SavingXML_Lab2
 * DESCRIPTION: This class serializes and saves the input from the menu to an XML file in your documents folder 
 *              (at the moment, this should be extended to include deserialization later).
 */
public class SavingXML_Lab1 : MonoBehaviour
{
    //there will NEVER be anything different for the array sizes, thus should improve efficiency over sizeof() or any other method 
    private const int SIZE = 13;
    public GameObject[] P2FieldOBJ = new GameObject[SIZE];
    public GameObject[] FRFieldOBJ = new GameObject[SIZE];
    /*
     * FUNCTION    : SaveAsXML()
     * DESCRIPTION : Saves input from user into sliding menu for resultsto XML using XmlSerializer and a textwriter.
     * PARAMETERS  :
     *		VOID
     * RETURNS     :
     *		VOID
     */
    public void SaveAsXML()
    {
        SaveFile_Lab1 save = new SaveFile_Lab1();
        Result_Lab1[] resultArray = new Result_Lab1[SIZE];
        for (int j = 0; j < SIZE; j++)
        {
            resultArray[j] = new Result_Lab1();
        }


        for (int i = 0; i < SIZE; i++)
        {
            string text = P2FieldOBJ[i].GetComponent<Text>().text;
            resultArray[i].P2FieldString = text;
            //Debug.Log(save.Results[i].GA1FieldString);

            text = FRFieldOBJ[i].GetComponent<Text>().text;
            resultArray[i].FRFieldString = text;
            //Debug.Log(save.Results[i].GA2FieldString);
        }

        save.Results = resultArray;

        XmlSerializer s = new XmlSerializer(typeof(SaveFile_Lab1));

        var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SaveFile_Lab1.xml";
        TextWriter writer = new StreamWriter(path);

        s.Serialize(writer, save);
        writer.Close();
    }
}
