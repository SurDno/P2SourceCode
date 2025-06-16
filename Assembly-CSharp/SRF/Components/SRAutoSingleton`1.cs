// Decompiled with JetBrains decompiler
// Type: SRF.Components.SRAutoSingleton`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace SRF.Components
{
  public abstract class SRAutoSingleton<T> : SRMonoBehaviour where T : SRAutoSingleton<T>
  {
    private static T _instance;

    public static T Instance
    {
      [DebuggerStepThrough] get
      {
        if ((Object) SRAutoSingleton<T>._instance == (Object) null && Application.isPlaying)
          new GameObject("_" + typeof (T).Name).AddComponent<T>();
        return SRAutoSingleton<T>._instance;
      }
    }

    public static bool HasInstance => (Object) SRAutoSingleton<T>._instance != (Object) null;

    protected virtual void Awake()
    {
      if ((Object) SRAutoSingleton<T>._instance != (Object) null)
        UnityEngine.Debug.LogWarning((object) "More than one singleton object of type {0} exists.".Fmt((object) typeof (T).Name));
      else
        SRAutoSingleton<T>._instance = (T) this;
    }

    private void OnApplicationQuit() => SRAutoSingleton<T>._instance = default (T);
  }
}
