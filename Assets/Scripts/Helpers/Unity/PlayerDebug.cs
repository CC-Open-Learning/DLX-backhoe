/*
FILENAME		: PlayerDebug.cs
PROJECT			: JWC Project
PROGRAMMERS		: Andy Sgro
FIRST VERSION	: Nov 8, 2019
DESCRIPTION		: This script allows programmers to see logs in realtime while their headset is on.

				  All logs are key-value pairs.
					- When the key represent a variable name, the value represent the variable's value.
					- When the key repentets an event, the value represents the the number of times
					  it's occured.

				  Attach this to a canvas that's parented to the OVRPlayerController's centerEyeAnchor.
				  Make sure that the child of this game object has a few TextMeshPro Text blocks
				  (doesn't matter how many, just as long as there is at least one) 
				  otherwise this script will output a logError specifying an initilization error.
*/

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RemoteEducation.Extensions;

/**
* NAME	  : PlayerDebug
* PURPOSE : 
*	- This script allows programmers to see logs in realtime while their headset is on.
*/
public class PlayerDebug : MonoBehaviour
{
	//*****************************//
	// fields + enums + properties //
	//*****************************//

	[SerializeField]
	private bool visible = false;
	[SerializeField]
	private TextMeshProUGUI[] textboxes = null;

	// singlton fields
	private static PlayerDebug m = null; // singleton instance
	private static int instanceCount = 0;

	private bool prevVisible = true;
	public static int Count { get { return m.keys.Count; } }
	private List<string> keys = new List<string>();
	private List<string> values = new List<string>();
	private List<float> updateTimes = new List<float>();
	private float alpha = 0;

	/**
	* \brief	Checks if this script has the required components.
	*/
	private static bool IsValid
	{
		get { return (m.textboxes.Length > 0); }
	}

	//***********************//
	// monobehaviour methods //
	//***********************//

	/**
	* \brief	Gets all of the textboxes.
	* 
	*			NOTE: This was recently changed to 'Awake()'.
	*				  Change back to 'Start()' if needed.
	*/
	private void Awake()
	{
		m = this;
		++instanceCount;

		if (IsValid)
		{
			alpha = textboxes[0].alpha;
			Clear();
		}
		else
		{
			Debug.LogError("The PlayerDebug script cannot work because there are no child objects with TextMeshProUGUI scripts," +
				"or because there is more than one instance of the PlayerDebug component.");
		}
	}


	/**
	* \brief	Updates the textboxes.
	*/
	private void LateUpdate()
	{
		if (IsValid)
		{
			if (visible != prevVisible)
			{
				foreach (TextMeshProUGUI textbox in textboxes)
				{
					textbox.alpha = visible ? alpha : 0;
				}
			}

			for (int i = 0; i < keys.Count; ++i)
			{
				textboxes[i].text = keys[i] + values[i];
			}

			prevVisible = visible;
		}
	}

	//***************************//
	// non-monobehaviour methods //
	//***************************//

	/**
	* \brief	Logs a message to the debug canvas.
	* 
	* \param	string variableName : The variable / event that's being logged. 
	* \param	object value = null : The values / details associated with the variable / event.
	* 
	* \return	Returns the number of logs that are currently being shown on the screen.
	*/
	public static int Log(string variableName, object value = null)
	{
		if (IsValid)
		{
			int foundIndex = -1;
			string strValue = (value != null) ? value.ToString() : "";

			// if the value isn't null, use the 'varName: value' format
			//    if the value is null, use the 'varName  (<repeat count>)' format
			if (value != null)
			{
				strValue = " " + strValue;
				variableName += ":";
				foundIndex = m.keys.IndexOf(variableName);
			}
			else
			{
				variableName += " ";
				foundIndex = m.keys.IndexOf(variableName);

				// if log has happened before, append ' (<repeat count>)' to value
				if (foundIndex >= 0)
				{
					string foundValue = m.values[foundIndex];
					int repeatCount = 0;
					bool canParse = foundValue.GetInt(out repeatCount);

					if (canParse)
					{
						strValue = " (" + (repeatCount + 1) + ")";
					}
					else // this else branch is 99% not needed, but keep it here just in case
					{
						strValue = " (0)";
					}
				}
				// if the log hasn't happened before, append ' (0)' to value
				else
				{
					strValue = " (0)";
				}
			}

			// if the variable has already been logged, replace the value
			if (foundIndex >= 0)
			{
				ReplaceValue(foundIndex, strValue);
			}
			// if the variable has not been logged, and there are less than 5 logs,
			// add a new log
			else if (m.keys.Count < m.textboxes.Length)
			{
				AddNewLog(variableName, strValue);
			}
			// if the variable has not been logged, and all the logs have been filled up,
			// replace the oldest log
			else
			{
				ReplaceOldLog(variableName, strValue);
			}
		}

		return m.keys.Count;
	}

	/**
	* \brief	Replaces the value portion of a log. (Logs are key-value pairs.)
	* 
	* \param	int index    : The index of the log to replace. 
	* \param	string value : The new value to replace it with.
	*/
	private static void ReplaceValue(int index, string value)
	{
		m.values[index] = value;
		m.updateTimes[index] = Time.realtimeSinceStartup;
	}

	/**
	* \brief	Logs a formatted error.
	* 
	* \param	Component thisScript     : The script that is calling this function.
	* \param	GameObject sourceOfError : The GameObject that 'thisScript' is attached to.
	* \param	string documentation     : The page / line numbers that contain the correct
	*									   implementation instructions of the Component.
	*/
	public static void LogError(Component thisScript, GameObject sourceOfError, string documentation)
	{
		Debug.LogError(sourceOfError.name + "'s " + thisScript.name + " component can't function because of bad serialized fields. " +
				" Look at the " + documentation + " to see how the serialized fields should be.");
	}


	/**
	* \brief	Adds a new log to the screen.
	* 
	* \param	string variableName : The variable / event that's being logged. 
	* \param	string value        : The values / details associated with the variable / event.
	*/
	private static void AddNewLog(string variableName, string value)
	{
		m.keys.Add(variableName);
		m.values.Add(value);
		m.updateTimes.Add(Time.realtimeSinceStartup);
	}

	/**
	* \brief	Replaces the oldest log that's on the screen with a new one.
	* 
	* \param	string variableName : The variable / event that's being logged. 
	* \param	string value        : The values / details associated with the variable / event.
	*/
	private static void ReplaceOldLog(string variableName, string value)
	{
		int indexToUpdate = m.updateTimes.IndexOf(m.updateTimes.Min());
		m.keys[indexToUpdate] = variableName;
		m.values[indexToUpdate] = value;
		m.updateTimes[indexToUpdate] = Time.realtimeSinceStartup;
	}

	private static void Clear()
	{
		for (int i = 0; i < m.keys.Count; ++i)
		{
			m.keys[i] = "";
		}

		for (int i = 0; i < m.values.Count; ++i)
		{
			m.values[i] = "";
		}
	}
	
}
