// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineCore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  public sealed class CinemachineCore
  {
    public static readonly int kStreamingVersion = 20170927;
    public static readonly string kVersionString = "2.1";
    private static CinemachineCore sInstance = (CinemachineCore) null;
    public static bool sShowHiddenObjects = false;
    private List<CinemachineBrain> mActiveBrains = new List<CinemachineBrain>();
    private List<ICinemachineCamera> mActiveCameras = new List<ICinemachineCamera>();
    private List<List<ICinemachineCamera>> mChildCameras = new List<List<ICinemachineCamera>>();
    private Dictionary<ICinemachineCamera, CinemachineCore.UpdateStatus> mUpdateStatus;

    public static CinemachineCore Instance
    {
      get
      {
        if (CinemachineCore.sInstance == null)
          CinemachineCore.sInstance = new CinemachineCore();
        return CinemachineCore.sInstance;
      }
    }

    public int BrainCount => this.mActiveBrains.Count;

    public CinemachineBrain GetActiveBrain(int index) => this.mActiveBrains[index];

    internal void AddActiveBrain(CinemachineBrain brain)
    {
      this.RemoveActiveBrain(brain);
      this.mActiveBrains.Insert(0, brain);
    }

    internal void RemoveActiveBrain(CinemachineBrain brain) => this.mActiveBrains.Remove(brain);

    public int VirtualCameraCount => this.mActiveCameras.Count;

    public ICinemachineCamera GetVirtualCamera(int index) => this.mActiveCameras[index];

    internal void AddActiveCamera(ICinemachineCamera vcam)
    {
      this.RemoveActiveCamera(vcam);
      int index = 0;
      while (index < this.mActiveCameras.Count && vcam.Priority < this.mActiveCameras[index].Priority)
        ++index;
      this.mActiveCameras.Insert(index, vcam);
    }

    internal void RemoveActiveCamera(ICinemachineCamera vcam) => this.mActiveCameras.Remove(vcam);

    internal void AddChildCamera(ICinemachineCamera vcam)
    {
      this.RemoveChildCamera(vcam);
      int num = 0;
      for (ICinemachineCamera cinemachineCamera = vcam; cinemachineCamera != null; cinemachineCamera = cinemachineCamera.ParentCamera)
        ++num;
      while (this.mChildCameras.Count < num)
        this.mChildCameras.Add(new List<ICinemachineCamera>());
      this.mChildCameras[num - 1].Add(vcam);
    }

    internal void RemoveChildCamera(ICinemachineCamera vcam)
    {
      for (int index = 0; index < this.mChildCameras.Count; ++index)
        this.mChildCameras[index].Remove(vcam);
    }

    internal void UpdateAllActiveVirtualCameras(Vector3 worldUp, float deltaTime)
    {
      for (int index1 = this.mChildCameras.Count - 1; index1 >= 0; --index1)
      {
        int count = this.mChildCameras[index1].Count;
        for (int index2 = 0; index2 < count; ++index2)
          this.UpdateVirtualCamera(this.mChildCameras[index1][index2], worldUp, deltaTime);
      }
      int virtualCameraCount = this.VirtualCameraCount;
      for (int index = 0; index < virtualCameraCount; ++index)
        this.UpdateVirtualCamera(this.GetVirtualCamera(index), worldUp, deltaTime);
    }

    internal bool UpdateVirtualCamera(ICinemachineCamera vcam, Vector3 worldUp, float deltaTime)
    {
      int frameCount = Time.frameCount;
      CinemachineCore.UpdateFilter updateFilter = this.CurrentUpdateFilter;
      bool flag1 = updateFilter != CinemachineCore.UpdateFilter.ForcedFixed && updateFilter != CinemachineCore.UpdateFilter.ForcedLate;
      bool flag2 = updateFilter == CinemachineCore.UpdateFilter.Late;
      if (!flag1)
      {
        if (updateFilter == CinemachineCore.UpdateFilter.ForcedFixed)
          updateFilter = CinemachineCore.UpdateFilter.Fixed;
        if (updateFilter == CinemachineCore.UpdateFilter.ForcedLate)
          updateFilter = CinemachineCore.UpdateFilter.Late;
      }
      if (this.mUpdateStatus == null)
        this.mUpdateStatus = new Dictionary<ICinemachineCamera, CinemachineCore.UpdateStatus>();
      if ((Object) vcam.VirtualCameraGameObject == (Object) null)
      {
        if (this.mUpdateStatus.ContainsKey(vcam))
          this.mUpdateStatus.Remove(vcam);
        return false;
      }
      CinemachineCore.UpdateStatus updateStatus;
      if (!this.mUpdateStatus.TryGetValue(vcam, out updateStatus))
      {
        updateStatus = new CinemachineCore.UpdateStatus(frameCount);
        this.mUpdateStatus.Add(vcam, updateStatus);
      }
      int num = flag2 ? 1 : CinemachineBrain.GetSubframeCount();
      if (updateStatus.lastUpdateFrame != frameCount)
        updateStatus.lastUpdateSubframe = 0;
      bool flag3 = !flag1;
      if (flag1)
      {
        Matrix4x4 targetPos;
        flag3 = CinemachineCore.GetTargetPosition(vcam, out targetPos) ? updateStatus.ChoosePreferredUpdate(frameCount, targetPos, updateFilter) == updateFilter : flag2;
      }
      if (flag3)
      {
        updateStatus.preferredUpdate = updateFilter;
        for (; updateStatus.lastUpdateSubframe < num; ++updateStatus.lastUpdateSubframe)
          vcam.UpdateCameraState(worldUp, deltaTime);
        updateStatus.lastUpdateFrame = frameCount;
      }
      this.mUpdateStatus[vcam] = updateStatus;
      return true;
    }

    internal CinemachineCore.UpdateFilter CurrentUpdateFilter { get; set; }

    private static bool GetTargetPosition(ICinemachineCamera vcam, out Matrix4x4 targetPos)
    {
      ICinemachineCamera liveChildOrSelf = vcam.LiveChildOrSelf;
      if (liveChildOrSelf == null || (Object) liveChildOrSelf.VirtualCameraGameObject == (Object) null)
      {
        targetPos = Matrix4x4.identity;
        return false;
      }
      targetPos = liveChildOrSelf.VirtualCameraGameObject.transform.localToWorldMatrix;
      if ((Object) liveChildOrSelf.LookAt != (Object) null)
      {
        targetPos = liveChildOrSelf.LookAt.localToWorldMatrix;
        return true;
      }
      if ((Object) liveChildOrSelf.Follow != (Object) null)
      {
        targetPos = liveChildOrSelf.Follow.localToWorldMatrix;
        return true;
      }
      targetPos = vcam.VirtualCameraGameObject.transform.localToWorldMatrix;
      return true;
    }

    public CinemachineCore.UpdateFilter GetVcamUpdateStatus(ICinemachineCamera vcam)
    {
      CinemachineCore.UpdateStatus updateStatus;
      return this.mUpdateStatus == null || !this.mUpdateStatus.TryGetValue(vcam, out updateStatus) ? CinemachineCore.UpdateFilter.Late : updateStatus.preferredUpdate;
    }

    public bool IsLive(ICinemachineCamera vcam)
    {
      if (vcam != null)
      {
        for (int index = 0; index < this.BrainCount; ++index)
        {
          CinemachineBrain activeBrain = this.GetActiveBrain(index);
          if ((Object) activeBrain != (Object) null && activeBrain.IsLive(vcam))
            return true;
        }
      }
      return false;
    }

    public void GenerateCameraActivationEvent(ICinemachineCamera vcam)
    {
      if (vcam == null)
        return;
      for (int index = 0; index < this.BrainCount; ++index)
      {
        CinemachineBrain activeBrain = this.GetActiveBrain(index);
        if ((Object) activeBrain != (Object) null && activeBrain.IsLive(vcam))
          activeBrain.m_CameraActivatedEvent.Invoke(vcam);
      }
    }

    public void GenerateCameraCutEvent(ICinemachineCamera vcam)
    {
      if (vcam == null)
        return;
      for (int index = 0; index < this.BrainCount; ++index)
      {
        CinemachineBrain activeBrain = this.GetActiveBrain(index);
        if ((Object) activeBrain != (Object) null && activeBrain.IsLive(vcam))
          activeBrain.m_CameraCutEvent.Invoke(activeBrain);
      }
    }

    public CinemachineBrain FindPotentialTargetBrain(ICinemachineCamera vcam)
    {
      int brainCount = this.BrainCount;
      if (vcam != null && brainCount > 1)
      {
        for (int index = 0; index < brainCount; ++index)
        {
          CinemachineBrain activeBrain = this.GetActiveBrain(index);
          if ((Object) activeBrain != (Object) null && (Object) activeBrain.OutputCamera != (Object) null && activeBrain.IsLive(vcam))
            return activeBrain;
        }
      }
      for (int index = 0; index < brainCount; ++index)
      {
        CinemachineBrain activeBrain = this.GetActiveBrain(index);
        if ((Object) activeBrain != (Object) null && (Object) activeBrain.OutputCamera != (Object) null)
          return activeBrain;
      }
      return (CinemachineBrain) null;
    }

    public enum Stage
    {
      Body,
      Aim,
      Noise,
    }

    public delegate float AxisInputDelegate(string axisName);

    private struct UpdateStatus
    {
      private const int kWindowSize = 30;
      public int lastUpdateFrame;
      public int lastUpdateSubframe;
      public int windowStart;
      public int numWindowLateUpdateMoves;
      public int numWindowFixedUpdateMoves;
      public int numWindows;
      public CinemachineCore.UpdateFilter preferredUpdate;
      public Matrix4x4 targetPos;

      public UpdateStatus(int currentFrame)
      {
        this.lastUpdateFrame = -1;
        this.lastUpdateSubframe = 0;
        this.windowStart = currentFrame;
        this.numWindowLateUpdateMoves = 0;
        this.numWindowFixedUpdateMoves = 0;
        this.numWindows = 0;
        this.preferredUpdate = CinemachineCore.UpdateFilter.Late;
        this.targetPos = Matrix4x4.zero;
      }

      public CinemachineCore.UpdateFilter ChoosePreferredUpdate(
        int currentFrame,
        Matrix4x4 pos,
        CinemachineCore.UpdateFilter updateFilter)
      {
        if (this.targetPos != pos)
        {
          if (updateFilter == CinemachineCore.UpdateFilter.Late)
            ++this.numWindowLateUpdateMoves;
          else if (this.lastUpdateSubframe == 0)
            ++this.numWindowFixedUpdateMoves;
          this.targetPos = pos;
        }
        CinemachineCore.UpdateFilter updateFilter1 = (this.numWindowLateUpdateMoves <= 0 || this.numWindowFixedUpdateMoves <= 0) && this.numWindowLateUpdateMoves < this.numWindowFixedUpdateMoves ? CinemachineCore.UpdateFilter.Fixed : CinemachineCore.UpdateFilter.Late;
        if (this.numWindows == 0)
          this.preferredUpdate = updateFilter1;
        if (this.windowStart + 30 <= currentFrame)
        {
          this.preferredUpdate = updateFilter1;
          ++this.numWindows;
          this.windowStart = currentFrame;
          this.numWindowLateUpdateMoves = this.numWindowFixedUpdateMoves = 0;
        }
        return this.preferredUpdate;
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
