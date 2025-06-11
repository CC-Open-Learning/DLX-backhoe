/*
 *  FILE            : ScenarioData.cs
 *  PROJECT         : CORE (Config)
 *  PROGRAMMER      : Kieron Higgs, David Inglis, adapted from ContentOrder.cs by Michael Hilts
 *  UPDATED VERSION : 2020-07-15
 *  DESCRIPTION     : This class holds imported Scenario data for use in instantiated CORE Scenarios. Its structure is also used for instantiating
 *                    scenario buttons on the New Game screen.
 */

using UnityEngine;
using System.Collections.Generic;

namespace RemoteEducation.Scenarios
{
    public class ScenarioData : MonoBehaviour
    {
        public List<Scenario> scenarios;
    }
}