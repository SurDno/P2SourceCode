﻿using System;
using Cofe.Meta;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Engine.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class SelectionGroupDebug
  {
    private static string name = "[Selection]";
    private static KeyCode key = KeyCode.E;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static KeyCode eyeKey = KeyCode.F1;
    private static KeyCode hearingKey = KeyCode.F2;
    private static KeyCode movableKey = KeyCode.F3;
    private static Color headerColor = Color.green;
    private static Color trueColor = Color.white;
    private static Color falseColor = ColorPreset.LightGray;
    private static BoolPlayerProperty eyeVisible;
    private static BoolPlayerProperty hearingVisible;
    private static BoolPlayerProperty movableVisible;

    [Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() =>
      {
        GroupDebugService.RegisterGroup(name, key, modifficators, Update);
        eyeVisible = BoolPlayerProperty.Create(() => eyeVisible);
        hearingVisible = BoolPlayerProperty.Create(() => hearingVisible);
        movableVisible = BoolPlayerProperty.Create(() => movableVisible);
      });
    }

    private static void Update()
    {
      if (InputUtility.IsKeyDown(eyeKey, KeyModifficator.Control))
        eyeVisible.Value = !eyeVisible;
      if (InputUtility.IsKeyDown(hearingKey, KeyModifficator.Control))
        hearingVisible.Value = !hearingVisible;
      if (InputUtility.IsKeyDown(movableKey, KeyModifficator.Control))
        movableVisible.Value = !movableVisible;
      Vector3 characterPosition = Vector3.zero;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player != null)
      {
        GameObject gameObject = ((IEntityView) player).GameObject;
        if (gameObject != null)
        {
          characterPosition = gameObject.transform.position;
          GizmoService service = ServiceLocator.GetService<GizmoService>();
          DetectableComponent component1 = player.GetComponent<DetectableComponent>();
          if (component1 != null)
          {
            Vector3 position = gameObject.transform.position + gameObject.transform.rotation * new Vector3(0.0f, 0.0f, 2f);
            service.DrawText3d(position, "", TextCorner.None, Color.clear);
            SelectionGroupDebugUtility.DrawDetectable(component1, gameObject.transform.position, gameObject.transform.rotation, eyeVisible, hearingVisible);
          }
          PlayerControllerComponent component2 = player.GetComponent<PlayerControllerComponent>();
          if (component2 != null)
          {
            foreach (NpcControllerComponent near in component2.Nears)
            {
              if (!near.IsDisposed)
              {
                Vector3 position = ((IEntityView) near.Owner).Position;
                service.DrawLine(position, position + Vector3.up, Color.cyan);
              }
            }
          }
        }
      }
      ComputeHotkeys();
      DrawInfo();
      DrawSelected(characterPosition);
      DrawHelp();
    }

    private static void DrawInfo()
    {
      string text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
      PickingService service = ServiceLocator.GetService<PickingService>();
      if (service == null)
        return;
      if (service.TargetGameObject == null)
      {
        string text2 = "  Object not found";
        ServiceLocator.GetService<GizmoService>().DrawText(text2, falseColor);
      }
      else
      {
        Vector3 vector3 = new Vector3(0.01f, 0.01f, 0.01f);
        Bounds bounds = SelectionGroupDebugUtility.GetBounds(service.TargetGameObject);
        Color color1 = service.TargetEntity != null ? new Color(1f, 0.1f, 0.0f, 1f) : Color.yellow;
        ServiceLocator.GetService<GizmoService>().DrawBox(bounds.min - vector3, bounds.max + vector3, color1);
        Color color2 = service.TargetEntity != null ? ColorPreset.Orange : Color.yellow;
        string str = "  Distance : " + service.TargetGameObjectDistance;
        IEntity targetEntity = service.TargetEntity;
        if (targetEntity != null)
          str = str + "\n  Entity : " + targetEntity.GetInfo() + "\n  Context : " + (targetEntity.Context ?? "null");
        string text3 = str + "\n  GameObject : " + service.TargetGameObject.GetFullName();
        ServiceLocator.GetService<GizmoService>().DrawText(text3, color2);
      }
    }

    private static void ComputeHotkeys()
    {
      PickingService service = ServiceLocator.GetService<PickingService>();
      if (service == null)
        return;
      int index = -1;
      if (InputUtility.IsKeyDown(KeyCode.Alpha0, KeyModifficator.Shift))
        index = 0;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha1, KeyModifficator.Shift))
        index = 1;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha2, KeyModifficator.Shift))
        index = 2;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha3, KeyModifficator.Shift))
        index = 3;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha4, KeyModifficator.Shift))
        index = 4;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha5, KeyModifficator.Shift))
        index = 5;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha6, KeyModifficator.Shift))
        index = 6;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha7, KeyModifficator.Shift))
        index = 7;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha8, KeyModifficator.Shift))
        index = 8;
      else if (InputUtility.IsKeyDown(KeyCode.Alpha9, KeyModifficator.Shift))
        index = 9;
      if (index == -1)
        return;
      ServiceLocator.GetService<SelectionService>().SetSelection(index, null);
      IEntity targetEntity = service.TargetEntity;
      if (targetEntity != null)
      {
        ServiceLocator.GetService<SelectionService>().SetSelection(index, targetEntity);
        Debug.Log(ObjectInfoUtility.GetStream().Append("Store object : ").GetInfo(targetEntity).Append(" , index : ").Append(index).Append(" , type : ").Append(TypeUtility.GetTypeName(targetEntity.GetType())));
      }
      else
      {
        GameObject targetGameObject = service.TargetGameObject;
        if (targetGameObject != null)
        {
          ServiceLocator.GetService<SelectionService>().SetSelection(index, targetGameObject);
          Debug.Log(ObjectInfoUtility.GetStream().Append("Store object : ").GetFullName(targetGameObject).Append(" , index : ").Append(index).Append(" , type : ").Append(TypeUtility.GetTypeName(targetGameObject.GetType())));
        }
      }
    }

    private static void DrawSelected(Vector3 characterPosition)
    {
      string text1 = "\n[Slots]";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
      SelectionService service = ServiceLocator.GetService<SelectionService>();
      if (service == null)
        return;
      int selectionCount = service.SelectionCount;
      for (int index = 0; index < selectionCount; ++index)
      {
        object selection = service.GetSelection(index);
        IObject engineObject = selection as IObject;
        GameObject go = selection as GameObject;
        bool flag = false;
        string str = "  Slot : " + index + " , ";
        string text2;
        if (engineObject != null)
          text2 = str + "Name : " + engineObject.Name + " , Type : " + engineObject.GetType().Name;
        else if (go != null)
        {
          text2 = str + "Name : " + go.name + " , Type : " + typeof (GameObject).Name;
        }
        else
        {
          text2 = str + "Empty";
          flag = true;
        }
        DrawEntityGizmo(engineObject, go, index, characterPosition);
        ServiceLocator.GetService<GizmoService>().DrawText(text2, flag ? falseColor : trueColor);
      }
    }

    private static void DrawHelp()
    {
      string text1 = "\n[Gizmos]";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
      string text2 = "  Eye " + (eyeVisible ? "True" : (object) "False") + " [Control + " + eyeKey + "]";
      ServiceLocator.GetService<GizmoService>().DrawText(text2, eyeVisible ? trueColor : falseColor);
      string text3 = "  Hearing " + (hearingVisible ? "True" : (object) "False") + " [Control + " + hearingKey + "]";
      ServiceLocator.GetService<GizmoService>().DrawText(text3, hearingVisible ? trueColor : falseColor);
      string text4 = "  Movable " + (movableVisible ? "True" : (object) "False") + " [Control + " + movableKey + "]";
      ServiceLocator.GetService<GizmoService>().DrawText(text4, movableVisible ? trueColor : falseColor);
    }

    private static void DrawEntityGizmo(
      IObject engineObject,
      GameObject go,
      int index,
      Vector3 characterPosition)
    {
      IEntity entity = engineObject as IEntity;
      string name;
      if (go == null)
      {
        if (entity == null || entity.IsDisposed)
          return;
        go = ((IEntityView) entity).GameObject;
        if (go == null)
          return;
        name = " , Name : " + entity.Name + "\nId : " + entity.Id + "\nContext : " + (entity.Context ?? "null");
      }
      else
        name = " , Name : " + go.name;
      Bounds bounds = SelectionGroupDebugUtility.GetBounds(go);
      DrawEntity(index, name, bounds, go.transform.position, characterPosition);
      if (entity == null)
        return;
      if (movableVisible)
        SelectionGroupDebugUtility.DrawPath(go, go.transform.position, go.transform.rotation, bounds);
      Vector3 position = ((bounds.max - bounds.min) / 2f + bounds.min) with
      {
        y = bounds.min.y
      };
      ServiceLocator.GetService<GizmoService>().DrawText3d(position, "", TextCorner.None, Color.clear);
      if (eyeVisible)
      {
        DetectorComponent component1 = entity.GetComponent<DetectorComponent>();
        if (component1 != null)
          SelectionGroupDebugUtility.DrawDetector(component1, go.transform.position, go.transform.rotation, eyeVisible, false);
        DetectableComponent component2 = entity.GetComponent<DetectableComponent>();
        if (component2 != null)
          SelectionGroupDebugUtility.DrawDetectable(component2, go.transform.position, go.transform.rotation, eyeVisible, false);
      }
      if (!hearingVisible)
        return;
      DetectorComponent component3 = entity.GetComponent<DetectorComponent>();
      if (component3 != null)
        SelectionGroupDebugUtility.DrawDetector(component3, go.transform.position, go.transform.rotation, false, hearingVisible);
      DetectableComponent component4 = entity.GetComponent<DetectableComponent>();
      if (component4 != null)
        SelectionGroupDebugUtility.DrawDetectable(component4, go.transform.position, go.transform.rotation, false, hearingVisible);
    }

    private static void DrawEntity(
      int index,
      string name,
      Bounds bounds,
      Vector3 position,
      Vector3 characterPosition)
    {
      ServiceLocator.GetService<GizmoService>().DrawBox(bounds.min, bounds.max, Color.green);
      string text = "Slot : " + index + name + "\nPosition : " + position + " , Distance : " + (characterPosition - position).magnitude.ToString("F2");
      Vector3 position1 = ((bounds.max - bounds.min) / 2f + bounds.min) with
      {
        y = bounds.max.y
      };
      ServiceLocator.GetService<GizmoService>().DrawText3d(position1, text, TextCorner.Up, Color.green);
    }
  }
}
