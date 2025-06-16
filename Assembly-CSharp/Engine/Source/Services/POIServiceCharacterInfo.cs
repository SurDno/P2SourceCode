using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components;
using System;
using UnityEngine;

namespace Engine.Source.Services
{
  public class POIServiceCharacterInfo
  {
    public POIServiceCharacterStateEnum State;
    public bool SearchClosestPOI;
    public bool IsCrowd;
    private IEntity entity;
    private LocationItemComponent location;
    private bool isIndoors;

    public GameObject Character { get; private set; }

    public IEntity Entity
    {
      get
      {
        if (this.entity == null)
          this.InitEntity();
        return this.entity;
      }
      private set
      {
        this.entity = value;
        if (this.entity == null)
          return;
        this.location = this.entity.GetComponent<LocationItemComponent>();
        if (this.location != null)
        {
          this.IsIndoors = this.location.IsIndoor;
          this.location.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.ChangedLocation);
          this.location.OnChangeLocation += new Action<ILocationItemComponent, ILocationComponent>(this.ChangedLocation);
        }
      }
    }

    public bool IsIndoors
    {
      get
      {
        if (this.Entity == null)
          this.InitEntity();
        return this.isIndoors;
      }
      private set => this.isIndoors = value;
    }

    public POIServiceCharacterInfo(GameObject go)
    {
      this.Character = go;
      this.InitEntity();
    }

    public void Clear()
    {
      if (this.location == null)
        return;
      this.location.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.ChangedLocation);
    }

    private void InitEntity()
    {
      EngineGameObject component = this.Character.GetComponent<EngineGameObject>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      this.Entity = component.Owner;
    }

    private void ChangedLocation(ILocationItemComponent locItem, ILocationComponent loc)
    {
      this.IsIndoors = locItem.IsIndoor;
    }
  }
}
