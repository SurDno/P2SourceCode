// Decompiled with JetBrains decompiler
// Type: Cinemachine.Timeline.CinemachineTrack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#nullable disable
namespace Cinemachine.Timeline
{
  [TrackClipType(typeof (CinemachineShot))]
  [TrackMediaType(TimelineAsset.MediaType.Script)]
  [TrackBindingType(typeof (CinemachineBrain))]
  [TrackColor(0.53f, 0.0f, 0.08f)]
  [Serializable]
  public class CinemachineTrack : TrackAsset
  {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
      foreach (TimelineClip clip in this.GetClips())
      {
        CinemachineVirtualCameraBase virtualCameraBase = ((CinemachineShot) clip.asset).VirtualCamera.Resolve(graph.GetResolver());
        if ((UnityEngine.Object) virtualCameraBase != (UnityEngine.Object) null)
          clip.displayName = virtualCameraBase.Name;
      }
      ScriptPlayable<CinemachineMixer> playable = ScriptPlayable<CinemachineMixer>.Create(graph);
      playable.SetInputCount<ScriptPlayable<CinemachineMixer>>(inputCount);
      return (Playable) playable;
    }
  }
}
