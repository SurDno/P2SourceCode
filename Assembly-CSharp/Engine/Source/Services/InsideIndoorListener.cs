using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using System;

namespace Engine.Source.Services
{
  [Depend(typeof (ISimulation))]
  [RuntimeService(new Type[] {typeof (InsideIndoorListener)})]
  public class InsideIndoorListener : IInitialisable
  {
    private bool insideIndoor;
    private bool isolatedIndoor;
    private LocationItemComponent item;
    private ControllerComponent controllerComponent;
    private NavigationComponent navigationComponent;

    public event Action<bool> OnInsideIndoorChanged;

    public event Action<bool> OnIsolatedIndoorChanged;

    public event Action OnPlayerBeginsExit;

    [Inspected(Mutable = true)]
    public bool InsideIndoor
    {
      get => this.insideIndoor;
      private set
      {
        if (this.insideIndoor == value)
          return;
        this.insideIndoor = value;
        Action<bool> insideIndoorChanged = this.OnInsideIndoorChanged;
        if (insideIndoorChanged == null)
          return;
        insideIndoorChanged(value);
      }
    }

    [Inspected(Mutable = true)]
    public bool IsolatedIndoor
    {
      get => this.isolatedIndoor;
      private set
      {
        if (this.isolatedIndoor == value)
          return;
        this.isolatedIndoor = value;
        Action<bool> isolatedIndoorChanged = this.OnIsolatedIndoorChanged;
        if (isolatedIndoorChanged == null)
          return;
        isolatedIndoorChanged(value);
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

    private void OnBeginInteract(
      IEntity player,
      IInteractableComponent interactable,
      IInteractItem item)
    {
      if (item.Type != InteractType.Outdoor)
        return;
      Action playerBeginsExit = this.OnPlayerBeginsExit;
      if (playerBeginsExit == null)
        return;
      playerBeginsExit();
    }

    private void OnPlayerChanged(IEntity player)
    {
      if (this.item != null)
      {
        this.item.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.OnChangeLocation);
        this.item = (LocationItemComponent) null;
      }
      if (this.controllerComponent != null)
      {
        this.controllerComponent.BeginInteractEvent -= new Action<IEntity, IInteractableComponent, IInteractItem>(this.OnBeginInteract);
        this.controllerComponent = (ControllerComponent) null;
      }
      this.navigationComponent = (NavigationComponent) null;
      if (player != null)
      {
        this.item = player.GetComponent<LocationItemComponent>();
        this.controllerComponent = player.GetComponent<ControllerComponent>();
        this.navigationComponent = player.GetComponent<NavigationComponent>();
      }
      if (this.item != null)
        this.item.OnChangeLocation += new Action<ILocationItemComponent, ILocationComponent>(this.OnChangeLocation);
      if (this.controllerComponent != null)
        this.controllerComponent.BeginInteractEvent += new Action<IEntity, IInteractableComponent, IInteractItem>(this.OnBeginInteract);
      this.UpdateValue();
    }

    private void OnChangeLocation(ILocationItemComponent sender, ILocationComponent value)
    {
      this.UpdateValue();
    }

    private void UpdateValue()
    {
      this.InsideIndoor = this.item != null && this.item.IsIndoor;
      if (this.InsideIndoor)
      {
        IBuildingComponent building = this.navigationComponent?.Building;
        this.IsolatedIndoor = building != null && ScriptableObjectInstance<IndoorSettingsData>.Instance.IsIndoorIsolated(building.Building);
      }
      else
        this.IsolatedIndoor = false;
    }
  }
}
