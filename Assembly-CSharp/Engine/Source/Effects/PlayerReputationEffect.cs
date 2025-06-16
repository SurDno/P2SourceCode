// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.PlayerReputationEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;

#nullable disable
namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlayerReputationEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    private IParameter<float> parameter;
    private NavigationComponent navigation;
    private PlayerControllerComponent controller;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      this.parameter = this.Target.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Reputation);
      this.navigation = this.Target.GetComponent<NavigationComponent>();
      this.controller = this.Target.GetComponent<PlayerControllerComponent>();
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if (this.navigation == null || this.controller == null || this.parameter == null)
        return false;
      IRegionComponent region = this.navigation.Region;
      if (region == null)
      {
        this.parameter.Value = 0.0f;
        return true;
      }
      this.parameter.Value = region.Reputation.Value;
      return true;
    }

    public void Cleanup()
    {
      this.navigation = (NavigationComponent) null;
      this.controller = (PlayerControllerComponent) null;
      this.parameter = (IParameter<float>) null;
    }
  }
}
