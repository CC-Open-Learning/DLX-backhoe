using QuickGraph;

public class GraphEdge : Edge<GraphVertex>
{
    public enum EdgeTypes
    {
        Traversable,
        Reliant,
        DataCheck
    }

    public EdgeTypes EdgeType;
    public GraphData Data;

    public bool PassVertexData;

    //this could be used by child objects to define their own way of comparing the data, but
    // I'm not actually sure this will ever have to be used.
    public delegate bool AlternateDataCompareDelegate(GraphData vertex, GraphData edge);
    public AlternateDataCompareDelegate AlternateDataCompare;

    public GraphEdge(GraphVertex source, GraphVertex target) : base(source, target)
    {
        Data = new GraphData();
        PassVertexData = false;
    }

    public GraphEdge(GraphVertex source, GraphVertex target, GraphData data) : base(source, target)
    {
        Data = data;
        PassVertexData = false;
    }

}
