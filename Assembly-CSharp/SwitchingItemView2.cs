using System;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

public class SwitchingItemView2 : ItemView
{
  [SerializeField]
  private Transform backAnchor;
  [SerializeField]
  private Transform frontAnchor;
  [SerializeField]
  private ProgressViewBase backOpacity;
  [SerializeField]
  private ProgressViewBase frontOpacity;
  [SerializeField]
  private ProgressViewBase nullOpacity;
  [SerializeField]
  private Transform backTransform;
  [SerializeField]
  private Transform frontTransform;
  [SerializeField]
  private ItemView backView;
  [SerializeField]
  private ItemView frontView;
  [SerializeField]
  private float swapTime = 0.5f;
  private StorableComponent storable;
  private bool nextDirection;
  private bool direction;
  private bool needToSwitch;
  private float phase = 1f;
  private bool isAnimated;

  public event Action AnimationBeginEvent;

  public event Action AnimationEndEvent;

  public bool IsAnimated
  {
    get => isAnimated;
    private set
    {
      if (value == isAnimated)
        return;
      isAnimated = value;
      if (isAnimated)
      {
        Action animationBeginEvent = AnimationBeginEvent;
        if (animationBeginEvent == null)
          return;
        animationBeginEvent();
      }
      else
      {
        Action animationEndEvent = AnimationEndEvent;
        if (animationEndEvent == null)
          return;
        animationEndEvent();
      }
    }
  }

  public bool ReversedDirection
  {
    get => nextDirection;
    set => nextDirection = value;
  }

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      if (isActiveAndEnabled)
        needToSwitch = true;
      else
        ForceResult();
    }
  }

  private void ForceResult()
  {
    SetFrontViewStorable();
    phase = 1f;
    ApplyPhase();
  }

  private void ApplyPhase()
  {
    if (phase >= 1.0)
    {
      phase = 1f;
      backView.Storable = null;
      IsAnimated = false;
    }
    else
      IsAnimated = true;
    float num = 1f - phase;
    frontOpacity.Progress = frontView.Storable != null ? phase : 0.0f;
    backOpacity.Progress = backView.Storable != null ? num : 0.0f;
    if (!(nullOpacity != null))
      return;
    nullOpacity.Progress = (float) ((frontView.Storable == null ? phase : 0.0) + (backView.Storable == null ? num : 0.0));
  }

  private void OnDisable() => ForceResult();

  private void SetFrontViewStorable()
  {
    frontView.Storable = storable;
    needToSwitch = false;
  }

  private void SwitchViews()
  {
    ApplyDirection();
    ItemView backView = this.backView;
    this.backView = frontView;
    frontView = backView;
    Transform backTransform = this.backTransform;
    this.backTransform = frontTransform;
    frontTransform = backTransform;
    this.backTransform.SetParent(backAnchor, false);
    frontTransform.SetParent(frontAnchor, false);
  }

  private void ApplyDirection()
  {
    if (nextDirection == direction)
      return;
    direction = nextDirection;
    Transform backAnchor = this.backAnchor;
    this.backAnchor = frontAnchor;
    frontAnchor = backAnchor;
    ProgressViewBase backOpacity = this.backOpacity;
    this.backOpacity = frontOpacity;
    frontOpacity = backOpacity;
  }

  private void Update()
  {
    if (phase == 1.0)
    {
      if (!needToSwitch)
        return;
      SwitchViews();
      SetFrontViewStorable();
      phase = Time.deltaTime / swapTime;
      ApplyPhase();
    }
    else
    {
      phase += Time.deltaTime / swapTime;
      ApplyPhase();
    }
  }
}
