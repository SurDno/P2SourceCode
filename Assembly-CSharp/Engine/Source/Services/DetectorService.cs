using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.Detectablies;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services
{
  [GameService(typeof (DetectorService))]
  public class DetectorService
  {
    [Inspected]
    private Dictionary<DetectableComponent, DetectableCandidatInfo> detectablies = new Dictionary<DetectableComponent, DetectableCandidatInfo>();
    private List<DetectableCandidatInfo> cache = new List<DetectableCandidatInfo>();
    private bool invalidate = true;

    public List<DetectableCandidatInfo> Detectablies
    {
      get
      {
        if (invalidate)
        {
          invalidate = false;
          cache.Clear();
          foreach (KeyValuePair<DetectableComponent, DetectableCandidatInfo> detectably in detectablies)
            cache.Add(detectably.Value);
        }
        return cache;
      }
    }

    public void AddDetectable(DetectableComponent detectable)
    {
      invalidate = true;
      ILocationItemComponent component = detectable.GetComponent<ILocationItemComponent>();
      if (component == null)
      {
        Debug.LogError("ILocationItemComponent not found in " + detectable.Owner.GetInfo());
      }
      else
      {
        GameObject gameObject = ((IEntityView) detectable.Owner).GameObject;
        gameObject.GetComponent<Collider>();
        Vector3 up = Vector3.up;
        detectablies[detectable] = new DetectableCandidatInfo {
          Detectable = detectable,
          LocationItem = component,
          GameObject = gameObject,
          Offset = up
        };
      }
    }

    public void RemoveDetectable(DetectableComponent detectable)
    {
      invalidate = true;
      detectablies.Remove(detectable);
    }
  }
}
