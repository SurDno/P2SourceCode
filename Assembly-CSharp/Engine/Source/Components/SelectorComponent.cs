// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.SelectorComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Selectors;
using Engine.Source.Connections;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Required(typeof (LocationItemComponent))]
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SelectorComponent : EngineComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<SelectorPreset> presets = new List<SelectorPreset>();
    private bool initialise;
    [FromThis]
    private LocationItemComponent locationItemComponent;

    public override void OnAdded()
    {
      base.OnAdded();
      this.locationItemComponent.OnHibernationChanged += new Action<ILocationItemComponent>(this.LocationItemComponent_OnChangeHibernation);
    }

    public override void OnRemoved()
    {
      this.locationItemComponent.OnHibernationChanged -= new Action<ILocationItemComponent>(this.LocationItemComponent_OnChangeHibernation);
      base.OnRemoved();
    }

    private void LocationItemComponent_OnChangeHibernation(ILocationItemComponent sender)
    {
      if (this.initialise || this.locationItemComponent.IsHibernation)
        return;
      this.initialise = true;
      if (this.presets.Count == 0)
        return;
      IEntity sceneEntity = ((IEntityHierarchy) this.Owner).SceneEntity;
      if (sceneEntity == null)
      {
        Debug.LogError((object) ("SceneEntity not found : " + this.Owner.GetInfo()));
      }
      else
      {
        int num = UnityEngine.Random.Range(0, this.presets.Count);
        for (int index = 0; index < this.presets.Count; ++index)
        {
          foreach (SceneGameObject sceneGameObject in this.presets[index].Objects)
          {
            IEntity entityByTemplate = EntityUtility.GetEntityByTemplate(sceneEntity, sceneGameObject.Id);
            if (entityByTemplate != null)
              entityByTemplate.IsEnabled = num == index;
            else
              Debug.LogError((object) ("SelectorComponent - EntityByTemplate not found , id : " + (object) sceneGameObject.Id + " , owner : " + this.Owner.GetInfo()));
          }
        }
      }
    }
  }
}
