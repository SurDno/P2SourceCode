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
      if ((Object) mBrain != (Object) null)
        mBrain.ReleaseCameraOverride(mBrainOverrideId);
      mBrainOverrideId = -1;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
      base.ProcessFrame(playable, info, playerData);
      GameObject gameObject = playerData as GameObject;
      mBrain = !((Object) gameObject == (Object) null) ? gameObject.GetComponent<CinemachineBrain>() : (CinemachineBrain) playerData;
      if ((Object) mBrain == (Object) null)
        return;
      int num = 0;
      ICinemachineCamera camB = null;
      ICinemachineCamera camA = null;
      float weightB = 1f;
      for (int index = 0; index < playable.GetInputCount<Playable>(); ++index)
      {
        CinemachineShotPlayable behaviour = ((ScriptPlayable<CinemachineShotPlayable>) playable.GetInput<Playable>(index)).GetBehaviour();
        float inputWeight = playable.GetInputWeight<Playable>(index);
        if (behaviour != null && (Object) behaviour.VirtualCamera != (Object) null && playable.GetPlayState<Playable>() == PlayState.Playing && inputWeight > 9.9999997473787516E-05)
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
        if (!Application.isPlaying && (mLastOverrideFrame < 0.0 || realtimeSinceStartup - (double) mLastOverrideFrame > (double) Time.maximumDeltaTime))
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
