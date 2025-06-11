
public class ReliantEdge : GraphEdge
{
    public const int NoStepCountLimit = int.MaxValue;

    //how many steps should be taken beyond the first vertex
    public int StepCount;

    //if this edge should be checked again after it is complete. 
    public bool OnlyCompleteOnce;

    /// <summary>
    /// When checking reliant edges on a vertex, sub edges will only be check if all the other reliant edges are complete.
    /// </summary>
    public bool IsSubEdge;

    public ReliantEdge(GraphVertex source, GraphVertex target, int stepCount = NoStepCountLimit) : base(source, target)
    {
        EdgeType = EdgeTypes.Reliant;
        StepCount = stepCount;
    }

    public static ReliantEdge CreateSubEdge(GraphVertex source, GraphVertex target, bool passData)
    {
        ReliantEdge edge = new ReliantEdge(source, target);
        
        edge.PassVertexData = passData;
        edge.OnlyCompleteOnce = true;
        edge.IsSubEdge = true;

        return edge;
    }
}
