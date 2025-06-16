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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Engine.Source.Components
{
  [Required(typeof (LocationComponent))]
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class StaticModelComponent : EngineComponent, IEntityEventsListener
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool relativePosition;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
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
      get => this.needLoad;
      set
      {
        this.needLoad = value;
        this.UpdateModel();
      }
    }

    public SceneAsset SceneAsset => this.sceneAsset;

    public Typed<IScene> Connection
    {
      get => this.connection;
      set => this.connection = value;
    }

    [Inspected]
    public bool RelativePosition => this.relativePosition;

    public override void OnAdded()
    {
      base.OnAdded();
      this.location.OnPlayerChanged += new Action(this.OnPlayerChanged);
      ((Entity) this.Owner).AddListener((IEntityEventsListener) this);
      ((IEntityView) this.Owner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChangedEvent);
      this.UpdateModel();
    }

    public override void OnRemoved()
    {
      ((Entity) this.Owner).RemoveListener((IEntityEventsListener) this);
      ((IEntityView) this.Owner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChangedEvent);
      this.location.OnPlayerChanged -= new Action(this.OnPlayerChanged);
      this.location = (LocationComponent) null;
      base.OnRemoved();
    }

    private void OnPlayerChanged() => this.UpdateModel();

    private void OnEnableChanged() => this.UpdateModel();

    private void OnControllerChanged() => this.UpdateModel();

    private void UpdateModel()
    {
      IScene reference = this.connection.Value;
      if (reference == null)
        return;
      bool flag1 = this.Owner.IsEnabledInHierarchy && ((IEntityView) this.Owner).IsAttached && (this.NeedLoad || this.location.Player != null);
      if (flag1 && this.sceneAsset == null)
      {
        this.sceneAsset = ServiceLocator.GetService<AssetLoader>().CreateSceneAsset(reference, new Func<bool, IEnumerator>(this.OnLoad), this.Owner.GetInfo());
      }
      else
      {
        if (flag1 || this.sceneAsset == null)
          return;
        string[] strArray = new string[8];
        strArray[0] = "IsEnabledInHierarchy : ";
        strArray[1] = this.Owner.IsEnabledInHierarchy.ToString();
        strArray[2] = " , IsAttached : ";
        bool flag2 = ((IEntityView) this.Owner).IsAttached;
        strArray[3] = flag2.ToString();
        strArray[4] = " , NeedLoad : ";
        flag2 = this.NeedLoad;
        strArray[5] = flag2.ToString();
        strArray[6] = " , location.Player != null : ";
        flag2 = this.location.Player != null;
        strArray[7] = flag2.ToString();
        this.Clear(string.Concat(strArray));
      }
    }

    private void OnGameObjectChangedEvent() => this.UpdateModel();

    private IEnumerator OnLoad(bool success)
    {
      if (!success)
      {
        this.Clear("Error load asset");
      }
      else
      {
        if (this.sceneAsset == null)
          throw new Exception(this.Owner.GetInfo());
        if (!this.sceneAsset.IsValid)
          throw new Exception(this.Owner.GetInfo());
        HierarchyContainer container = ServiceLocator.GetService<HierarchyService>().GetContainer(this.Connection.Id);
        if (container == null)
          throw new Exception("Container not found for connection id : " + (object) this.Connection.Id + " , owner : " + this.Owner.GetInfo());
        SceneObjectContainer sceneContainer = SceneObjectContainer.GetContainer(this.sceneAsset.Scene);
        if ((UnityEngine.Object) sceneContainer == (UnityEngine.Object) null)
          throw new Exception(typeof (SceneObjectContainer).Name + " not found, path : " + this.sceneAsset.Path + " , owner : " + this.Owner.GetInfo());
        if (this.relativePosition)
          this.MoveToPosition();
        if (this.Owner.Childs != null)
        {
          List<IEntity> childs = new List<IEntity>();
          StaticModelComponent.CollectChilds(this.Owner.Childs, sceneContainer, container, childs);
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
                yield return (object) null;
                item = (HierarchyItem) null;
                go = (GameObject) null;
                child = (IEntity) null;
              }
            }
          }
          childs = (List<IEntity>) null;
        }
        Profiler.BeginSample("AssetLoader IsHibernation");
        this.location.IsHibernation = false;
        Profiler.EndSample();
      }
    }

    private void OnDispose()
    {
      if (this.location == null)
        return;
      this.location.IsHibernation = true;
    }

    private void Clear(string reason)
    {
      if (this.sceneAsset == null)
        return;
      this.CleanupContainer();
      ServiceLocator.GetService<AssetLoader>().DisposeAsset((IAsset) this.sceneAsset, new Action(this.OnDispose), reason);
      this.sceneAsset = (SceneAsset) null;
    }

    private void CleanupContainer()
    {
      HierarchyContainer container = ServiceLocator.GetService<HierarchyService>().GetContainer(this.Connection.Id);
      if (container == null)
        throw new Exception(this.Owner.GetInfo());
      if (this.Owner.Childs == null)
        return;
      StaticModelComponent.CleanupHierarchy(this.Owner.Childs, container);
    }

    private void MoveToPosition()
    {
      foreach (GameObject rootGameObject in this.sceneAsset.Scene.GetRootGameObjects())
      {
        Matrix4x4 matrix4x4 = Matrix4x4.TRS(rootGameObject.transform.localPosition, rootGameObject.transform.localRotation, rootGameObject.transform.localScale);
        Vector3 position;
        Quaternion rotation;
        EntityViewUtility.ConvertMatrix(Matrix4x4.TRS(((IEntityView) this.Owner).Position, ((IEntityView) this.Owner).Rotation, Vector3.one) * matrix4x4, out position, out rotation);
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
          StaticModelComponent.CollectChilds(child.Childs, sceneContainer, hierachyContainer, collectedChilds);
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
            StaticModelComponent.CleanupHierarchy(child.Childs, hierachyContainer);
        }
      }
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      this.OnEnableChanged();
    }
  }
}
