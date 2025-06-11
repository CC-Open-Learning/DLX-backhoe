using TMPro;
using UnityEngine;
using UnityEngine.UI;
using States = UserTaskVertex.States;

namespace RemoteEducation.Scenarios
{
	public class TaskBoardElement : MonoBehaviour
	{
        public States State
        {
            get
            {
                return Task.State;
            }
        }

        public UserTaskVertex Task { get; set; }

        /* These need to be manually assigned within prefab						 */
        /* instead of using Awake(), for the flexibility of the prefab structure */
        /* From here :															 */
        public Image Background;			// backgound ui of the task element
		public TextMeshProUGUI TaskText;    // task title displayed
		public TextMeshProUGUI TaskDescriptionText;    // task description displayed

		public GameObject PassImage;        // image displayed when completed task
		public GameObject FailImage;		// image displayed when failed task
		public GameObject WarnImage;        // image displayed when undo completed task
		public GameObject ThumbUpImage;
		public GameObject ThumbDownImage;
		/*																 To here */

		private bool IconLocked;

        public enum IconType
        {
			Pass,
			Fail,
			Warn,
			ThumbUp,
			ThumbDown,
			Blank
		}

        public void SetTaskDetails(UserTaskVertex task)
        {
            Task = task;
            TaskText.text = task.Title;
            TaskDescriptionText.text = task.Description;
        }

        public void SetIcon(IconType type)
        {
			// This check is done to prevent stale UserTaskVertex objects from
			// attempting to update the UI when they should be destroyed
			// TODO: fix this so that the delegates are not invoked on
			// UserTaskVertex objects when they are not actually in the Scenario
			if (!PassImage)
            {
				return;
            }

			if(!IconLocked)
            {
				PassImage.SetActive(false);
				FailImage.SetActive(false);
				WarnImage.SetActive(false);
				ThumbUpImage.SetActive(false);
				ThumbDownImage.SetActive(false);

				switch (type)
				{
					case IconType.Fail:
						FailImage.SetActive(true);
						break;

					case IconType.Pass:
						PassImage.SetActive(true);
						break;

					case IconType.ThumbDown:
						ThumbDownImage.SetActive(true);
						break;

					case IconType.ThumbUp:
						ThumbUpImage.SetActive(true);
						break;

					case IconType.Warn:
						WarnImage.SetActive(true);
						break;
				}
			}
		}

		public void ToggleIconLocked(bool lockedOrNot)
        {
			IconLocked = lockedOrNot ? true : false;
        }

		public void Indent(int indentSize)
        {
			RectTransform backgroundTransform = transform.Find("Background") as RectTransform;

			backgroundTransform.offsetMin = new Vector2(indentSize, backgroundTransform.offsetMin.y);
        }

		public void Deindent()
        {
			RectTransform backgroundTransform = transform.Find("Background") as RectTransform;

			backgroundTransform.offsetMin = new Vector2(0, backgroundTransform.offsetMin.y);
		}
    }
}
