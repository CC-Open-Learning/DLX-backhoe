namespace RemoteEducation.UserReporting
{
    /// <summary>Represents a user reporting state.</summary>
    public enum UserReportingState
    {
        Idle = 0,
        CreatingUserReport = 1,
        ShowingForm = 2,
        SubmittingForm = 3
    }
}