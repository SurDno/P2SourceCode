using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Behaviours;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Impl.Services.HierarchyServices;
using Engine.Services.Engine.Assets;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.Components
{
  [Required(typeof (LocationComponent))]
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class StaticModelComponent : EngineComponent, IEntityEventsListener
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool relativePosition;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Name = "Scene")]
    [Inspected(Name = "Scene", Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<IScene> connection;
    [FromThis]
    private LocationComponent location;
    private SceneAsset sceneAsset;
    private bool needLoad = true;

    [Inspected]
    public bool NeedLoad
    {
      get => needLoad;
      set
      {
        needLoad = value;
        UpdateModel();
      }
    }

    public SceneAsset SceneAsset => sceneAsset;

    public Typed<IScene> Connection
    {
      get => connection;
      set => connection = value;
    }

    [Inspected]
    public bool RelativePosition => relativePosition;

    public override void OnAdded()
    {
      base.OnAdded();
      location.OnPlayerChanged += OnPlayerChanged;
      ((Entity) Owner).AddListener(this);
      ((IEntityView) Owner).OnGameObjectChangedEvent += OnGameObjectChangedEvent;
      UpdateModel();
    }

    public override void OnRemoved()
    {
      ((Entity) Owner).RemoveListener(this);
      ((IEntityView) Owner).OnGameObjectChangedEvent -= OnGameObjectChangedEvent;
      location.OnPlayerChanged -= OnPlayerChanged;
      location = null;
      base.OnRemoved();
    }

    private void OnPlayerChanged() => UpdateModel();

    private void OnEnableChanged() => UpdateModel();

    private void OnControllerChanged() => UpdateModel();

    private void UpdateModel()
    {
      IScene reference = connection.Value;
      if (reference == null)
        return;
      bool flag1 = Owner.IsEnabledInHierarchy && ((IEntityView) Owner).IsAttached && (NeedLoad || location.Player != null);
      if (flag1 && sceneAsset == null)
      {
        sceneAsset = ServiceLocator.GetService<AssetLoader>().CreateSceneAsset(reference, OnLoad, Owner.GetInfo());
      }
      else
      {
        if (flag1 || sceneAsset == null)
          return;
        string[] strArray = new string[8];
        strArray[0] = "IsEnabledInHierarchy : ";
        strArray[1] = Owner.IsEnabledInHierarchy.ToString();
        strArray[2] = " , IsAttached : ";
        bool flag2 = ((IEntityView) Owner).IsAttached;
        strArray[3] = flag2.ToString();
        strArray[4] = " , NeedLoad : ";
        flag2 = NeedLoad;
        strArray[5] = flag2.ToString();
        strArray[6] = " , location.Player != null : ";
        flag2 = location.Player != null;
        strArray[7] = flag2.ToString();
        Clear(string.Concat(strArray));
      }
    }

    private void OnGameObjectChangedEvent() => UpdateModel();

    private IEnumerator OnLoad(bool success)
    {
      if (!success)
      {
        Clear("Error load asset");
      }
      else
      {
        if (sceneAsset == null)
          throw new Exception(Owner.GetInfo());
        if (!sceneAsset.IsValid)
          throw new Exception(Owner.GetInfo());
        HierarchyContainer container = ServiceLocator.GetService<HierarchyService>().GetContainer(Connection.Id);
        if (container == null)
          throw new Exception("Container not found for connection id : " + Connection.Id + " , owner : " + Owner.GetInfo());
        SceneObjectContainer sceneContainer = SceneObjectContainer.GetContainer(sceneAsset.Scene);
        if ((UnityEngine.Object) sceneContainer == (UnityEngine.Object) null)
          throw new Exception(typeof (SceneObjectContainer).Name + " not found, path : " + sceneAsset.Path + " , owner : " + Owner.GetInfo());
        if (relativePosition)
          MoveToPosition();
        if (Owner.Childs != null)
        {
          List<IEntity> childs = new List<IEntity>();
          CollectChilds(Owner.Childs, sceneContainer, container, childs);
          foreach (IEntity entity in childs)
          {
            IEntity child = entity;
            HierarchyItem item = container.GetItemByTemplateId(child.TemplateId);
            if (item != null)
            {
              GameObject go = sceneContainer.GetGameObject(item.Reference.Id);
              if ((UnityEngine.Object) go == (UnityEngine.Object) null)
              {
                Debug.LogError((object) ("Map entity not found : " + child.GetInfo()));
              }
              else
              {
                ((IEntityViewSetter) child).GameObject = go;
                EntityViewUtility.FromTransformToData(child);
                yield return null;
                item = null;
                go = (GameObject) null;
                child = null;
              }
            }
          }
          childs = null;
        }
        Profiler.BeginSample("AssetLoader IsHibernation");
        location.IsHibernation = false;
        Profiler.EndSample();
      }
    }

    private void OnDispose()
    {
      if (location == null)
        return;
      location.IsHibernation = true;
    }

    private void Clear(string reason)
    {
      if (sceneAsset == null)
        return;
      CleanupContainer();
      ServiceLocator.GetService<AssetLoader>().DisposeAsset(sceneAsset, OnDispose, reason);
      sceneAsset = null;
    }

    private void CleanupContainer()
    {
      HierarchyContainer container = ServiceLocator.GetService<HierarchyService>().GetContainer(Connection.Id);
      if (container == null)
        throw new Exception(Owner.GetInfo());
      if (Owner.Childs == null)
        return;
      CleanupHierarchy(Owner.Childs, container);
    }

    private void MoveToPosition()
    {
      foreach (GameObject rootGameObject in sceneAsset.Scene.GetRootGameObjects())
      {
        Matrix4x4 matrix4x4 = Matrix4x4.TRS(rootGameObject.transform.localPosition, rootGameObject.transform.localRotation, rootGameObject.transform.localScale);
        Vector3 position;
        Quaternion rotation;
        EntityViewUtility.ConvertMatrix(Matrix4x4.TRS(((IEntityView) Owner).Position, ((IEntityView) Owner).Rotation, Vector3.one) * matrix4x4, out position, out rotation);
        Transform transform = rootGameObject.transform.transform;
        transform.position = position;
        transform.rotation = rotation;
      }
    }

    private static void CollectChilds(
      IEnumerable<IEntity> childs,
      SceneObjectContainer sceneContainer,
      HierarchyContainer hierachyContainer,
      List<IEntity> collectedChilds)
    {
      foreach (IEntity child in childs)
      {
        collectedChilds.Add(child);
        if (child.Childs != null)
          CollectChilds(child.Childs, sceneContainer, hierachyContainer, collectedChilds);
      }
    }

    private static void CleanupHierarchy(
      IEnumerable<IEntity> childs,
      HierarchyContainer hierachyContainer)
    {
      foreach (IEntity child in childs)
      {
        if (hierachyContainer.GetItemByTemplateId(child.TemplateId) != null)
        {
          ((IEntityViewSetter) child).GameObject = (GameObject) null;
          if (child.Childs != null)
            CleanupHierarchy(child.Childs, hierachyContainer);
        }
      }
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      OnEnableChanged();
    }
  }
}
