using System;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Services
{
  [Depend(typeof (ISimulation))]
  [GameService(typeof (IsDeadListener))]
  public class IsDeadListener : IInitialisable
  {
    private bool isDead;
    private IParameter<bool> parameter;

    public event Action<bool> OnIsDeadChanged;

    [Inspected(Mutable = true)]
    public bool IsDead
    {
      get => isDead;
      private set
      {
        if (isDead == value)
          return;
        isDead = value;
        Action<bool> onIsDeadChanged = OnIsDeadChanged;
        if (onIsDeadChanged == null)
          return;
        onIsDeadChanged(value);
      }
    }

    public void Initialise()
    {
      ServiceLocator.GetService<Simulation>().OnPlayerChanged += OnPlayerChanged;
      OnPlayerChanged(ServiceLocator.GetService<ISimulation>().Player);
    }

    public void Terminate()
    {
      ServiceLocator.GetService<Simulation>().OnPlayerChanged -= OnPlayerChanged;
    }

    private void OnPlayerChanged(IEntity player)
    {
      parameter = null;
      if (player != null)
      {
        ParametersComponent component = player.GetComponent<ParametersComponent>();
        if (component != null)
          parameter = component.GetByName<bool>(ParameterNameEnum.Dead);
      }
      UpdateValue();
    }

    private void OnDeadStateChanged(bool value) => UpdateValue();

    private void UpdateValue() => IsDead = parameter != null && parameter.Value;
  }
}
