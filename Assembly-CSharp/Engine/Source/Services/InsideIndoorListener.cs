using System;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Services
{
  [Depend(typeof (ISimulation))]
  [RuntimeService(typeof (InsideIndoorListener))]
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
      get => insideIndoor;
      private set
      {
        if (insideIndoor == value)
          return;
        insideIndoor = value;
        Action<bool> insideIndoorChanged = OnInsideIndoorChanged;
        if (insideIndoorChanged == null)
          return;
        insideIndoorChanged(value);
      }
    }

    [Inspected(Mutable = true)]
    public bool IsolatedIndoor
    {
      get => isolatedIndoor;
      private set
      {
        if (isolatedIndoor == value)
          return;
        isolatedIndoor = value;
        Action<bool> isolatedIndoorChanged = OnIsolatedIndoorChanged;
        if (isolatedIndoorChanged == null)
          return;
        isolatedIndoorChanged(value);
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

    private void OnBeginInteract(
      IEntity player,
      IInteractableComponent interactable,
      IInteractItem item)
    {
      if (item.Type != InteractType.Outdoor)
        return;
      Action playerBeginsExit = OnPlayerBeginsExit;
      if (playerBeginsExit == null)
        return;
      playerBeginsExit();
    }

    private void OnPlayerChanged(IEntity player)
    {
      if (item != null)
      {
        item.OnChangeLocation -= OnChangeLocation;
        item = null;
      }
      if (controllerComponent != null)
      {
        controllerComponent.BeginInteractEvent -= OnBeginInteract;
        controllerComponent = null;
      }
      navigationComponent = null;
      if (player != null)
      {
        item = player.GetComponent<LocationItemComponent>();
        controllerComponent = player.GetComponent<ControllerComponent>();
        navigationComponent = player.GetComponent<NavigationComponent>();
      }
      if (item != null)
        item.OnChangeLocation += OnChangeLocation;
      if (controllerComponent != null)
        controllerComponent.BeginInteractEvent += OnBeginInteract;
      UpdateValue();
    }

    private void OnChangeLocation(ILocationItemComponent sender, ILocationComponent value)
    {
      UpdateValue();
    }

    private void UpdateValue()
    {
      InsideIndoor = item != null && item.IsIndoor;
      if (InsideIndoor)
      {
        IBuildingComponent building = navigationComponent?.Building;
        IsolatedIndoor = building != null && ScriptableObjectInstance<IndoorSettingsData>.Instance.IsIndoorIsolated(building.Building);
      }
      else
        IsolatedIndoor = false;
    }
  }
}
