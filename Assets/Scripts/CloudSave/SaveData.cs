using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using VARLab.CloudSave;


[CloudSaved]
[JsonObject(MemberSerialization.OptIn)]
public class SaveData : MonoBehaviour
{
    //USER PROGRESS
    [JsonProperty]
    public List<ProgressData> AllElements; // This is a dictionary for each element the key will be the FullName of property of the InspectableElement.

    [JsonProperty]
    public int vertexID = 0; //This may also be used for tracking graphs putting both in just in case

    //INITIALIZATION
    [JsonProperty]
    public List<string> badElements;

    //DEBRIS GROUPS CLEAR STATE
    [JsonProperty]
    public List<bool> debrisGroupStates;
}

[JsonObject(MemberSerialization.OptIn)]
public class ProgressData
{
    [JsonProperty]
    public string ElementName;

    [JsonProperty]
    public int CurrentStatus;

    [JsonProperty]
    public int UserSelectedIdentifier;

    [JsonProperty]
    public int StateIdentifier;

    [JsonProperty]
    public int State;

    [JsonProperty]
    public bool currentlyInspectable;
}
