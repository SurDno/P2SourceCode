using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
      foreach (TimelineClip clip in GetClips())
      {
        CinemachineVirtualCameraBase virtualCameraBase = ((CinemachineShot) clip.asset).VirtualCamera.Resolve(graph.GetResolver());
        if (virtualCameraBase != null)
          clip.displayName = virtualCameraBase.Name;
      }
      ScriptPlayable<CinemachineMixer> playable = ScriptPlayable<CinemachineMixer>.Create(graph);
      playable.SetInputCount(inputCount);
      return playable;
    }
  }
}
