/// <summary>
///     Interface description: This interface allows the magnifier to interact with any class that uses it.
///     Basically, this is created for the MagnifierTexture to poke the task manager of a HydraulicComponentController.
/// </summary>
namespace RemoteEducation.Interactions
{
    public interface IMagnifierLockable
    {
        public bool MagnifierLocked { get; set; }

        public void LockInMagnifier();
    }
}
