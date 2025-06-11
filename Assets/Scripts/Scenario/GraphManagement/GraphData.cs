
using System;
using UnityEngine;
using RemoteEducation.Extensions;

/// <summary>
/// This class is used to define the "data" that is held by the 
/// <see cref="GraphEdge"/> and <see cref="GraphVertex"/> classes.
/// This class provides a way to set and compare data that is slightly more
/// complex that just object = object and Equals(object, object). At the root
/// of this class, this is how it work. There are just a few steps that happen 
/// before this to add functionality that is specific for the graph.
/// 
/// There is still a single <see cref="object"/> that holds the data. When 
/// the class is created, the data type of this class can be locked. This means
/// that when ever any new data is passed in, if it doesn't match the defined 
/// data type, it will be ignored. If null is passed in and the data type is locked,
/// then the value is set to the default of that data type. 
/// 
/// There are also special cases that can be set for <see cref="GraphData"/> classes
/// inside a <see cref="GraphEdge"/>. These make it so that the <see cref="Compare(GraphVertex, GraphEdge)"/>
/// method checks differently than just using Equals(). These special cases are set 
/// in the constructor. For example, you can compare by Type. So if the data in the 
/// vertex is of the data type defined in the edge, true will be returned. 
/// In the future we can add more special cases. They must just be generic and not
/// tied to and specific module. 
/// </summary>
public class GraphData
{
    /// <summary>
    /// The public facing value. The setter will do the all checks needed for
    /// ensuring the data type remains as the locked on if that is needed.
    /// </summary>
    public object Value
    {
        get
        {
            return ValueInternal;
        }

        set
        {
            if (HasDefinedType)
            {
                //if the value is null. Reset the data based on its data type.
                if (value == null)
                {
                    if (DefinedType.IsClass)
                    {
                        //if its a referential type, set it to null
                        ValueInternal = value;
                    }
                    else
                    {
                        //this basically does "default(DefinedType)" but that doesn't work for some reason
                        ValueInternal = Activator.CreateInstance(DefinedType);
                    }
                }
                else
                {
                    if (value.GetType() == DefinedType)
                    {
                        ValueInternal = value;
                    }

                    //else ignore the new data
                }
            }
            else
            {
                ValueInternal = value;
            }
        }
    }

    private object ValueInternal;


    private bool HasDefinedType => DefinedType != null;
    public Type DefinedType { get; private set; }

    public enum EdgeSpecialCases
    {
        Default = 0,
        AlwaysPass = 101,
        NotNull = 102,
        PassByType = 103,
        Button = 104,
        GreaterThan_Int = 105,
        LessThan_Int = 106,
        GreaterThan_Float = 107,
        LessThan_Float = 108
    }

    public bool HasSpecialCase => SpecialCase != (int)EdgeSpecialCases.Default;
    private int SpecialCase; 

    public GraphData()
    {

    }

    public GraphData(object value, bool lockedDataType = true)
    {
        Value = value;

        DefinedType = lockedDataType ? value.GetType() : null;
    }

    public GraphData(int value, bool lockedDataType = true) : this((object)value, lockedDataType)
    {

    }

    public GraphData(EdgeSpecialCases specialCase, object data = null)
    {
        SpecialCase = (int)specialCase;

        switch (specialCase)
        {
            case EdgeSpecialCases.PassByType:
                DefinedType = data as Type;
                break;

            case EdgeSpecialCases.Button:

                //this one has the set up special, but its really just a regular edge
                SpecialCase = (int)EdgeSpecialCases.Default;

                //set the value using the Button graph interface prefix
                Value = ButtonGraphInterface.IdentifierPrefix + (data as string);

                DefinedType = typeof(string);
                break;

            case EdgeSpecialCases.GreaterThan_Int:
            case EdgeSpecialCases.LessThan_Int:
                
                try
                {
                    Value = (int)data;
                }
                catch (InvalidCastException)
                {
                    Debug.LogError("Special Case " + specialCase.ToString() + " was used, but the data passed in was invalid");
                }

                break;

            case EdgeSpecialCases.GreaterThan_Float:
            case EdgeSpecialCases.LessThan_Float:

                try
                {
                    ValueInternal = (float)data;
                }
                catch(InvalidCastException)
                {
                    Debug.LogError("Special Case " + specialCase.ToString() + " was used, but the data passed in was invalid");
                }

                break;
        }
    }

    public static bool Compare(GraphVertex vertex, GraphEdge edge)
    {
        //get the data from both components
        GraphData vertexData = vertex.Data;
        GraphData edgeData = edge.Data;

        if(vertexData.HasSpecialCase)
        {
            Debug.LogError("Vertices can not use special cases.");
        }

        //if there is no special case, just compare the data
        if(!edgeData.HasSpecialCase)
        {
            if(vertexData.Value != null && edgeData.Value != null)
            {
                //approximately compare floats
                if (vertexData.Value.GetType() == typeof(float) && edgeData.Value.GetType() == typeof(float))
                {
                    return ((float)vertexData.Value).ApproxEquals((float)edgeData.Value, 0.3f);
                }
            }
            // Uncomment to help debug vertex/edge data.
            /*if(vertexData.Value != null)
            {
                Debug.Log(vertexData.Value.GetHashCode());
            }

            if(edgeData.Value != null)
            {
                Debug.Log(edgeData.Value.GetHashCode());
            }*/

            return Equals(vertexData.Value, edgeData.Value);
        }

        //check if the special case is defined here
        if (Enum.IsDefined(typeof(EdgeSpecialCases), edgeData.SpecialCase))
        {
            switch (edgeData.SpecialCase)
            {
                case (int)EdgeSpecialCases.AlwaysPass:
                    return true;

                    //check that the data just isn't null
                case (int)EdgeSpecialCases.NotNull:

                    if (vertexData.Value.GetType().IsClass)
                    {
                        return vertexData.Value != null;
                    }
                    else
                    {
                        Debug.LogWarning("A \"Not Null\" edge case can not be tested on for a vertex which has data that is not a class type.");
                    }

                    return false;

                    //check if the data in the vertex is the of the right type (and not null) 
                case (int)EdgeSpecialCases.PassByType:
                    return vertexData.Value == null ? false : vertexData.Value.GetType() == edgeData.DefinedType;

                case (int)EdgeSpecialCases.GreaterThan_Int:

                    if(vertexData.Value is int && edgeData.Value is int)
                    {
                        return (int)vertexData.Value > (int)edgeData.Value;
                    }
                    else
                    {
                        return false;
                    }

                case (int)EdgeSpecialCases.LessThan_Int:

                    if (vertexData.Value is int && edgeData.Value is int)
                    {
                        return (int)vertexData.Value < (int)edgeData.Value;
                    }
                    else
                    {
                        return false;
                    }

                case (int)EdgeSpecialCases.GreaterThan_Float:

                    if (vertexData.Value is float && edgeData.Value is float)
                    {
                        return (float)vertexData.Value > (float)edgeData.Value;
                    }
                    else
                    {
                        return false;
                    }

                case (int)EdgeSpecialCases.LessThan_Float:

                    if (vertexData.Value is float && edgeData.Value is float)
                    {
                        return (float)vertexData.Value < (float)edgeData.Value;
                    }
                    else
                    {
                        return false;
                    }

                default:
                    Debug.LogWarning("A comparison has not been coded for this case yet : \"" + edgeData.SpecialCase.ToString() + "\"");
                    return false;
            }
        }
        else
        {
            //check if the edge has its own way of comparing data
            //  this might not be actually useful
            if (edge.AlternateDataCompare != null)
            {
                return edge.AlternateDataCompare(vertexData, edgeData);
            }
            else
            {
                Debug.LogError("The edge has a special case for data comparison defined, but did not provide a method for how to compare the data.");
                return false;
            }
        }
    }
}
