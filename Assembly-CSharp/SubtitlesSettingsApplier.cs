// Decompiled with JetBrains decompiler
// Type: SubtitlesSettingsApplier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Settings;
using System;

#nullable disable
public class SubtitlesSettingsApplier : EngineDependent
{
  protected override void OnConnectToEngine()
  {
    this.Apply();
    InstanceByRequest<SubtitlesGameSettings>.Instance.OnApply += new Action(this.Apply);
  }

  protected override void OnDisconnectFromEngine()
  {
    InstanceByRequest<SubtitlesGameSettings>.Instance.OnApply -= new Action(this.Apply);
  }

  private void Apply()
  {
    ServiceLocator.GetService<SubtitlesService>().SubtitlesEnabled = InstanceByRequest<SubtitlesGameSettings>.Instance.SubtitlesEnabled.Value;
    ServiceLocator.GetService<SubtitlesService>().DialogSubtitlesEnabled = InstanceByRequest<SubtitlesGameSettings>.Instance.DialogSubtitlesEnabled.Value;
  }
}
