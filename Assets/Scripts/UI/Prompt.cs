
using UnityEngine;

namespace RemoteEducation.UI 
{
    /// <summary>
    ///     Base class for various informational prompts displayed throughout CORE Engine
    /// </summary>
    public class Prompt : MonoBehaviour 
    {

        public enum Type 
        {
            Arrow,
            Breadcrumb,
            Message
        }
        
        public Type PromptType;
    }
}
