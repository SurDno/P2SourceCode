// Decompiled with JetBrains decompiler
// Type: Engine.Source.Controllers.SceneLoadStateController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Commons;
using Inspectors;
using StateSetters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Controllers
{
  public class SceneLoadStateController : MonoBehaviour, IEntityAttachable
  {
    [SerializeField]
    private StateSetterItem[] sceneLoadState;
    private ILocationComponent location;

    public void Attach(IEntity owner)
    {
      this.location = owner.GetComponent<ILocationComponent>();
      if (this.location == null)
        return;
      this.location.OnHibernationChanged += new Action<ILocationComponent>(this.LocationOnChangeHibernation);
      this.LocationOnChangeHibernation(this.location);
    }

    public void Detach()
    {
      if (this.location == null)
        return;
      this.location.OnHibernationChanged -= new Action<ILocationComponent>(this.LocationOnChangeHibernation);
    }

    private void LocationOnChangeHibernation(ILocationComponent sender)
    {
      if (this.location.IsHibernation)
        this.SceneLoadOff();
      else
        this.SceneLoadOn();
    }

    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    private void SceneLoadOn() => this.sceneLoadState.Apply(true);

    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    private void SceneLoadOff() => this.sceneLoadState.Apply(false);
  }
}
