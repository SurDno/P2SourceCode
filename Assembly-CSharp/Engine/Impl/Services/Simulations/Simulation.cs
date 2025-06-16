// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.Simulations.Simulation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Comparers;
using Engine.Common.Services;
using Engine.Impl.Services.HierarchyServices;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

#nullable disable
namespace Engine.Impl.Services.Simulations
{
  [Depend(typeof (IFactory))]
  [Depend(typeof (HierarchyService))]
  [RuntimeService(new System.Type[] {typeof (Simulation), typeof (ISimulation)})]
  [SaveDepend(typeof (VirtualMachineController))]
  [SaveDepend(typeof (ISteppeHerbService))]
  public class Simulation : ISimulation, IInitialisable, ISavesController
  {
    private const string nodeName = "Simulation";
    private const string entityName = "Entity";
    private const string idName = "Id";
    private const string pathName = "HierarchyPath";
    [Inspected]
    private Dictionary<Guid, IEntity> entities = new Dictionary<Guid, IEntity>((IEqualityComparer<Guid>) GuidComparer.Instance);
    private IEntity hierarchy;
    private IEntity objects;
    private IEntity storables;
    private IEntity others;
    private bool initialise;
    private IEntity player;
    [Inspected]
    private List<IEntity> players = new List<IEntity>();

    public IEntity Get(Guid id)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      IEntity entity;
      this.entities.TryGetValue(id, out entity);
      return entity;
    }

    [Inspected]
    public IEntity Hierarchy
    {
      get
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return this.hierarchy;
      }
    }

    [Inspected]
    public IEntity Objects
    {
      get
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return this.objects;
      }
    }

    [Inspected]
    public IEntity Storables
    {
      get
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return this.storables;
      }
    }

    [Inspected]
    public IEntity Others
    {
      get
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return this.others;
      }
    }

    [Inspected]
    public IEntity Player
    {
      get => this.player;
      set
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (this.player == value)
          return;
        if (this.player != null)
          this.DeactivatePlayer(this.player);
        this.player = value;
        if (this.player != null)
          this.ActivatePlayer(this.player);
        Action<IEntity> onPlayerChanged = this.OnPlayerChanged;
        if (onPlayerChanged == null)
          return;
        onPlayerChanged(this.player);
      }
    }

    public event Action<IEntity> OnPlayerChanged;

    public void Initialise()
    {
      this.initialise = true;
      this.hierarchy = this.CreateObject("Hierarchy", Ids.HierarchyId);
      ((ComponentCollection) this.Hierarchy).Add<LocationComponent>().IsHibernation = false;
      this.objects = this.CreateObject("Objects", Ids.ObjectsId);
      this.storables = this.CreateObject("Storables", Ids.StorablesId);
      this.others = this.CreateObject("Others", Ids.OthersId);
    }

    public void Terminate()
    {
      this.DisposeRoot(this.Storables);
      this.storables = (IEntity) null;
      this.DisposeRoot(this.Objects);
      this.objects = (IEntity) null;
      this.DisposeRoot(this.Others);
      this.others = (IEntity) null;
      this.DisposeRoot(this.Hierarchy);
      this.hierarchy = (IEntity) null;
      if (this.entities.Count != 0)
      {
        Debug.LogError((object) ("Simulation is not empty, count : " + (object) this.entities.Count));
        this.entities.Clear();
      }
      this.initialise = false;
    }

    private IEntity CreateObject(string name, Guid id)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      IEntity entity = ServiceCache.Factory.Create<IEntity>(id);
      entity.Name = name;
      this.entities.Add(entity.Id, entity);
      ((Entity) entity).OnAdded();
      return entity;
    }

    public void Add(IEntity entity, IEntity parent)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (entity.IsTemplate)
        Debug.LogError((object) ("Add template to simulation : " + entity.GetInfo()));
      ServiceCache.OptimizationService.FrameHasSpike = true;
      this.entities.Add(entity.Id, entity);
      ((IEntityHierarchy) parent).Add(entity);
      ((Entity) entity).OnAdded();
    }

    public void Remove(IEntity entity)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      ((Entity) entity).OnRemoved();
      IEntity parent = entity.Parent;
      if (parent != null)
      {
        ((IEntityHierarchy) parent).Remove(entity);
        this.entities.Remove(entity.Id);
      }
      else if (!Ids.IsRoot(entity.Id))
        throw new Exception(entity.GetInfo());
    }

    private void DisposeRoot(IEntity entity)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      ((Entity) entity).OnRemoved();
      entity.Dispose();
      this.entities.Remove(entity.Id);
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      DateTime time = DateTime.UtcNow;
      TimeSpan minTime = TimeSpan.FromSeconds(1.0);
      XmlElement vmNode = element[nameof (Simulation)];
      if (vmNode == null)
      {
        errorHandler.LogError("Simulation node not found , context : " + context);
      }
      else
      {
        foreach (XmlElement childNode in vmNode.ChildNodes)
        {
          XmlElement item = childNode;
          if (item.Name != "Entity")
          {
            Debug.LogError((object) (item.Name + " is not Entity , context : " + context));
          }
          else
          {
            XmlElement idNode = item["Id"];
            if (idNode == null)
            {
              Debug.LogError((object) ("Id node not found , context : " + context));
            }
            else
            {
              Guid id = DefaultConverter.ParseGuid(idNode.InnerText);
              IEntity entity;
              if (!this.entities.TryGetValue(id, out entity))
              {
                XmlElement pathNode = item["HierarchyPath"];
                Debug.LogError((object) ("Entity " + (object) id + " not found , path : " + (pathNode != null ? (object) pathNode.InnerText : (object) "null") + " , count : " + (object) this.entities.Count + " , context : " + context));
              }
              else
              {
                XmlNodeDataReader reader = new XmlNodeDataReader((XmlNode) item, context);
                ((ISerializeStateLoad) entity).StateLoad((IDataReader) reader, typeof (Entity));
                DateTime currentTime = DateTime.UtcNow;
                if (time + minTime < currentTime)
                {
                  time = currentTime;
                  yield return (object) null;
                }
                idNode = (XmlElement) null;
                id = new Guid();
                entity = (IEntity) null;
                reader = (XmlNodeDataReader) null;
                item = (XmlElement) null;
              }
            }
          }
        }
        foreach (KeyValuePair<Guid, IEntity> entity1 in this.entities)
        {
          KeyValuePair<Guid, IEntity> entity = entity1;
          MetaService.Compute((object) entity.Value, OnLoadedAttribute.Id);
          entity = new KeyValuePair<Guid, IEntity>();
        }
      }
    }

    public void Unload()
    {
      List<KeyValuePair<Guid, IEntity>> list = this.entities.ToList<KeyValuePair<Guid, IEntity>>();
      for (int index = 0; index < list.Count; ++index)
      {
        KeyValuePair<Guid, IEntity> keyValuePair = list[index];
        if (!Ids.IsRoot(keyValuePair.Value.Id))
        {
          Debug.LogError((object) ("Wrong clenup simulation, entity not unloaded : " + keyValuePair.Value.GetInfo()));
          this.Remove(keyValuePair.Value);
          keyValuePair.Value.Dispose();
        }
      }
    }

    public void Save(IDataWriter writer, string context)
    {
      writer.Begin(nameof (Simulation), (System.Type) null, true);
      foreach (KeyValuePair<Guid, IEntity> entity1 in this.entities)
      {
        Entity entity2 = (Entity) entity1.Value;
        if (entity2.NeedSave)
          DefaultStateSaveUtility.SaveSerialize<Entity>(writer, "Entity", entity2);
      }
      writer.End(nameof (Simulation), true);
    }

    public void AddPlayer(IEntity owner)
    {
      ((Entity) owner).IsPlayer = true;
      this.players.Remove(owner);
      this.players.Add(owner);
      this.Player = this.players.FirstOrDefault<IEntity>();
    }

    public void RemovePlayer(IEntity owner)
    {
      this.players.Remove(owner);
      this.Player = this.players.FirstOrDefault<IEntity>();
    }

    private void ActivatePlayer(IEntity player)
    {
      foreach (IComponent component in player.Components)
      {
        if (component is IPlayerActivated playerActivated)
          playerActivated.PlayerActivated();
      }
    }

    private void DeactivatePlayer(IEntity player)
    {
      foreach (IComponent component in player.Components)
      {
        if (component is IPlayerActivated playerActivated)
          playerActivated.PlayerDeactivated();
      }
    }
  }
}
