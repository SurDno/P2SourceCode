using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using UnityEngine;

namespace Engine.Behaviours
{
  [DisallowMultipleComponent]
  public class EngineUpdateBehaviour : MonoBehaviour
  {
    private bool computeConsole;

    private void Awake()
    {
      computeConsole = !Application.isEditor && ScriptableObjectInstance<BuildData>.Instance.Development && ScriptableObjectInstance<BuildData>.Instance.Release;
      if (!computeConsole)
        return;
      Debug.ClearDeveloperConsole();
      Debug.developerConsoleVisible = false;
    }

    private void Start() => InstanceByRequest<UpdateService>.Instance.Start();

    private void Update() => InstanceByRequest<UpdateService>.Instance.Update();

    private void LateUpdate()
    {
      InstanceByRequest<UpdateService>.Instance.LateUpdate();
      ServiceCache.OptimizationService.FrameHasSpike = false;
      IEntity player = ServiceCache.Simulation.Player;
      if (player != null)
        EngineApplication.PlayerPosition = ((IEntityView) player).Position;
      EngineApplication.FrameCount = Time.frameCount;
      if (!computeConsole || !Debug.developerConsoleVisible)
        return;
      Debug.ClearDeveloperConsole();
      Debug.developerConsoleVisible = false;
    }

    private void OnApplicationFocus(bool focus)
    {
      InstanceByRequest<EngineApplication>.Instance.FireApplicationFocusEvent(focus);
    }
  }
}
