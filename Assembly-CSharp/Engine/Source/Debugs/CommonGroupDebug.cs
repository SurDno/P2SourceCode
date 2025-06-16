using System;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using InputServices;
using SRF;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class CommonGroupDebug
  {
    [Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(new UpdatableProxy((Action) (() => Update()))));
    }

    private static void Update()
    {
      if (InputUtility.IsKeyDown(KeyCode.F12, KeyModifficator.Control))
      {
        bool visible = CursorService.Instance.Visible;
        CursorService.Instance.Visible = !visible;
        CursorService.Instance.Free = !visible;
      }
      if (InputUtility.IsKeyDown(KeyCode.F11, KeyModifficator.Control))
      {
        bool flag = !ServiceLocator.GetService<UIService>().Visible;
        ServiceLocator.GetService<UIService>().Visible = flag;
        Transform transform = Hierarchy.Get("SRDebugger/UI");
        if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
          transform.gameObject.SetActive(flag);
        ServiceLocator.GetService<GizmoService>().Visible = flag;
      }
      if (!InstanceByRequest<EngineApplication>.Instance.IsDebug || !InputUtility.IsKeyDown(KeyCode.Home))
        return;
      if (ServiceLocator.GetService<CameraService>().Kind == CameraKindEnum.Fly)
        ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.FirstPerson_Controlling;
      else
        ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Fly;
    }
  }
}
