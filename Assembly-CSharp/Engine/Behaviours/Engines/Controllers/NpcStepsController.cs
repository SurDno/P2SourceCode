// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.NpcStepsController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Source.Behaviours.Controllers;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Behaviours.Engines.Controllers
{
  [DisallowMultipleComponent]
  public class NpcStepsController : StepsController
  {
    private Pivot pivot;

    [Inspected]
    private bool Indoor { get; set; }

    [Inspected]
    protected override AudioMixerGroup FootAudioMixer
    {
      get
      {
        return this.Indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcFootIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcFootOutdoorMixer;
      }
    }

    [Inspected]
    protected override AudioMixerGroup FootEffectsAudioMixer
    {
      get
      {
        return this.Indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcFootEffectsIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcFootEffectsOutdoorMixer;
      }
    }

    protected void Start()
    {
      this.pivot = this.gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        Debug.LogError((object) ("Please move " + typeof (NpcStepsController).Name + " to root of prefab and make sure " + typeof (Pivot).Name + " is also at the same level."), (UnityEngine.Object) this.gameObject);
      }
      else
      {
        this.pivot.GetAnimatorEventProxy().AnimatorEventEvent += new Action<string>(this.AnimatorEvent);
        this.pivot.IndoorChangedEvent += new Action<bool>(this.Pivot_IndoorChangedEvent);
        this.Pivot_IndoorChangedEvent(this.pivot.Indoor);
      }
    }

    protected void OnDestroy()
    {
      if (!((UnityEngine.Object) this.pivot != (UnityEngine.Object) null))
        return;
      this.pivot.IndoorChangedEvent -= new Action<bool>(this.Pivot_IndoorChangedEvent);
    }

    private void Pivot_IndoorChangedEvent(bool indoor)
    {
      this.Indoor = indoor;
      this.RefreshMixers();
    }

    protected void AnimatorEvent(string data)
    {
      string str = data;
      if (!(str == "Skeleton.Humanoid.Foot_Left") && !(str == "Skeleton.Humanoid.Foot_Right") || !DetectorUtility.CheckDistance(EngineApplication.PlayerPosition, this.transform.position, ExternalSettingsInstance<ExternalCommonSettings>.Instance.StepsDistance))
        return;
      this.OnStep(data, this.Indoor);
    }
  }
}
