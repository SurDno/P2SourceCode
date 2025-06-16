using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Settings;
using System;

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
