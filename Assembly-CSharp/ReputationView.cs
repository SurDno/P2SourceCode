using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using System;
using UnityEngine;

public class ReputationView : MonoBehaviour
{
  [SerializeField]
  private RectTransform currentRect;
  [SerializeField]
  private RectTransform otherRect;
  [SerializeField]
  private StringView nameView;
  [SerializeField]
  private HideableView currentView;
  [SerializeField]
  private ProgressView valueView;
  [Space]
  [SerializeField]
  private EventView upLowView;
  [SerializeField]
  private EventView upMediumView;
  [SerializeField]
  private EventView upHighView;
  [SerializeField]
  private EventView downLowView;
  [SerializeField]
  private EventView downMediumView;
  [SerializeField]
  private EventView downHighView;
  [Space]
  [SerializeField]
  private float mediumThreshold;
  [SerializeField]
  private float highThreshold;
  private RegionEnum regionId;
  private RegionComponent region;
  private bool updatedAfterGameLoad;

  public event Action ReputationHighDownEvent;

  public event Action ReputationHighUpEvent;

  private void Awake()
  {
    ServiceLocator.GetService<SavesService>().UnloadEvent += new Action(this.OnGameUnloaded);
  }

  public void Initialize(
    RegionEnum regionId,
    RectTransform currentLayout,
    RectTransform otherLayout)
  {
    this.regionId = regionId;
    this.currentRect.SetParent((Transform) currentLayout, false);
    this.otherRect.SetParent((Transform) otherLayout, false);
    this.UpdateRegion();
  }

  private void OnDestroy()
  {
    ServiceLocator.GetService<SavesService>().UnloadEvent -= new Action(this.OnGameUnloaded);
  }

  private void OnDisable() => this.SetRegion((RegionComponent) null);

  private void OnEnable() => this.UpdateRegion();

  private void OnGameUnloaded() => this.updatedAfterGameLoad = false;

  private void SetValue(float value)
  {
    bool flag = !this.updatedAfterGameLoad || ServiceLocator.GetService<NotificationService>().TypeOrLayerBlocked(NotificationEnum.Reputation);
    if (!flag)
    {
      float num = value - this.valueView.Progress;
      if ((double) num <= -(double) this.highThreshold)
      {
        this.downHighView.Invoke();
        Action reputationHighDownEvent = this.ReputationHighDownEvent;
        if (reputationHighDownEvent != null)
          reputationHighDownEvent();
      }
      else if ((double) num <= -(double) this.mediumThreshold)
        this.downMediumView.Invoke();
      else if ((double) num < 0.0)
        this.downLowView.Invoke();
      else if ((double) num >= (double) this.highThreshold)
      {
        this.upHighView.Invoke();
        Action reputationHighUpEvent = this.ReputationHighUpEvent;
        if (reputationHighUpEvent != null)
          reputationHighUpEvent();
      }
      else if ((double) num >= (double) this.mediumThreshold)
        this.upMediumView.Invoke();
      else if ((double) num > 0.0)
        this.upLowView.Invoke();
    }
    this.valueView.Progress = value;
    if (flag)
      this.valueView.SkipAnimation();
    this.updatedAfterGameLoad = true;
  }

  public void SetCurrentRegion(RegionEnum currentRegionId, bool instant)
  {
    this.currentView.Visible = this.regionId == currentRegionId;
    if (!instant)
      return;
    this.currentView.SkipAnimation();
  }

  private void SetRegion(RegionComponent newRegion)
  {
    if (this.region != null)
    {
      if (this.region.Reputation != null)
        this.region.Reputation.ChangeValueEvent -= new Action<float>(this.SetValue);
      else
        Debug.LogError((object) "region.Reputation == null , разобраться");
    }
    this.region = newRegion;
    if (this.region == null)
      return;
    this.nameView.StringValue = RegionUtility.GetRegionTitle((IRegionComponent) this.region);
    this.SetValue(this.region.Reputation.Value);
    this.region.Reputation.ChangeValueEvent += new Action<float>(this.SetValue);
  }

  private void UpdateRegion() => this.SetRegion(RegionUtility.GetRegionByName(this.regionId));
}
