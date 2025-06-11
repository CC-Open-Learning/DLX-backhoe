using RemoteEducation.Scenarios;

/// <summary>
/// This class is used to define details about a task that the user completed.
/// This class does not have to be correlated to a <see cref="UserTaskVertex"/>. 
/// This is useful since the actually tasks vertices that make up a scenario may not have 
/// a 1 to 1 correlation to the grade-able outcomes for that scenario. For example: the 
/// inspection scenes have all the inspections done under 1 UserTaskVertex. This means that
/// only 1 task shows up in the Result Scene.
/// 
/// The <see cref="TaskManager"/> will be able to generate a list of these objects
/// to pass to the Result scene in <see cref="TaskManager.GenerateTaskResults"/>.
/// </summary>
public class TaskDetails
{
    public string Title;
    public string Description;
    public string Comments;

    public UserTaskVertex.States State;

    public TaskDetails(UserTaskVertex.States state, string title, string description, string comments = null)
    {
        Title = title;
        Description = description;
        State = state;
        Comments = comments; 
    }

    public TaskDetails()
    {

    }
}
