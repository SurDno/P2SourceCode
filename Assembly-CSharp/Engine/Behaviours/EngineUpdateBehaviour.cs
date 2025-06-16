// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.EngineUpdateBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours
{
  [DisallowMultipleComponent]
  public class EngineUpdateBehaviour : MonoBehaviour
  {
    private bool computeConsole;

    private void Awake()
    {
      this.computeConsole = !Application.isEditor && ScriptableObjectInstance<BuildData>.Instance.Development && ScriptableObjectInstance<BuildData>.Instance.Release;
      if (!this.computeConsole)
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
      if (!this.computeConsole || !Debug.developerConsoleVisible)
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
