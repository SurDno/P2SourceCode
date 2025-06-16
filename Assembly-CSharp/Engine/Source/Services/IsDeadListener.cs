// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.IsDeadListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Services
{
  [Depend(typeof (ISimulation))]
  [GameService(new Type[] {typeof (IsDeadListener)})]
  public class IsDeadListener : IInitialisable
  {
    private bool isDead;
    private IParameter<bool> parameter;

    public event Action<bool> OnIsDeadChanged;

    [Inspected(Mutable = true)]
    public bool IsDead
    {
      get => this.isDead;
      private set
      {
        if (this.isDead == value)
          return;
        this.isDead = value;
        Action<bool> onIsDeadChanged = this.OnIsDeadChanged;
        if (onIsDeadChanged == null)
          return;
        onIsDeadChanged(value);
      }
    }

    public void Initialise()
    {
      ServiceLocator.GetService<Simulation>().OnPlayerChanged += new Action<IEntity>(this.OnPlayerChanged);
      this.OnPlayerChanged(ServiceLocator.GetService<ISimulation>().Player);
    }

    public void Terminate()
    {
      ServiceLocator.GetService<Simulation>().OnPlayerChanged -= new Action<IEntity>(this.OnPlayerChanged);
    }

    private void OnPlayerChanged(IEntity player)
    {
      this.parameter = (IParameter<bool>) null;
      if (player != null)
      {
        ParametersComponent component = player.GetComponent<ParametersComponent>();
        if (component != null)
          this.parameter = component.GetByName<bool>(ParameterNameEnum.Dead);
      }
      this.UpdateValue();
    }

    private void OnDeadStateChanged(bool value) => this.UpdateValue();

    private void UpdateValue() => this.IsDead = this.parameter != null && this.parameter.Value;
  }
}
