using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCloudSave : MonoBehaviour, ICloudSaving
{
    /// <summary>
    /// This is a dummy class to mock out the save/load calls for testing.
    /// </summary>
    public void Save()
    {
        return;
    }
  
    public void Load()
    {
        return;
    }
    public void Delete()
    {
        return;
    }
}
