using System.Collections;
using System.Diagnostics;
using VARLab.CloudSave;

public class SaveHandlerExample : ExperienceSaveHandler
{

    private Stopwatch m_Stopwatch;

    public override void Awake()
    {
        base.Awake();

        m_Stopwatch = new Stopwatch(); // Benchmarking timer.
    }


    protected override IEnumerator InternalSave()
    {
        m_Stopwatch.Restart();

        yield return base.InternalSave();

        m_Stopwatch.Stop();

        UnityEngine.Debug.Log($"Save completed in {m_Stopwatch.ElapsedMilliseconds}ms");
    }


    protected override IEnumerator InternalLoad()
    {
        m_Stopwatch.Restart();

        yield return base.InternalLoad();

        m_Stopwatch.Stop();

        UnityEngine.Debug.Log($"Load completed in {m_Stopwatch.ElapsedMilliseconds}ms");
    }



    protected override IEnumerator InternalDelete()
    {
        m_Stopwatch.Restart();

        yield return base.InternalDelete();

        m_Stopwatch.Stop();

        UnityEngine.Debug.Log($"Delete completed in {m_Stopwatch.ElapsedMilliseconds}ms");
    }
}
