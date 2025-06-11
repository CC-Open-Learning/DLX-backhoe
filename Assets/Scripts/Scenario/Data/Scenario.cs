/*
 *  FILE          :	Scenario.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	David Inglis, Kieron Higgs
 *  FIRST VERSION :	2020-07-06
 *  DESCRIPTION   : 
 *      The Scenario class acts as a container for all 
 *      Tasks that make up the Scenario. 
 */

using System;
using System.Xml;
using System.Xml.Serialization;

namespace RemoteEducation.Scenarios
{
    [Serializable]
    [XmlType("Scenario")]
    [XmlRoot("Scenario")]
    public class Scenario
    {
        // Identifier constant used to indicate an invalid or empty Scenario
        public const int None = -1;

        [XmlAttribute("identifier")]
        public int Identifier;

        [XmlAttribute("title")]
        public string Title;

        [XmlAttribute("module")]
        public string Module;

        [XmlAttribute("defines")]
        public string Defines;

        [XmlAttribute("projectkey")]
        public string ProjectKey;

        // String which points to the environment prefab
        [XmlAttribute("environment")]
        public string Environment;

        // String which points to the environment prefab
        [XmlAttribute("taskGraph")]
        public string TaskGraphResourcePath;

        public string Description;
        public string PositiveComment;
        public string NegativeComment;

        [XmlElement]
        public object ModuleData;

        [XmlIgnore]
        public ITaskableGraphBuilder TaskGraph;

        [NonSerialized]
        public string OriginalFileName;
    }
}
