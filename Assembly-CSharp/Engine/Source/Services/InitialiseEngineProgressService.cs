// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.InitialiseEngineProgressService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Menu.Main;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (InitialiseEngineProgressService)})]
  public class InitialiseEngineProgressService
  {
    public int Progress { get; set; }

    public int Count { get; private set; }

    public void Begin(int count)
    {
      this.Progress = 0;
      this.Count = count;
      LoadWindow.Instance.Progress = ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
      LoadWindow.Instance.ShowProgress = true;
    }

    public void Update(string title, string info)
    {
      float maxLoaderProgress = ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
      LoadWindow.Instance.Progress = Mathf.Clamp01((float) ((double) (this.Progress + 1) / (double) this.Count * (1.0 - (double) maxLoaderProgress)) + maxLoaderProgress);
    }

    public void End()
    {
      LoadWindow.Instance.ShowProgress = false;
      LoadWindow.Instance.Progress = 0.0f;
    }
  }
}
