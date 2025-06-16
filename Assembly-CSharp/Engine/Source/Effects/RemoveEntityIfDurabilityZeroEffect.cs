// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.RemoveEntityIfDurabilityZeroEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Audio;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Connections;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RemoveEntityIfDurabilityZeroEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<AudioClip> removeSound;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<AudioMixerGroup> removeSoundMixer;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime) => true;

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      IEntity item = this.AbilityItem.Item;
      ParametersComponent component = item.GetComponent<ParametersComponent>();
      if (component == null)
      {
        Debug.LogWarning((object) string.Format("{0} has no {1}", (object) typeof (RemoveEntityIfDurabilityZeroEffect), (object) typeof (ParametersComponent).Name));
        return false;
      }
      IParameter<float> byName = component.GetByName<float>(ParameterNameEnum.Durability);
      if (byName == null)
      {
        Debug.LogWarning((object) string.Format("{0} has no durability parameter", (object) typeof (RemoveEntityIfDurabilityZeroEffect)));
        return false;
      }
      if ((double) byName.Value <= 0.0)
        CoroutineService.Instance.WaitFrame((Action) (() =>
        {
          if ((bool) (UnityEngine.Object) this.removeSound.Value)
            SoundUtility.PlayAudioClip2D(this.removeSound.Value, this.removeSoundMixer.Value, 1f, 0.0f);
          item.Dispose();
        }));
      return false;
    }

    public void Cleanup()
    {
    }
  }
}
