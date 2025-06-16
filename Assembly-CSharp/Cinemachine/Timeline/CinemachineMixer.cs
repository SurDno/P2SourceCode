using UnityEngine;
using UnityEngine.Playables;

namespace Cinemachine.Timeline
{
  public sealed class CinemachineMixer : PlayableBehaviour
  {
    private CinemachineBrain mBrain;
    private int mBrainOverrideId = -1;
    private bool mPlaying;
    private float mLastOverrideFrame;

    public override void OnGraphStop(Playable playable)
    {
      if (mBrain != null)
        mBrain.ReleaseCameraOverride(mBrainOverrideId);
      mBrainOverrideId = -1;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
      base.ProcessFrame(playable, info, playerData);
      GameObject gameObject = playerData as GameObject;
      mBrain = !(gameObject == null) ? gameObject.GetComponent<CinemachineBrain>() : (CinemachineBrain) playerData;
      if (mBrain == null)
        return;
      int num = 0;
      ICinemachineCamera camB = null;
      ICinemachineCamera camA = null;
      float weightB = 1f;
      for (int index = 0; index < playable.GetInputCount(); ++index)
      {
        CinemachineShotPlayable behaviour = ((ScriptPlayable<CinemachineShotPlayable>) playable.GetInput(index)).GetBehaviour();
        float inputWeight = playable.GetInputWeight(index);
        if (behaviour != null && behaviour.VirtualCamera != null && playable.GetPlayState() == PlayState.Playing && inputWeight > 9.9999997473787516E-05)
        {
          if (num == 1)
            camA = camB;
          weightB = inputWeight;
          camB = behaviour.VirtualCamera;
          ++num;
          if (num == 2)
            break;
        }
      }
      float deltaTime = info.deltaTime;
      if (!mPlaying)
      {
        if (mBrainOverrideId < 0)
          mLastOverrideFrame = -1f;
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        deltaTime = Time.unscaledDeltaTime;
        if (!Application.isPlaying && (mLastOverrideFrame < 0.0 || realtimeSinceStartup - (double) mLastOverrideFrame > Time.maximumDeltaTime))
          deltaTime = -1f;
        mLastOverrideFrame = realtimeSinceStartup;
      }
      mBrainOverrideId = mBrain.SetCameraOverride(mBrainOverrideId, camA, camB, weightB, deltaTime);
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
      mPlaying = info.evaluationType == FrameData.EvaluationType.Playback;
    }
  }
}
