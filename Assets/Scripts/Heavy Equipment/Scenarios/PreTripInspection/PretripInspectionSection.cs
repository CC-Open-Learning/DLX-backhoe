using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PretripInspectionSection
{
    private GraphVertex head, tail;
    public GraphVertex Head => head;
    public GraphVertex Tail => tail;


    public PretripInspectionSection(GraphVertex _head, GraphVertex _tail)
    {
        head = _head;
        tail = _tail;
        SetupInspectionSection();
    }

    void SetupInspectionSection() { 
        
    }
}
