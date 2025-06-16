using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Components;
using Inspectors;
using System;

namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (StelthEnableListener)})]
  public class StelthEnableListener : IInitialisable
  {
    private bool visible;
    private ControllerComponent controller;

    public event Action<bool> OnVisibleChanged;

    [Inspected(Mutable = true)]
    public bool Visible
    {
      get => this.visible;
      private set
      {
        if (this.visible == value)
          return;
        this.visible = value;
        Action<bool> onVisibleChanged = this.OnVisibleChanged;
        if (onVisibleChanged == null)
          return;
        onVisibleChanged(value);
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
      if (this.controller != null)
      {
        this.controller.IsStelth.ChangeValueEvent -= new Action<bool>(this.OnStelthEnableChanged);
        this.controller = (ControllerComponent) null;
      }
      if (player != null)
        this.controller = player.GetComponent<ControllerComponent>();
      if (this.controller != null)
        this.controller.IsStelth.ChangeValueEvent += new Action<bool>(this.OnStelthEnableChanged);
      this.UpdateValue();
    }

    private void OnStelthEnableChanged(bool value) => this.UpdateValue();

    private void UpdateValue()
    {
      this.Visible = this.controller != null && this.controller.IsStelth.Value;
    }
  }
}
