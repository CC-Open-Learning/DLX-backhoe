using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICloudSaving
{
    public void Save();
    public void Load();
    public void Delete();
}
