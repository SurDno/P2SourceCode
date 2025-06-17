using System;
using System.Collections.Generic;
using AssetDatabases;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Connections;
using FlowCanvas;
using UnityEngine;

namespace Engine.Source.Services
{
  public static class BlueprintServiceUtility
  {
    private static Dictionary<ResourceRequest, Description> asyncRequests = new();

    public static FlowScriptController Start(
      GameObject blueprintPrefab,
      IEntity target,
      Action complete,
      string context = "")
    {
      if (blueprintPrefab == null)
      {
        Debug.LogError("prefab is null , context : " + context);
        Action action = complete;
        if (action != null)
          action();
        return null;
      }
      Debug.Log(ObjectInfoUtility.GetStream().Append("Start blueprint : ").Append(blueprintPrefab.name).Append(" , target : ").GetInfo(target).Append(" , context : ").Append(context));
      if (blueprintPrefab.GetComponent<FlowScriptController>() == null)
      {
        Debug.LogError(typeof (FlowScriptController).Name + " not found , context : " + context);
        Action action = complete;
        if (action != null)
          action();
        return null;
      }
      GameObject gameObject1 = UnityFactory.Instantiate(blueprintPrefab, "[Blueprints]");
      gameObject1.name = blueprintPrefab.name;
      FlowScriptController component = gameObject1.GetComponent<FlowScriptController>();
      Action<FlowScriptController> destroy = null;
      destroy = graph =>
      {
        graph.DestroyEvent -= destroy;
        Action action = complete;
        if (action == null)
          return;
        action();
      };
      component.DestroyEvent += destroy;
      component.enableAction = FlowScriptController.EnableAction.DoNothing;
      component.StartBehaviour();
      if (target != null)
      {
        component.SetValue("Target", target);
        GameObject gameObject2 = ((IEntityView) target).GameObject;
        if (gameObject2 != null)
          component.SetValue("TargetGameObject", gameObject2);
      }
      component.SendEvent(nameof (Start));
      return component;
    }

    public static FlowScriptController Start(
      IBlueprintObject bp,
      IEntity target,
      Action complete,
      string context = "")
    {
      BlueprintObject blueprintObject = (BlueprintObject) bp;
      if (blueprintObject != null)
        return Start(blueprintObject.GameObject, target, complete, context);
      Debug.LogError("Blueprint is null , context : " + context);
      if (complete != null)
        complete();
      return null;
    }

    public static void Start(
      UnityAsset<GameObject> blueprint,
      IAbilityValueContainer container,
      IEntity target,
      string context = "")
    {
      GameObject prefab = blueprint.Value;
      if (prefab == null)
        return;
      FlowScriptController component = UnityFactory.Instantiate(prefab, "[Blueprints]").GetComponent<FlowScriptController>();
      if (component == null)
        return;
      component.WaitForThreadFinish();
      component.StartBehaviour();
      component.SetValue("AbilityValueContainer", container);
      component.SetValue("Target", target);
      GameObject gameObject = ((IEntityView) target).GameObject;
      if (gameObject == null)
        Debug.LogError("Blueprint start failed, GameObject not found, target : " + target.GetInfo() + " , context : " + context);
      component.SetValue("TargetGameObject", gameObject);
      component.SendEvent(nameof (Start));
    }

    public static void StartAsync(
      UnityAsset<GameObject> blueprint,
      IEntity target,
      Action resourcesLoaded,
      Action complete,
      bool onlyResourcesAsync,
      string context = "")
    {
      StartAsync(blueprint, null, target, resourcesLoaded, complete, onlyResourcesAsync, context);
    }

    public static void StartAsync(
      UnityAsset<GameObject> blueprint,
      IAbilityValueContainer container,
      IEntity target,
      Action resourcesLoaded,
      Action complete,
      bool onlyResourcesAsync,
      string context = "")
    {
      string resourcePath = AssetDatabaseUtility.ConvertToResourcePath(AssetDatabaseService.Instance.GetPath(blueprint.Id));
      if (resourcePath.IsNullOrEmpty())
        return;
      ResourceRequest key = Resources.LoadAsync(resourcePath);
      if (key == null)
        return;
      key.completed += Request_completed;
      asyncRequests.Add(key, new Description {
        Blueprint = blueprint,
        Container = container,
        Target = target,
        Context = context,
        ResourcesLoaded = resourcesLoaded,
        Complete = complete,
        OnlyResourcesAsync = onlyResourcesAsync
      });
    }

    private static void Request_completed(AsyncOperation operation)
    {
      operation.completed -= Request_completed;
      Description description = asyncRequests[(ResourceRequest) operation];
      asyncRequests.Remove((ResourceRequest) operation);
      Action resourcesLoaded = description.ResourcesLoaded;
      if (resourcesLoaded != null)
        resourcesLoaded();
      if (description.Target.IsDisposed)
        return;
      GameObject gameObject = ((IEntityView) description.Target).GameObject;
      if (gameObject == null)
      {
        Debug.LogError("Blueprint async start failed, GameObject not found, target (this is not a bug) : " + description.Target.GetInfo() + " , context : " + description.Context);
      }
      else
      {
        GameObject prefab = description.Blueprint.Value;
        if (prefab == null)
          return;
        FlowScriptController component = UnityFactory.Instantiate(prefab, "[Blueprints]").GetComponent<FlowScriptController>();
        if (component == null)
          return;
        if (description.OnlyResourcesAsync)
          component.WaitForThreadFinish();
        if (description.Container != null)
          component.SetValue("AbilityValueContainer", description.Container);
        if (description.Target != null)
          component.SetValue("Target", description.Target);
        component.SetValue("TargetGameObject", gameObject);
        Action<FlowScriptController> destroy = null;
        destroy = graph =>
        {
          graph.DestroyEvent -= destroy;
          Action complete = description.Complete;
          if (complete == null)
            return;
          complete();
        };
        component.DestroyEvent += destroy;
        component.enableAction = FlowScriptController.EnableAction.DoNothing;
        component.StartBehaviour();
        component.SendEvent("Start");
      }
    }

    private struct Description
    {
      public UnityAsset<GameObject> Blueprint;
      public IAbilityValueContainer Container;
      public IEntity Target;
      public string Context;
      public Action ResourcesLoaded;
      public Action Complete;
      public bool OnlyResourcesAsync;
    }
  }
}
