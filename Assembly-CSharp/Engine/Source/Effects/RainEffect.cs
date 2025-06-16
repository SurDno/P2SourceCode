// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.RainEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using Rain;
using UnityEngine;

#nullable disable
namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RainEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    private IParameter<float> rainParameter;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      this.rainParameter = this.Target.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Rain);
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      RainManager instance = RainManager.Instance;
      if ((Object) instance == (Object) null || this.rainParameter == null)
        return false;
      if ((double) this.rainParameter.Value != (double) instance.rainIntensity)
      {
        this.rainParameter.BaseValue = instance.rainIntensity;
        this.rainParameter.Value = instance.rainIntensity;
      }
      return true;
    }

    public void Cleanup() => this.rainParameter = (IParameter<float>) null;
  }
}
