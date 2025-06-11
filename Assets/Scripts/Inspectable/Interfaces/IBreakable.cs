namespace RemoteEducation.Scenarios.Inspectable
{
    /// <summary>This interface is will be used for any classes that need to work with <see cref="DynamicInspectableElement"/>s.</summary>
    public interface IBreakable
    {
        /// <summary>
        /// Attach the <see cref="DynamicInspectableElement"/> to the class that will need to interface with it.
        /// This will provide the reference to the element, it will also give the class a chance to initialize 
        /// itself for the inspectable state it is in.
        /// </summary>
        /// <param name="inspectableElement">The element that corresponds to object</param>
        /// <param name="broken">If the object should be broken.</param>
        /// <param name="breakMode">If the object has more than one way to be broken, the mode is specified here.</param>
        void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode);
    }
}