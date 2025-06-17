using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
  public sealed class CinemachineCore
  {
    public static readonly int kStreamingVersion = 20170927;
    public static readonly string kVersionString = "2.1";
    private static CinemachineCore sInstance;
    public static bool sShowHiddenObjects = false;
    private List<CinemachineBrain> mActiveBrains = [];
    private List<ICinemachineCamera> mActiveCameras = [];
    private List<List<ICinemachineCamera>> mChildCameras = [];
    private Dictionary<ICinemachineCamera, UpdateStatus> mUpdateStatus;

    public static CinemachineCore Instance
    {
      get
      {
        if (sInstance == null)
          sInstance = new CinemachineCore();
        return sInstance;
      }
    }

    public int BrainCount => mActiveBrains.Count;

    public CinemachineBrain GetActiveBrain(int index) => mActiveBrains[index];

    internal void AddActiveBrain(CinemachineBrain brain)
    {
      RemoveActiveBrain(brain);
      mActiveBrains.Insert(0, brain);
    }

    internal void RemoveActiveBrain(CinemachineBrain brain) => mActiveBrains.Remove(brain);

    public int VirtualCameraCount => mActiveCameras.Count;

    public ICinemachineCamera GetVirtualCamera(int index) => mActiveCameras[index];

    internal void AddActiveCamera(ICinemachineCamera vcam)
    {
      RemoveActiveCamera(vcam);
      int index = 0;
      while (index < mActiveCameras.Count && vcam.Priority < mActiveCameras[index].Priority)
        ++index;
      mActiveCameras.Insert(index, vcam);
    }

    internal void RemoveActiveCamera(ICinemachineCamera vcam) => mActiveCameras.Remove(vcam);

    internal void AddChildCamera(ICinemachineCamera vcam)
    {
      RemoveChildCamera(vcam);
      int num = 0;
      for (ICinemachineCamera cinemachineCamera = vcam; cinemachineCamera != null; cinemachineCamera = cinemachineCamera.ParentCamera)
        ++num;
      while (mChildCameras.Count < num)
        mChildCameras.Add([]);
      mChildCameras[num - 1].Add(vcam);
    }

    internal void RemoveChildCamera(ICinemachineCamera vcam)
    {
      for (int index = 0; index < mChildCameras.Count; ++index)
        mChildCameras[index].Remove(vcam);
    }

    internal void UpdateAllActiveVirtualCameras(Vector3 worldUp, float deltaTime)
    {
      for (int index1 = mChildCameras.Count - 1; index1 >= 0; --index1)
      {
        int count = mChildCameras[index1].Count;
        for (int index2 = 0; index2 < count; ++index2)
          UpdateVirtualCamera(mChildCameras[index1][index2], worldUp, deltaTime);
      }
      int virtualCameraCount = VirtualCameraCount;
      for (int index = 0; index < virtualCameraCount; ++index)
        UpdateVirtualCamera(GetVirtualCamera(index), worldUp, deltaTime);
    }

    internal bool UpdateVirtualCamera(ICinemachineCamera vcam, Vector3 worldUp, float deltaTime)
    {
      int frameCount = Time.frameCount;
      UpdateFilter updateFilter = CurrentUpdateFilter;
      bool flag1 = updateFilter != UpdateFilter.ForcedFixed && updateFilter != UpdateFilter.ForcedLate;
      bool flag2 = updateFilter == UpdateFilter.Late;
      if (!flag1)
      {
        if (updateFilter == UpdateFilter.ForcedFixed)
          updateFilter = UpdateFilter.Fixed;
        if (updateFilter == UpdateFilter.ForcedLate)
          updateFilter = UpdateFilter.Late;
      }
      if (mUpdateStatus == null)
        mUpdateStatus = new Dictionary<ICinemachineCamera, UpdateStatus>();
      if (vcam.VirtualCameraGameObject == null)
      {
        if (mUpdateStatus.ContainsKey(vcam))
          mUpdateStatus.Remove(vcam);
        return false;
      }

      if (!mUpdateStatus.TryGetValue(vcam, out UpdateStatus updateStatus))
      {
        updateStatus = new UpdateStatus(frameCount);
        mUpdateStatus.Add(vcam, updateStatus);
      }
      int num = flag2 ? 1 : CinemachineBrain.GetSubframeCount();
      if (updateStatus.lastUpdateFrame != frameCount)
        updateStatus.lastUpdateSubframe = 0;
      bool flag3 = !flag1;
      if (flag1)
      {
        flag3 = GetTargetPosition(vcam, out Matrix4x4 targetPos) ? updateStatus.ChoosePreferredUpdate(frameCount, targetPos, updateFilter) == updateFilter : flag2;
      }
      if (flag3)
      {
        updateStatus.preferredUpdate = updateFilter;
        for (; updateStatus.lastUpdateSubframe < num; ++updateStatus.lastUpdateSubframe)
          vcam.UpdateCameraState(worldUp, deltaTime);
        updateStatus.lastUpdateFrame = frameCount;
      }
      mUpdateStatus[vcam] = updateStatus;
      return true;
    }

    internal UpdateFilter CurrentUpdateFilter { get; set; }

    private static bool GetTargetPosition(ICinemachineCamera vcam, out Matrix4x4 targetPos)
    {
      ICinemachineCamera liveChildOrSelf = vcam.LiveChildOrSelf;
      if (liveChildOrSelf == null || liveChildOrSelf.VirtualCameraGameObject == null)
      {
        targetPos = Matrix4x4.identity;
        return false;
      }
      targetPos = liveChildOrSelf.VirtualCameraGameObject.transform.localToWorldMatrix;
      if (liveChildOrSelf.LookAt != null)
      {
        targetPos = liveChildOrSelf.LookAt.localToWorldMatrix;
        return true;
      }
      if (liveChildOrSelf.Follow != null)
      {
        targetPos = liveChildOrSelf.Follow.localToWorldMatrix;
        return true;
      }
      targetPos = vcam.VirtualCameraGameObject.transform.localToWorldMatrix;
      return true;
    }

    public UpdateFilter GetVcamUpdateStatus(ICinemachineCamera vcam)
    {
      return mUpdateStatus == null || !mUpdateStatus.TryGetValue(vcam, out UpdateStatus updateStatus) ? UpdateFilter.Late : updateStatus.preferredUpdate;
    }

    public bool IsLive(ICinemachineCamera vcam)
    {
      if (vcam != null)
      {
        for (int index = 0; index < BrainCount; ++index)
        {
          CinemachineBrain activeBrain = GetActiveBrain(index);
          if (activeBrain != null && activeBrain.IsLive(vcam))
            return true;
        }
      }
      return false;
    }

    public void GenerateCameraActivationEvent(ICinemachineCamera vcam)
    {
      if (vcam == null)
        return;
      for (int index = 0; index < BrainCount; ++index)
      {
        CinemachineBrain activeBrain = GetActiveBrain(index);
        if (activeBrain != null && activeBrain.IsLive(vcam))
          activeBrain.m_CameraActivatedEvent.Invoke(vcam);
      }
    }

    public void GenerateCameraCutEvent(ICinemachineCamera vcam)
    {
      if (vcam == null)
        return;
      for (int index = 0; index < BrainCount; ++index)
      {
        CinemachineBrain activeBrain = GetActiveBrain(index);
        if (activeBrain != null && activeBrain.IsLive(vcam))
          activeBrain.m_CameraCutEvent.Invoke(activeBrain);
      }
    }

    public CinemachineBrain FindPotentialTargetBrain(ICinemachineCamera vcam)
    {
      int brainCount = BrainCount;
      if (vcam != null && brainCount > 1)
      {
        for (int index = 0; index < brainCount; ++index)
        {
          CinemachineBrain activeBrain = GetActiveBrain(index);
          if (activeBrain != null && activeBrain.OutputCamera != null && activeBrain.IsLive(vcam))
            return activeBrain;
        }
      }
      for (int index = 0; index < brainCount; ++index)
      {
        CinemachineBrain activeBrain = GetActiveBrain(index);
        if (activeBrain != null && activeBrain.OutputCamera != null)
          return activeBrain;
      }
      return null;
    }

    public enum Stage
    {
      Body,
      Aim,
      Noise,
    }

    public delegate float AxisInputDelegate(string axisName);

    private struct UpdateStatus(int currentFrame) {
      private const int kWindowSize = 30;
      public int lastUpdateFrame = -1;
      public int lastUpdateSubframe = 0;
      public int windowStart = currentFrame;
      public int numWindowLateUpdateMoves = 0;
      public int numWindowFixedUpdateMoves = 0;
      public int numWindows = 0;
      public UpdateFilter preferredUpdate = UpdateFilter.Late;
      public Matrix4x4 targetPos = Matrix4x4.zero;

      public UpdateFilter ChoosePreferredUpdate(
        int currentFrame,
        Matrix4x4 pos,
        UpdateFilter updateFilter)
      {
        if (targetPos != pos)
        {
          if (updateFilter == UpdateFilter.Late)
            ++numWindowLateUpdateMoves;
          else if (lastUpdateSubframe == 0)
            ++numWindowFixedUpdateMoves;
          targetPos = pos;
        }
        UpdateFilter updateFilter1 = (numWindowLateUpdateMoves <= 0 || numWindowFixedUpdateMoves <= 0) && numWindowLateUpdateMoves < numWindowFixedUpdateMoves ? UpdateFilter.Fixed : UpdateFilter.Late;
        if (numWindows == 0)
          preferredUpdate = updateFilter1;
        if (windowStart + 30 <= currentFrame)
        {
          preferredUpdate = updateFilter1;
          ++numWindows;
          windowStart = currentFrame;
          numWindowLateUpdateMoves = numWindowFixedUpdateMoves = 0;
        }
        return preferredUpdate;
      }
    }

    public enum UpdateFilter
    {
      Fixed,
      ForcedFixed,
      Late,
      ForcedLate,
    }
  }
}
