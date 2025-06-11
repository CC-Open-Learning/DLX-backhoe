#region Resources

using RemoteEducation.Networking.Telemetry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace RemoteEducation.Scenarios
{
	/// <summary>
	///		Connects to the main bootstrapping process of CORE Engine. Allows quick access 
	///		to the defined CORE Engine Scenarios.
	/// </summary>
	/// <remarks>
	///		This class no longer hooks into <see cref="LaunchEligibility"/> to enable/disable
	///		access to the CORE Engine application
	/// </remarks>
	public class ContentModuleAccess : MonoBehaviour
	{
		#region Properties

		[Tooltip("The name of the scene the simulation is to be run in.")]
		public string EnvironmentSceneName = "ScenarioScene";

		/// <summary>Collection of available Scenarios, loaded from the Scenario Configuration file</summary>
		private List<Scenario> scenarios;

		#endregion

		#region MonoBehaviour Callbacks

		/// <summary>
		///		Calls the LaunchHook method at the beginning of the MonoBehaviour life cycle
		/// </summary>
		private void Awake()
		{
			DetermineModuleStart();
		}

		#endregion

		#region Public Methods


		/// <summary>
		///		Determines if a Scenario identifier has been passed into CORE Engine
		///		through command-line arguments, then launches directly into that Scenario.
		///		<para />
		///		If no Scenario ID is present on the command-line arguments, then the
		///		application's main menu is made visible to the user
		/// </summary>
		public void DetermineModuleStart()
        {
			LoadConfiguration();

			// Determines if a Scenario ID was passed in as part of the URI as a
			// command-line argument, and launches the appropriate Scenario
			if (ScenarioBuilder.TryGetDefaultScenario(out int scenarioId))
			{
				Debug.Log("Loading Scenario " + scenarioId + " directly from StartScene");
				LoadScenarioScene(scenarioId);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		///		Attempts to retrieve a collection of <see cref="Scenario"/> objects using the <see cref="ScenarioBuilder"/>
		/// </summary>
		private void LoadConfiguration()
		{
			Debug.Log("ContentModuleQuickAccess : Loading config from file");

			scenarios = ScenarioBuilder.LoadFromFile();
		}


		/// <summary>
		///		Loads the Scenario scene based on the provided Scenario <paramref name="identifier"/>
		/// </summary>
		/// <param name="identifier">Integer corresponding to a particular CORE Engine Scenario</param>
		private void LoadScenarioScene(int identifier)
		{
			// Attempt to load from config again if there are no stored Scenarios
			if (scenarios.Count < 1)
			{
				LoadConfiguration();
			}

			// After attempting to load again, if there are still no stored Scenarios
			// then LoadScenarioScene should quit
			if (scenarios.Count < 1)
			{
				Debug.LogError("ContentModuleQuickAccess : No Scenarios loaded from configuration.");
				return;
			}

			// Grab the scenario which has been loaded from the config file whose identifier matches 'labNum'
			Scenario chosenScenario = scenarios.DefaultIfEmpty(null).FirstOrDefault(x => x.Identifier == identifier);

			// Create persistent Scenario data
			ScenarioBuilder.CreatePersistentScenarioData(chosenScenario);

			StartCoroutine(ScenarioManager.LazyLoad());
		}

		#endregion

	}
}