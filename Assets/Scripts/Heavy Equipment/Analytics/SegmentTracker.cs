using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentTracker : MonoBehaviour
{
    private int _segmentIndex;

    void Start()
    {
        _segmentIndex = 1;
    }

    public void IncreamentSegmentIndex()
    {
        _segmentIndex++;
    }

    public int GetSegmentIndex()
    {
        return _segmentIndex;
    }

    private void OnDestroy()
    {
        _segmentIndex = 1;
    }
}
