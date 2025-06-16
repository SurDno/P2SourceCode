using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components;
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
        if (entity == null)
          InitEntity();
        return entity;
      }
      private set
      {
        entity = value;
        if (entity == null)
          return;
        location = entity.GetComponent<LocationItemComponent>();
        if (location != null)
        {
          IsIndoors = location.IsIndoor;
          location.OnChangeLocation -= ChangedLocation;
          location.OnChangeLocation += ChangedLocation;
        }
      }
    }

    public bool IsIndoors
    {
      get
      {
        if (Entity == null)
          InitEntity();
        return isIndoors;
      }
      private set => isIndoors = value;
    }

    public POIServiceCharacterInfo(GameObject go)
    {
      Character = go;
      InitEntity();
    }

    public void Clear()
    {
      if (location == null)
        return;
      location.OnChangeLocation -= ChangedLocation;
    }

    private void InitEntity()
    {
      EngineGameObject component = Character.GetComponent<EngineGameObject>();
      if (!(component != null))
        return;
      Entity = component.Owner;
    }

    private void ChangedLocation(ILocationItemComponent locItem, ILocationComponent loc)
    {
      IsIndoors = locItem.IsIndoor;
    }
  }
}
