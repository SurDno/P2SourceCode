using Inspectors;

namespace Scripts.Inspectors
{
  public class UnityEngineInfo
  {
    private static UnityEngineInfo instance;

    public static UnityEngineInfo Instance
    {
      get
      {
        if (UnityEngineInfo.instance == null)
          UnityEngineInfo.instance = new UnityEngineInfo();
        return UnityEngineInfo.instance;
      }
    }

    [Inspected]
    private float Time => UnityEngine.Time.time;

    [Inspected]
    private float FixedTime => UnityEngine.Time.fixedTime;

    [Inspected]
    private float UnscaledTime => UnityEngine.Time.unscaledTime;

    [Inspected]
    private float FixedUnscaledTime => UnityEngine.Time.fixedUnscaledTime;

    [Inspected(Mutable = true)]
    private float TimeScale
    {
      get => UnityEngine.Time.timeScale;
      set => UnityEngine.Time.timeScale = value;
    }
  }
}
