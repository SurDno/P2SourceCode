// Decompiled with JetBrains decompiler
// Type: Cinemachine.Timeline.CinemachineShot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#nullable disable
namespace Cinemachine.Timeline
{
  public sealed class CinemachineShot : PlayableAsset, IPropertyPreview
  {
    public ExposedReference<CinemachineVirtualCameraBase> VirtualCamera;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
      ScriptPlayable<CinemachineShotPlayable> playable = ScriptPlayable<CinemachineShotPlayable>.Create(graph);
      playable.GetBehaviour().VirtualCamera = this.VirtualCamera.Resolve(graph.GetResolver());
      return (Playable) playable;
    }

    public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
      driver.AddFromName<Transform>("m_LocalPosition.x");
      driver.AddFromName<Transform>("m_LocalPosition.y");
      driver.AddFromName<Transform>("m_LocalPosition.z");
      driver.AddFromName<Transform>("m_LocalRotation.x");
      driver.AddFromName<Transform>("m_LocalRotation.y");
      driver.AddFromName<Transform>("m_LocalRotation.z");
      driver.AddFromName<Camera>("field of view");
      driver.AddFromName<Camera>("near clip plane");
      driver.AddFromName<Camera>("far clip plane");
    }
  }
}
