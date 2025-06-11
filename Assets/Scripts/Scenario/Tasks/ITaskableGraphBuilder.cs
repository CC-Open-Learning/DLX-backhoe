using RemoteEducation.Scenarios;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITaskableGraphBuilder
{
    //A method that will build the graph 
    CoreGraph BuildGraph(out GraphVertex start, out GraphVertex end, IExtensionModule extensionModule); //extension 
}
