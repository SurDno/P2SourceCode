using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
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

namespace Engine.Impl.Services.Simulations
{
  [Depend(typeof (IFactory))]
  [Depend(typeof (HierarchyService))]
  [RuntimeService(typeof (Simulation), typeof (ISimulation))]
  [SaveDepend(typeof (VirtualMachineController))]
  [SaveDepend(typeof (ISteppeHerbService))]
  public class Simulation : ISimulation, IInitialisable, ISavesController
  {
    private const string nodeName = "Simulation";
    private const string entityName = "Entity";
    private const string idName = "Id";
    private const string pathName = "HierarchyPath";
    [Inspected]
    private Dictionary<Guid, IEntity> entities = new Dictionary<Guid, IEntity>(GuidComparer.Instance);
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
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      IEntity entity;
      entities.TryGetValue(id, out entity);
      return entity;
    }

    [Inspected]
    public IEntity Hierarchy
    {
      get
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return hierarchy;
      }
    }

    [Inspected]
    public IEntity Objects
    {
      get
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return objects;
      }
    }

    [Inspected]
    public IEntity Storables
    {
      get
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return storables;
      }
    }

    [Inspected]
    public IEntity Others
    {
      get
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return others;
      }
    }

    [Inspected]
    public IEntity Player
    {
      get => player;
      set
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (player == value)
          return;
        if (player != null)
          DeactivatePlayer(player);
        player = value;
        if (player != null)
          ActivatePlayer(player);
        Action<IEntity> onPlayerChanged = OnPlayerChanged;
        if (onPlayerChanged == null)
          return;
        onPlayerChanged(player);
      }
    }

    public event Action<IEntity> OnPlayerChanged;

    public void Initialise()
    {
      initialise = true;
      hierarchy = CreateObject("Hierarchy", Ids.HierarchyId);
      ((ComponentCollection) Hierarchy).Add<LocationComponent>().IsHibernation = false;
      objects = CreateObject("Objects", Ids.ObjectsId);
      storables = CreateObject("Storables", Ids.StorablesId);
      others = CreateObject("Others", Ids.OthersId);
    }

    public void Terminate()
    {
      DisposeRoot(Storables);
      storables = null;
      DisposeRoot(Objects);
      objects = null;
      DisposeRoot(Others);
      others = null;
      DisposeRoot(Hierarchy);
      hierarchy = null;
      if (entities.Count != 0)
      {
        Debug.LogError((object) ("Simulation is not empty, count : " + entities.Count));
        entities.Clear();
      }
      initialise = false;
    }

    private IEntity CreateObject(string name, Guid id)
    {
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      IEntity entity = ServiceCache.Factory.Create<IEntity>(id);
      entity.Name = name;
      entities.Add(entity.Id, entity);
      ((Entity) entity).OnAdded();
      return entity;
    }

    public void Add(IEntity entity, IEntity parent)
    {
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (entity.IsTemplate)
        Debug.LogError((object) ("Add template to simulation : " + entity.GetInfo()));
      ServiceCache.OptimizationService.FrameHasSpike = true;
      entities.Add(entity.Id, entity);
      ((IEntityHierarchy) parent).Add(entity);
      ((Entity) entity).OnAdded();
    }

    public void Remove(IEntity entity)
    {
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      ((Entity) entity).OnRemoved();
      IEntity parent = entity.Parent;
      if (parent != null)
      {
        ((IEntityHierarchy) parent).Remove(entity);
        entities.Remove(entity.Id);
      }
      else if (!Ids.IsRoot(entity.Id))
        throw new Exception(entity.GetInfo());
    }

    private void DisposeRoot(IEntity entity)
    {
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      ((Entity) entity).OnRemoved();
      entity.Dispose();
      entities.Remove(entity.Id);
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
              if (!entities.TryGetValue(id, out entity))
              {
                XmlElement pathNode = item["HierarchyPath"];
                Debug.LogError((object) ("Entity " + id + " not found , path : " + (pathNode != null ? pathNode.InnerText : (object) "null") + " , count : " + entities.Count + " , context : " + context));
              }
              else
              {
                XmlNodeDataReader reader = new XmlNodeDataReader(item, context);
                ((ISerializeStateLoad) entity).StateLoad(reader, typeof (Entity));
                DateTime currentTime = DateTime.UtcNow;
                if (time + minTime < currentTime)
                {
                  time = currentTime;
                  yield return null;
                }
                idNode = null;
                id = new Guid();
                entity = null;
                reader = null;
                item = null;
              }
            }
          }
        }
        foreach (KeyValuePair<Guid, IEntity> entity1 in entities)
        {
          KeyValuePair<Guid, IEntity> entity = entity1;
          MetaService.Compute(entity.Value, OnLoadedAttribute.Id);
          entity = new KeyValuePair<Guid, IEntity>();
        }
      }
    }

    public void Unload()
    {
      List<KeyValuePair<Guid, IEntity>> list = entities.ToList();
      for (int index = 0; index < list.Count; ++index)
      {
        KeyValuePair<Guid, IEntity> keyValuePair = list[index];
        if (!Ids.IsRoot(keyValuePair.Value.Id))
        {
          Debug.LogError((object) ("Wrong clenup simulation, entity not unloaded : " + keyValuePair.Value.GetInfo()));
          Remove(keyValuePair.Value);
          keyValuePair.Value.Dispose();
        }
      }
    }

    public void Save(IDataWriter writer, string context)
    {
      writer.Begin(nameof (Simulation), null, true);
      foreach (KeyValuePair<Guid, IEntity> entity1 in entities)
      {
        Entity entity2 = (Entity) entity1.Value;
        if (entity2.NeedSave)
          DefaultStateSaveUtility.SaveSerialize(writer, "Entity", entity2);
      }
      writer.End(nameof (Simulation), true);
    }

    public void AddPlayer(IEntity owner)
    {
      ((Entity) owner).IsPlayer = true;
      players.Remove(owner);
      players.Add(owner);
      Player = players.FirstOrDefault();
    }

    public void RemovePlayer(IEntity owner)
    {
      players.Remove(owner);
      Player = players.FirstOrDefault();
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
