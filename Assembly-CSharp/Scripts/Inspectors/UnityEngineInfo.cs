// Decompiled with JetBrains decompiler
// Type: Scripts.Inspectors.UnityEngineInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;

#nullable disable
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
