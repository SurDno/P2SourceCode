using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Services.Saves;
using System;
using UnityEngine;

public class ReputationGroupView : MonoBehaviour
{
  [SerializeField]
  private ReputationView reputationViewPrefab;
  [Space]
  [SerializeField]
  private RectTransform currentLayout;
  [SerializeField]
  private RectTransform otherLayout;
  [Space]
  [SerializeField]
  private RegionEnum[] regions;
  [SerializeField]
  private EventView onReputationDown;
  [SerializeField]
  private EventView onReputationUp;
  private ReputationView[] views;
  private NavigationComponent playerNavigation;
  private RegionComponent currentRegion;
  private bool updatedAfterGameLoad;

  private void Awake()
  {
    this.views = new ReputationView[this.regions.Length];
    for (int index = 0; index < this.regions.Length; ++index)
    {
      ReputationView reputationView = UnityEngine.Object.Instantiate<ReputationView>(this.reputationViewPrefab, this.transform);
      reputationView.Initialize(this.regions[index], this.currentLayout, this.otherLayout);
      reputationView.ReputationHighDownEvent += new Action(this.OnReputationDown);
      reputationView.ReputationHighUpEvent += new Action(this.OnReputationUp);
      this.views[index] = reputationView;
    }
    ServiceLocator.GetService<SavesService>().UnloadEvent += new Action(this.OnGameUnloaded);
  }

  private void OnDestroy()
  {
    ServiceLocator.GetService<SavesService>().UnloadEvent -= new Action(this.OnGameUnloaded);
  }

  private void OnDisable()
  {
    ServiceLocator.GetService<Simulation>().OnPlayerChanged -= new Action<IEntity>(this.SetPlayer);
    this.SetPlayer((IEntity) null);
  }

  private void OnEnable()
  {
    Simulation service = ServiceLocator.GetService<Simulation>();
    service.OnPlayerChanged += new Action<IEntity>(this.SetPlayer);
    this.SetPlayer(service.Player);
  }

  private void OnGameUnloaded() => this.updatedAfterGameLoad = false;

  private void SetCurrentRegion(IRegionComponent region)
  {
    RegionEnum currentRegionId = region != null ? region.Region : RegionEnum.None;
    for (int index = 0; index < this.views.Length; ++index)
      this.views[index].SetCurrentRegion(currentRegionId, !this.updatedAfterGameLoad);
    this.updatedAfterGameLoad = true;
  }

  private void SetPlayer(IEntity entity)
  {
    NavigationComponent component = entity?.GetComponent<NavigationComponent>();
    if (this.playerNavigation == component)
      return;
    if (this.playerNavigation != null)
    {
      this.playerNavigation.EnterRegionEvent -= new RegionHandler(this.OnRegionEnter);
      this.playerNavigation.ExitRegionEvent -= new RegionHandler(this.OnRegionExit);
    }
    this.playerNavigation = component;
    if (this.playerNavigation != null)
    {
      this.playerNavigation.EnterRegionEvent += new RegionHandler(this.OnRegionEnter);
      this.playerNavigation.ExitRegionEvent += new RegionHandler(this.OnRegionExit);
      this.SetCurrentRegion(this.playerNavigation.Region);
    }
    else
      this.SetCurrentRegion((IRegionComponent) null);
  }

  private void OnRegionExit(
    ref EventArgument<IEntity, IRegionComponent> eventArguments)
  {
    if (this.currentRegion != eventArguments.Target)
      return;
    this.SetCurrentRegion((IRegionComponent) null);
  }

  private void OnRegionEnter(
    ref EventArgument<IEntity, IRegionComponent> eventArguments)
  {
    if (this.currentRegion == eventArguments.Target)
      return;
    this.SetCurrentRegion(eventArguments.Target);
  }

  private void OnReputationUp() => this.onReputationUp?.Invoke();

  private void OnReputationDown() => this.onReputationDown?.Invoke();
}
