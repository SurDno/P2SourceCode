using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using System;
using UnityEngine;

public class UndiscoveredMindmapCheck : MonoBehaviour
{
  [SerializeField]
  private HideableView view;
  private MMService service;

  private void OnDisable()
  {
    if (this.service == null)
      return;
    this.service.ChangeUndiscoveredEvent -= new Action(this.UpdateView);
    this.service = (MMService) null;
  }

  private void OnEnable()
  {
    if ((UnityEngine.Object) this.view == (UnityEngine.Object) null)
      return;
    this.service = ServiceLocator.GetService<MMService>();
    if (this.service == null)
      return;
    this.UpdateView();
    this.view.SkipAnimation();
    this.service.ChangeUndiscoveredEvent += new Action(this.UpdateView);
  }

  private void UpdateView() => this.view.Visible = this.service.HasUndiscovered();
}
