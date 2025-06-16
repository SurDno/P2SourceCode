// Decompiled with JetBrains decompiler
// Type: Engine.Source.Debugs.SelectionGroupDebugUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Gizmos;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
namespace Engine.Source.Debugs
{
  public static class SelectionGroupDebugUtility
  {
    public static void DrawPath(
      GameObject go,
      Vector3 position,
      Quaternion rotation,
      Bounds bounds)
    {
      NavMeshAgent component = go.GetComponent<NavMeshAgent>();
      if ((Object) component == (Object) null)
        return;
      NavMeshPath path = component.path;
      GizmoService service = ServiceLocator.GetService<GizmoService>();
      int num = NavMeshUtility.DrawPath(component, service);
      string text = "NavMeshAgent status : " + (object) component.pathStatus + "\nNavMeshAgent hasPath : " + component.hasPath.ToString() + "\nNavMeshAgent isOnNavMesh : " + component.isOnNavMesh.ToString();
      if (component.hasPath && component.isOnNavMesh)
        text = text + "\nNavMeshAgent remaining distance : " + (object) component.remainingDistance + "\nCorner count : " + (object) num;
      service.DrawText3d(go.transform.position, text, TextCorner.Down, Color.white);
    }

    public static void DrawDetector(
      DetectorComponent detector,
      Vector3 position,
      Quaternion rotation,
      bool eyeVisible,
      bool hearingVisible)
    {
      GizmoService service = ServiceLocator.GetService<GizmoService>();
      if (eyeVisible)
      {
        float baseEyeDistance = detector.BaseEyeDistance;
        float eyeDistance = detector.EyeDistance;
        float eyeAngle = detector.EyeAngle;
        Color yellow = Color.yellow;
        float startAngle = (float) (360.0 - (double) rotation.eulerAngles.y + 90.0) - eyeAngle / 2f;
        service.DrawEyeSector(position, baseEyeDistance, startAngle, startAngle + eyeAngle, yellow, false);
        service.DrawEyeSector(position, eyeDistance, startAngle, startAngle + eyeAngle, yellow);
        int num = 0;
        foreach (IDetectableComponent detectableComponent in detector.Visible)
        {
          if (detectableComponent != null && !detectableComponent.IsDisposed)
          {
            GameObject gameObject = ((IEntityView) detectableComponent.Owner).GameObject;
            if (!((Object) gameObject == (Object) null))
            {
              service.DrawLine(position, gameObject.transform.position, yellow);
              ++num;
            }
          }
        }
        string text = "Eye" + " , Angle : " + (object) eyeAngle + " , Base Distance : " + (object) baseEyeDistance + " , Distance : " + (object) eyeDistance + " , Count : " + (object) num;
        service.DrawText3d(text, TextCorner.Down, yellow);
      }
      if (!hearingVisible)
        return;
      Color red = Color.red;
      float baseHearingDistance = detector.BaseHearingDistance;
      float hearingDistance = detector.HearingDistance;
      service.DrawCircle(position, baseHearingDistance, red, false);
      service.DrawCircle(position, hearingDistance, red);
      LocationItemComponent component = detector.Owner.GetComponent<LocationItemComponent>();
      bool flag = component != null && component.IsIndoor;
      int num1 = 0;
      Vector3 vector3 = rotation * new Vector3(0.02f, 0.0f, 0.0f);
      foreach (IDetectableComponent detectableComponent in detector.Hearing)
      {
        if (detectableComponent != null && !detectableComponent.IsDisposed)
        {
          GameObject gameObject = ((IEntityView) detectableComponent.Owner).GameObject;
          if (!((Object) gameObject == (Object) null))
          {
            service.DrawLine(position + vector3, gameObject.transform.position + vector3, red);
            ++num1;
          }
        }
      }
      string text1 = "Hearing" + " , Base Distance : " + (object) baseHearingDistance + " , Distance : " + (object) hearingDistance + " , Count : " + (object) num1;
      service.DrawText3d(text1, TextCorner.Down, red);
    }

    public static void DrawDetectable(
      DetectableComponent detectable,
      Vector3 position,
      Quaternion rotation,
      bool eyeVisible,
      bool hearingVisible)
    {
      if (eyeVisible)
      {
        Color green = Color.green;
        ServiceLocator.GetService<GizmoService>().DrawCircle(position, detectable.BaseVisibleDistance, green, false);
        ServiceLocator.GetService<GizmoService>().DrawCircle(position, detectable.VisibleDistance, green);
        string text = "Visible" + " , Distance : " + (object) detectable.BaseVisibleDistance + " , Current distance : " + (object) detectable.VisibleDistance + " , Detect type : " + (object) detectable.VisibleDetectType;
        ServiceLocator.GetService<GizmoService>().DrawText3d(text, TextCorner.Down, green);
      }
      if (!hearingVisible)
        return;
      Color magenta = Color.magenta;
      ServiceLocator.GetService<GizmoService>().DrawCircle(position, detectable.NoiseDistance, magenta);
      string text1 = "Noise" + " , Distance : " + (object) detectable.NoiseDistance + " , Detect type : " + (object) detectable.NoiseDetectType;
      ServiceLocator.GetService<GizmoService>().DrawText3d(text1, TextCorner.Down, magenta);
    }

    public static Bounds GetBounds(GameObject go)
    {
      Collider[] componentsInChildren = go.GetComponentsInChildren<Collider>();
      if (componentsInChildren.Length == 0)
        return new Bounds();
      Bounds bounds = componentsInChildren[0].bounds;
      for (int index = 1; index < componentsInChildren.Length; ++index)
        bounds.Encapsulate(componentsInChildren[index].bounds);
      return bounds;
    }
  }
}
