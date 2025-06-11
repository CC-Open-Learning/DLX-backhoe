/*
 *  FILE          : ScoreText.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    : Michael Hilts, David Inglis
 *  FIRST VERSION :	2020-2-8
 *  DESCRIPTION   : 
 *		The ScoreText class is responsible for displaying the final line of the summary which
 *		includes either pass or fail and the completed tasks out of total tasks.
 *		
 *		This class is in need of some method comments, and needs some better definition of its use
 */

#region Resources

using RemoteEducation.Localization;
using TMPro;
using UnityEngine;

#endregion

namespace RemoteEducation.Scenarios
{
	public class ScoreText : MonoBehaviour
	{
		private string passText = "Complete";
		private string failText = "Incomplete";


		#region Fields

		[Tooltip("Indicate whether the result is passing")]
		[SerializeField] private bool pass;

		[Tooltip("Text color when user has passed the task")]
		public Color PassColor = Color.green;

		[Tooltip("Text color when user has failed the task")]
		public Color FailColor = Color.red;


		[Tooltip("The text component which is show either pass or fail.")]
		[SerializeField]
		private TextMeshProUGUI textField = null;

		[Tooltip("The text component which will show the users score out of the total.")]
		[SerializeField]
		private TextMeshProUGUI scoreField = null;

        #endregion

        #region Properties

        public bool Pass
        {
            get { return pass; }
            set
            {
                pass = value;

                if (textField)
                {
                    textField.text = pass ? passText : failText;
                    textField.color = pass ? PassColor : FailColor;
                }
            }
        }

        public void Awake()
        {
			passText = Localizer.Localize("Engine.Results.Pass");
			failText = Localizer.Localize("Engine.Results.Fail");
		}

        public void SetScore(float percentage)
		{
			if (!scoreField) { return; }

			scoreField.text = string.Format("{0:0.00}%", percentage);
		}

		public void SetScore(int score, int total, bool showPercentage = false, bool integerPercentage = false)
		{
			if (!scoreField) { return; }

			if (showPercentage)
			{
				if (integerPercentage)
				{
					scoreField.text = string.Format(Localizer.Localize("Engine.Results.Score"), score, total, (score / total) * 100);
				}
				else
				{
					scoreField.text = string.Format(Localizer.Localize("Engine.Results.Score"), score, total, (score / (float)total * 100));
				}
			}
			else
			{
				scoreField.text = string.Format(Localizer.Localize("Engine.Results.ScoreNoPercent"), score, total);
			}
		}

		#endregion
	}

}