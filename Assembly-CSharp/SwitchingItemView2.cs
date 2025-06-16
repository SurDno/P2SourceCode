// Decompiled with JetBrains decompiler
// Type: SwitchingItemView2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using System;
using UnityEngine;

#nullable disable
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
  private StorableComponent storable = (StorableComponent) null;
  private bool nextDirection = false;
  private bool direction = false;
  private bool needToSwitch = false;
  private float phase = 1f;
  private bool isAnimated = false;

  public event Action AnimationBeginEvent;

  public event Action AnimationEndEvent;

  public bool IsAnimated
  {
    get => this.isAnimated;
    private set
    {
      if (value == this.isAnimated)
        return;
      this.isAnimated = value;
      if (this.isAnimated)
      {
        Action animationBeginEvent = this.AnimationBeginEvent;
        if (animationBeginEvent == null)
          return;
        animationBeginEvent();
      }
      else
      {
        Action animationEndEvent = this.AnimationEndEvent;
        if (animationEndEvent == null)
          return;
        animationEndEvent();
      }
    }
  }

  public bool ReversedDirection
  {
    get => this.nextDirection;
    set => this.nextDirection = value;
  }

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      if (this.isActiveAndEnabled)
        this.needToSwitch = true;
      else
        this.ForceResult();
    }
  }

  private void ForceResult()
  {
    this.SetFrontViewStorable();
    this.phase = 1f;
    this.ApplyPhase();
  }

  private void ApplyPhase()
  {
    if ((double) this.phase >= 1.0)
    {
      this.phase = 1f;
      this.backView.Storable = (StorableComponent) null;
      this.IsAnimated = false;
    }
    else
      this.IsAnimated = true;
    float num = 1f - this.phase;
    this.frontOpacity.Progress = this.frontView.Storable != null ? this.phase : 0.0f;
    this.backOpacity.Progress = this.backView.Storable != null ? num : 0.0f;
    if (!((UnityEngine.Object) this.nullOpacity != (UnityEngine.Object) null))
      return;
    this.nullOpacity.Progress = (float) ((this.frontView.Storable == null ? (double) this.phase : 0.0) + (this.backView.Storable == null ? (double) num : 0.0));
  }

  private void OnDisable() => this.ForceResult();

  private void SetFrontViewStorable()
  {
    this.frontView.Storable = this.storable;
    this.needToSwitch = false;
  }

  private void SwitchViews()
  {
    this.ApplyDirection();
    ItemView backView = this.backView;
    this.backView = this.frontView;
    this.frontView = backView;
    Transform backTransform = this.backTransform;
    this.backTransform = this.frontTransform;
    this.frontTransform = backTransform;
    this.backTransform.SetParent(this.backAnchor, false);
    this.frontTransform.SetParent(this.frontAnchor, false);
  }

  private void ApplyDirection()
  {
    if (this.nextDirection == this.direction)
      return;
    this.direction = this.nextDirection;
    Transform backAnchor = this.backAnchor;
    this.backAnchor = this.frontAnchor;
    this.frontAnchor = backAnchor;
    ProgressViewBase backOpacity = this.backOpacity;
    this.backOpacity = this.frontOpacity;
    this.frontOpacity = backOpacity;
  }

  private void Update()
  {
    if ((double) this.phase == 1.0)
    {
      if (!this.needToSwitch)
        return;
      this.SwitchViews();
      this.SetFrontViewStorable();
      this.phase = Time.deltaTime / this.swapTime;
      this.ApplyPhase();
    }
    else
    {
      this.phase += Time.deltaTime / this.swapTime;
      this.ApplyPhase();
    }
  }
}
