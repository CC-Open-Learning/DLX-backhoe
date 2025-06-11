/*
 *  FILE          :	ProgressBar.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Michael Hilts
 *  FIRST VERSION :	2019-10-28
 *  DESCRIPTION   : This file contains the Progressbar class which is responsible for
 *					displaying the scores presented to it.
 */

#region Using Statements

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

/*
 * CLASS:       ProgressBar
 * DESCRIPTION: This class is responsible for displaying the scores presented to it.
 */

public class ProgressBar : MonoBehaviour
{
	[Tooltip("The final score to display in the summary.")]
	[SerializeField]
	private float finalScore = 0;

	[Tooltip("Max score possible for this bar.")]
	[SerializeField]
	private float maxScore = 0;

	[Tooltip("How long the summary animation will take.")]
	[SerializeField]
	private float summaryDuration = 0;

	[Tooltip("The text displaying the score.")]
	[SerializeField]
	private TextMeshProUGUI fillText;

	[Tooltip("The bar image of the score.")]
	[SerializeField]
	private Image fill;
	
	private int score;					// the current score
	private float summaryCounter;		// times the summary
	private bool startSummary;			// whether or not to start the summary
	private float percentComplete;		// current % of summary complete
	private float finalPercent;         // the final percent to display to for un perfect scores

	#region Properties

	// the currently displayed score
	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			if (value >= 0 && value <= maxScore)
			{
				score = value;
			}
		}
	}

	#endregion

	#region MonoBehaviour Callbacks

	/*
	 * FUNCTION    : Start()
	 * DESCRIPTION : Start is called once before the first frame update. It will get required
	 *				 references and calculate the finalPercent.
	 * PARAMETERS  :
	 *		VOID
	 * RETURNS     :
	 *		VOID
	 */

	void Start()
    {
		fill = gameObject.transform.Find("Fill").gameObject.GetComponent<Image>();
		percentComplete = 0f;
		fillText.text = "0 / " + maxScore.ToString("0.00");
    }

	/*
	 * FUNCTION    : Update()
	 * DESCRIPTION : Update is called once per frame and if it is able to it will start the summary.
	 * PARAMETERS  :
	 *		VOID
	 * RETURNS     :
	 *		VOID
	 */

	void Update()
    {
        if (startSummary)
		{
			Summarize();
		}
    }

	#endregion

	#region Private Methods

	/*
	 * FUNCTION    : Summarize()
	 * DESCRIPTION : This method will continually increase the bar and text as for the
	 *				 duration of the summary.
	 * PARAMETERS  :
	 *		VOID
	 * RETURNS     :
	 *		VOID
	 */

	private void Summarize()
	{

		if (percentComplete < finalPercent)
		{
			if (summaryDuration != 0)
			{
				percentComplete += (Time.deltaTime / summaryDuration) * finalPercent;
			}
			else
			{
				percentComplete = finalPercent;
				startSummary = false;
			}
			fillText.text = (percentComplete * maxScore).ToString("0.00") + " / " + maxScore.ToString("0.00");
			fill.fillAmount = percentComplete;
		}
		else
		{
			fillText.text = finalScore.ToString("0.00") + " / " + maxScore.ToString("0.00");
			startSummary = false;
		}
	}

	#endregion

	#region Public Methods

	/*
	 * FUNCTION    : StartSummary()
	 * DESCRIPTION : This method is used to begin the summary.
	 * PARAMETERS  :
	 *		VOID
	 * RETURNS     :
	 *		VOID
	 */

	public void StartSummary(float final, float max, float duration)
	{
		finalScore = final;
		maxScore = max;
		summaryDuration = duration;

		fillText.text = "0 / " + maxScore.ToString("0.00");

		if (maxScore != 0)
		{
			finalPercent = finalScore / maxScore;
		}
		else
		{
			finalPercent = 0;
		}

		startSummary = true;
	}

	public void ChangeFillColor(Color newColor)
	{
		fill.color = newColor;
	}

	public void SetFill(float fillPercent)
	{
		fill.fillAmount = fillPercent;
	}

	#endregion
}
