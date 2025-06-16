// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.DetectorService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.Detectablies;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (DetectorService)})]
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
        if (this.invalidate)
        {
          this.invalidate = false;
          this.cache.Clear();
          foreach (KeyValuePair<DetectableComponent, DetectableCandidatInfo> detectably in this.detectablies)
            this.cache.Add(detectably.Value);
        }
        return this.cache;
      }
    }

    public void AddDetectable(DetectableComponent detectable)
    {
      this.invalidate = true;
      ILocationItemComponent component = detectable.GetComponent<ILocationItemComponent>();
      if (component == null)
      {
        Debug.LogError((object) ("ILocationItemComponent not found in " + detectable.Owner.GetInfo()));
      }
      else
      {
        GameObject gameObject = ((IEntityView) detectable.Owner).GameObject;
        gameObject.GetComponent<Collider>();
        Vector3 up = Vector3.up;
        this.detectablies[detectable] = new DetectableCandidatInfo()
        {
          Detectable = detectable,
          LocationItem = component,
          GameObject = gameObject,
          Offset = up
        };
      }
    }

    public void RemoveDetectable(DetectableComponent detectable)
    {
      this.invalidate = true;
      this.detectablies.Remove(detectable);
    }
  }
}
