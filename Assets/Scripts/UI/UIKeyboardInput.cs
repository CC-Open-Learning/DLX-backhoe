using Lean.Gui;
using Lean.Transition.Method;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RemoteEducation.UI
{
	public class UIKeyboardInput : MonoBehaviour
	{
		public KeyCode[] NextKeys = new KeyCode[3] { KeyCode.Tab, KeyCode.DownArrow, KeyCode.RightArrow };
		public KeyCode[] PreviousKeys = new KeyCode[2] { KeyCode.UpArrow, KeyCode.LeftArrow };
		public KeyCode[] BackKeys = new KeyCode[1] { KeyCode.Escape };

		public UnityEvent backEvent;

		private const int NONE = -1;

		private List<Selectable> selectables;

		private int selectedIndex;

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				if (value >= NONE && value <= selectables.Count - 1)
				{
					if (SelectedIndex != NONE)
					{
						if (selectables[SelectedIndex] is LeanButton)
						{
							selectables[SelectedIndex].transform.Find("ExitTransition").
								gameObject.GetComponent<LeanGraphicColor>().BeginThisTransition();
						}
					}

					selectedIndex = value;

					if (value == NONE)
					{
						EventSystem.current.SetSelectedGameObject(null);
					}
					else
					{
						selectables[SelectedIndex].Select();

						if (selectables[SelectedIndex] is LeanButton)
						{
							selectables[SelectedIndex].transform.Find("EnterTransition").
								gameObject.GetComponent<LeanGraphicColor>().BeginThisTransition();
						}
					}
				}
			}
		}

		// Start is called before the first frame update
		void Start()
		{
			selectables = new List<Selectable>();
			FindSelectableChildren(transform);

			selectedIndex = NONE;
		}


		private void FindSelectableChildren(Transform searchTransform)
		{
			foreach (Transform current in searchTransform)
			{
				Selectable selectable;

				if ((selectable = current.gameObject.GetComponent<Selectable>()) != null)
				{
					selectables.Add(selectable);
				}
				else if (current.childCount > 0)
				{
					FindSelectableChildren(current);
				}
			}
		}

		// Update is called once per frame
		void Update()
		{
			CheckForInput();
		}

		private void CheckForInput()
		{
			foreach (KeyCode current in NextKeys)
			{
				if (Hotkeys.GetKeyDown(current))
				{
					IncrementSelection();
				}
			}

			foreach (KeyCode current in PreviousKeys)
			{
				if (Hotkeys.GetKeyDown(current))
				{
					DecrementSelection();
				}
			}

			foreach (KeyCode current in BackKeys)
			{
				if (Hotkeys.GetKeyDown(current))
				{
					if (backEvent != null)
					{
						backEvent.Invoke();
					}
				}
			}
		}

		private void IncrementSelection()
		{
			// if selected index would overflow list set back to 0, otherwise increment
			if (SelectedIndex >= selectables.Count - 1)
			{
				SelectedIndex = 0;
			}
			else
			{
				SelectedIndex += 1;
			}

			if (!selectables[SelectedIndex].gameObject.activeSelf ||
				!selectables[SelectedIndex].transform.parent.gameObject.activeSelf)
			{
				IncrementSelection();
			}
		}

		private void DecrementSelection()
		{
			// if selected index would overflow list set back to last element, otherwise decrement
			if (SelectedIndex <= 0)
			{
				SelectedIndex = selectables.Count - 1;
			}
			else
			{
				SelectedIndex -= 1;
			}

			if (!selectables[SelectedIndex].gameObject.activeSelf ||
				!selectables[SelectedIndex].transform.parent.gameObject.activeSelf)
			{
				DecrementSelection();
			}
		}
	}
}