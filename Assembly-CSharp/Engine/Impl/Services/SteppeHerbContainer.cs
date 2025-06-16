// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.SteppeHerbContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Impl.Services
{
  public class SteppeHerbContainer : IDisposable
  {
    private const float spawnRadius = 30f;
    private const float pointsPartToProcessPerUpdate = 0.1f;
    private IEntity template;
    private List<SteppeHerbEntry> SteppeHerbEntries = new List<SteppeHerbEntry>();
    private List<int> SteppeHerbIndices = new List<int>();
    private List<int> SteppeHerbIndicesLeft = new List<int>();
    private List<int> NearPlayerIndices = new List<int>();
    private int processingIndex;
    private int amount;

    public SteppeHerbContainer(IEntity template, GameObject prefab)
    {
      this.template = template;
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      for (int index = 0; index < gameObject.transform.childCount; ++index)
      {
        Vector3 position = gameObject.transform.GetChild(index).position;
        this.SteppeHerbIndicesLeft.Add(this.SteppeHerbEntries.Count);
        this.SteppeHerbEntries.Add(new SteppeHerbEntry()
        {
          Position = position,
          Entity = (IEntity) null,
          On = false
        });
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
    }

    [Inspected]
    public int Amount
    {
      get => this.amount;
      set
      {
        this.amount = Mathf.Clamp(value, 0, this.SteppeHerbEntries.Count);
        this.Syncronize();
      }
    }

    [Inspected]
    public void Reset()
    {
      foreach (SteppeHerbEntry steppeHerbEntry in this.SteppeHerbEntries)
        steppeHerbEntry.Collected = false;
    }

    public void Dispose()
    {
      foreach (int steppeHerbIndex in this.SteppeHerbIndices)
      {
        SteppeHerbEntry steppeHerbEntry = this.SteppeHerbEntries[steppeHerbIndex];
        if (steppeHerbEntry.Entity != null)
        {
          steppeHerbEntry.Entity.Dispose();
          steppeHerbEntry.Entity = (IEntity) null;
          steppeHerbEntry.On = false;
        }
      }
    }

    private void Syncronize()
    {
      if (this.amount == this.SteppeHerbIndices.Count)
        return;
      if (this.amount < this.SteppeHerbIndices.Count)
      {
        int num = this.SteppeHerbIndices.Count - this.amount;
        for (int index1 = 0; index1 < num; ++index1)
        {
          int index2 = UnityEngine.Random.Range(0, this.SteppeHerbIndices.Count);
          int steppeHerbIndex = this.SteppeHerbIndices[index2];
          this.SteppeHerbEntries[steppeHerbIndex].On = false;
          this.SteppeHerbIndices.RemoveAt(index2);
          this.SteppeHerbIndicesLeft.Add(steppeHerbIndex);
        }
      }
      else
      {
        int num = this.amount - this.SteppeHerbIndices.Count;
        for (int index3 = 0; index3 < num; ++index3)
        {
          int index4 = UnityEngine.Random.Range(0, this.SteppeHerbIndicesLeft.Count);
          int index5 = this.SteppeHerbIndicesLeft[index4];
          this.SteppeHerbEntries[index5].On = true;
          this.SteppeHerbIndicesLeft.RemoveAt(index4);
          this.SteppeHerbIndices.Add(index5);
        }
      }
    }

    private IEntity SpawnHerb(SteppeHerbEntry entry, LocationComponent locationComponent)
    {
      IEntity entity = ServiceCache.Factory.Instantiate<IEntity>(this.template);
      ((Entity) entity).DontSave = true;
      Simulation service = ServiceLocator.GetService<Simulation>();
      service.Add(entity, service.Objects);
      NavigationComponent component = entity.GetComponent<NavigationComponent>();
      if (component != null)
      {
        Quaternion rotation = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 6.28318548f), 0.0f);
        component.TeleportTo((ILocationComponent) locationComponent, entry.Position, rotation);
      }
      else
        Debug.LogError((object) ("NavigationComponent not found : " + entity.GetInfo()));
      return entity;
    }

    private bool IsCollected(IEntity entity)
    {
      ParametersComponent component = entity.GetComponent<ParametersComponent>();
      if (component == null)
        return true;
      IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.Collected);
      return byName == null || byName.Value;
    }

    public void Update(Vector3 playerPosition)
    {
      if (this.SteppeHerbEntries.Count == 0)
        return;
      RegionComponent regionByName = RegionUtility.GetRegionByName(RegionEnum.Steppe);
      if (regionByName == null)
      {
        Debug.LogError((object) "region == null");
      }
      else
      {
        LocationComponent component = regionByName.GetComponent<LocationComponent>();
        int index1 = 0;
        Vector3 vector3;
        while (index1 < this.NearPlayerIndices.Count)
        {
          vector3 = this.SteppeHerbEntries[this.NearPlayerIndices[index1]].Position - playerPosition;
          if ((double) vector3.magnitude > 30.0)
          {
            SteppeHerbEntry steppeHerbEntry = this.SteppeHerbEntries[this.NearPlayerIndices[index1]];
            if (steppeHerbEntry.Entity != null)
            {
              steppeHerbEntry.Collected = this.IsCollected(steppeHerbEntry.Entity);
              steppeHerbEntry.Entity.Dispose();
              steppeHerbEntry.Entity = (IEntity) null;
            }
            this.NearPlayerIndices.RemoveAt(index1);
          }
          else
            ++index1;
        }
        int num = (int) ((double) this.SteppeHerbEntries.Count * 0.10000000149011612) + 1;
        for (int index2 = 0; index2 < num; ++index2)
        {
          this.processingIndex = (this.processingIndex + 1) % this.SteppeHerbEntries.Count;
          vector3 = this.SteppeHerbEntries[this.processingIndex].Position - playerPosition;
          if ((double) vector3.magnitude < 30.0 && !this.NearPlayerIndices.Contains(this.processingIndex))
          {
            SteppeHerbEntry steppeHerbEntry = this.SteppeHerbEntries[this.processingIndex];
            if (steppeHerbEntry.On && !steppeHerbEntry.Collected)
            {
              if (steppeHerbEntry.Entity != null)
                Debug.LogError((object) "Entity can't be null here");
              else
                steppeHerbEntry.Entity = this.SpawnHerb(steppeHerbEntry, component);
            }
            this.NearPlayerIndices.Add(this.processingIndex);
          }
        }
      }
    }
  }
}
