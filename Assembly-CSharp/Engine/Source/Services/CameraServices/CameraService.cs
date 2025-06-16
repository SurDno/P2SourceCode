using Engine.Common;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Engine.Source.Services.CameraServices
{
  [RuntimeService(new System.Type[] {typeof (CameraService)})]
  public class CameraService : IInitialisable, IUpdatable
  {
    private CameraKindEnum kind = CameraKindEnum.Unknown;
    private IEntity target;
    private GameObject gameObjectTarget;
    private ICameraController emptyCameraController = (ICameraController) new EmptyCameraController();
    private bool initialise;
    [Inspected]
    private ICameraController currentCameraController;
    private Dictionary<CameraKindEnum, ICameraController> controllers = new Dictionary<CameraKindEnum, ICameraController>()
    {
      {
        CameraKindEnum.Fly,
        (ICameraController) new FlyCameraController()
      },
      {
        CameraKindEnum.FirstPerson_Controlling,
        (ICameraController) new FirstPersonCameraController()
      },
      {
        CameraKindEnum.FirstPerson_Tracking,
        (ICameraController) new TrackingCameraController()
      },
      {
        CameraKindEnum.Cutscene,
        (ICameraController) new CutsceneCameraController()
      },
      {
        CameraKindEnum.Cutscene_Cinemachine,
        (ICameraController) new CutsceneCinemachineCameraController()
      },
      {
        CameraKindEnum.Ragdoll,
        (ICameraController) new RagdollCameraController()
      },
      {
        CameraKindEnum.Dialog,
        (ICameraController) new DialogCameraController(ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogRigPrefab)
      },
      {
        CameraKindEnum.Trade,
        (ICameraController) new DialogCameraController(ScriptableObjectInstance<ResourceFromCodeData>.Instance.TradeRigPrefab)
      }
    };

    [Inspected(Mutable = true)]
    public CameraKindEnum Kind
    {
      get
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return this.kind;
      }
      set
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (this.kind == value)
          return;
        this.kind = value;
        if (this.currentCameraController == null)
          return;
        this.currentCameraController.Shutdown();
        if (!this.controllers.TryGetValue(this.kind, out this.currentCameraController))
          this.currentCameraController = this.emptyCameraController;
        this.currentCameraController.Initialise();
      }
    }

    [Inspected]
    public IEntity Target
    {
      get
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return this.target;
      }
      set
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (this.target == value)
          return;
        this.target = value;
      }
    }

    [Inspected]
    public GameObject GameObjectTarget
    {
      get
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return this.gameObjectTarget;
      }
      set
      {
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        this.gameObjectTarget = value;
      }
    }

    public void ComputeUpdate()
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      this.currentCameraController.Update(this.target, this.gameObjectTarget);
    }

    public void Initialise()
    {
      this.initialise = true;
      InstanceByRequest<UpdateService>.Instance.CameraUpdater.AddUpdatable((IUpdatable) this);
      this.currentCameraController = this.emptyCameraController;
      this.currentCameraController.Initialise();
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.CameraUpdater.RemoveUpdatable((IUpdatable) this);
      this.initialise = false;
    }
  }
}
