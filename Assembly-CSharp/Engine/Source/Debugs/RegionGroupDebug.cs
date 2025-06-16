using System;
using System.Collections.Generic;
using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using Scripts.Behaviours.LoadControllers;
using UnityEngine;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class RegionGroupDebug
  {
    private static string name = "[Regions]";
    private static KeyCode key = KeyCode.G;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = Color.magenta;
    private static Color trueColor = Color.white;
    private static Color falseColor = ColorPreset.LightGray;
    private static Color currentColor = ColorPreset.Green;

    [Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(name, key, modifficators, Update));
    }

    private static void Update()
    {
      string text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
      foreach (RegionEnum name in Enum.GetValues(typeof (RegionEnum)))
      {
        RegionComponent regionByName = RegionUtility.GetRegionByName(name);
        if (regionByName != null && regionByName.Owner.GetComponent<ILocationComponent>() is LocationComponent component1)
        {
          string str = "  " + regionByName.Owner.Name + " , Hibernation : " + component1.IsHibernation;
          IEnumerable<ILocationComponent> childs = component1.Childs;
          int num1 = 0;
          int num2 = 0;
          foreach (ILocationComponent locationComponent in childs)
          {
            ++num2;
            if (!locationComponent.IsHibernation)
              ++num1;
          }
          string text2 = str + " , Loaded childs (" + num1 + " / " + num2 + ")";
          GameObject gameObject = ((IEntityView) regionByName.Owner).GameObject;
          if (gameObject == null)
          {
            string text3 = text2 + " , Error : go == null";
            ServiceLocator.GetService<GizmoService>().DrawText(text3, GetColor(component1));
          }
          else
          {
            BaseLoadByDistance component = gameObject.GetComponent<BaseLoadByDistance>();
            if (component == null)
            {
              string text4 = text2 + " , Error : reginController == null";
              ServiceLocator.GetService<GizmoService>().DrawText(text4, GetColor(component1));
            }
            else
            {
              ServiceLocator.GetService<GizmoService>().DrawText(text2, GetColor(component1));
              RegionMesh regionMesh = regionByName.RegionMesh;
              if (!(regionMesh == null))
              {
                float magnitude = (regionMesh.Center - EngineApplication.PlayerPosition).magnitude;
                string text5 = regionByName.Owner.Name + "\nDistance : " + magnitude.ToString("F2") + "\nRadius : " + regionMesh.Radius.ToString("F2") + "\nLoad Distance : " + (component.LoadDistance + regionMesh.Radius).ToString("F2") + "\nUnload Distance : " + (component.UnloadDistance + regionMesh.Radius).ToString("F2");
                ServiceLocator.GetService<GizmoService>().DrawText3d(regionMesh.Center, text5, TextCorner.None, GetColor(component1));
              }
            }
          }
        }
      }
    }

    private static Color GetColor(LocationComponent location)
    {
      return location.IsHibernation ? falseColor : (location.Player != null ? currentColor : trueColor);
    }
  }
}
