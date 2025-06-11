/*
 *  FILE          :	InstructionPage.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Michael Hilts, David Inglis
 *  FIRST VERSION :	2019-10-20
 *  DESCRIPTION   : 
 *		This file contains the InstructionPage class which is 
 *		responsible for displaying information on the InstructionCanvas.
 */

#region Using Statements

using TMPro;
using UnityEngine;

#endregion

/*
* CLASS:       InstructionPage
* DESCRIPTION: This class is responsible for displaying information on the instructionCanvas.
*/

[System.Obsolete("InstructionPage has replaced with the LabInstructions class")]
public class InstructionPage : MonoBehaviour {
	// No references found, commented out to suppress warning.
	//[Tooltip("true if test mode for resultsscene")]
	//[SerializeField]
	//private bool test = false;						

	[Tooltip("the title text component")]
	[SerializeField]
	private TextMeshProUGUI titleText;    
	
	[Tooltip("the title escription text component")]
	[SerializeField]
	private TextMeshProUGUI titleDescriptionText;   

	[Tooltip("the taskname text components")]
	[SerializeField]
	private TextMeshProUGUI[] taskNameText;        
	
	[Tooltip("the task description text components")]
	[SerializeField]
	private TextMeshProUGUI[] taskDescriptionText;

	private int pageNumber;                         // this pages number

	#region Properties

	// accessor to get this pages number
	public int PageNumber
	{
		get { return PageNumber; }
	}

	#endregion

	#region Public Methods

	/*
	* FUNCTION    : Format()
	* DESCRIPTION : Formats the page, if page 0 as a title page, otherwise
	*				 as a regular page.
	* PARAMETERS  :
	*		ScenarioFlags scenario: the scenario this page belongs to
	*		int pageNumber: the number for this page
	* RETURNS     :
	*		VOID
	*/

	public void Format(string name, string description)
	{
		//add descriptions and names then set the page inactive
		taskNameText[0].text = name;
		taskDescriptionText[0].text = description;
	}

	#endregion

	#region Private Methods

	/*
	* FUNCTION    : Format()
	* DESCRIPTION : Formats the page as a title page for the instructionCanvas.
	* PARAMETERS  :
	*		ScenarioFlags scenario: the scenario this page belongs to
	* RETURNS     :
	*		VOID
	*/
	public void Format(string name, string description, string nameTitle, string descriptionTitle)
	{
		//add descriptions and names then set the page inactive
		taskNameText[0].text = name;
		taskDescriptionText[0].text = description;

		titleText.text = nameTitle;
		titleDescriptionText.text = descriptionTitle;
	}

	/*
	* FUNCTION    : NormalFormat()
	* DESCRIPTION : Formats the page as a regular sop canvas page.
	* PARAMETERS  :
	*		ScenarioFlags scenario: the scenario this page belongs to
	*		int pageNumber: the number for this page
	* RETURNS     :
	*		VOID
	*

	private void NormalFormat(ScenarioFlags scenario, int pageNumber)
	{
		// get description and names list
		string[] descriptions = ScoreKeeper.Instance.GetDescriptions(scenario, ScoreType.Task);
		string[] names = ScoreKeeper.Instance.GetNames(scenario, ScoreType.Task);
		
		//add descriptions and names then set the page inactive
		taskNameText[0].text = names[pageNumber - 1];
		taskDescriptionText[0].text = descriptions[pageNumber - 1];
		gameObject.SetActive(false);
	}*/

	#endregion
}
