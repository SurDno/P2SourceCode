using System;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Services
{
  [GameService(typeof (StelthEnableListener))]
  public class StelthEnableListener : IInitialisable
  {
    private bool visible;
    private ControllerComponent controller;

    public event Action<bool> OnVisibleChanged;

    [Inspected(Mutable = true)]
    public bool Visible
    {
      get => visible;
      private set
      {
        if (visible == value)
          return;
        visible = value;
        Action<bool> onVisibleChanged = OnVisibleChanged;
        if (onVisibleChanged == null)
          return;
        onVisibleChanged(value);
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
      if (controller != null)
      {
        controller.IsStelth.ChangeValueEvent -= OnStelthEnableChanged;
        controller = null;
      }
      if (player != null)
        controller = player.GetComponent<ControllerComponent>();
      if (controller != null)
        controller.IsStelth.ChangeValueEvent += OnStelthEnableChanged;
      UpdateValue();
    }

    private void OnStelthEnableChanged(bool value) => UpdateValue();

    private void UpdateValue()
    {
      Visible = controller != null && controller.IsStelth.Value;
    }
  }
}
