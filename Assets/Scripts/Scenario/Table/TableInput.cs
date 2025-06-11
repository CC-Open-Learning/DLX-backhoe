///*
// *  FILE          :	TableInput.cs
// *  PROJECT       :	CORE Engine
// *  PROGRAMMER    :	Kieron Higgs
// *  FIRST VERSION :	2020-07-06
// *  DESCRIPTION   : OnClick() interface for dynamically instantiated TableInput prefab to connect to parent Table object
// *                  when the user enters text or numbers into a table's input fields, to check if they have all been filled.     
// */

//using UnityEngine;

//namespace RemoteEducation
//{
//    /*
//     * CLASS      : TableInput
//     * DESCRIPTION: 
//     *      Tiny class needed to access TableCanvas method via OnClick() of a TableInpu objects's child input field object
//     */
//    public class TableInput : MonoBehaviour
//    {
//        /*
//         * FUNCTION    : OnInput()
//         * DESCRIPTION : 
//         *      Calls TableCanvas.CheckInputFields() to assess whether all fields have been filled and the Submit button
//         *      should be activated.
//         */
//        public void OnInput()
//        {
//            transform.root.GetComponent<TableCanvas>().CheckInputFields();
//        }
//    }
//}