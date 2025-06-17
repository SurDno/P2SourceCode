using System;
using System.Collections.Generic;
using System.Reflection;
using Engine.Common;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services.CameraServices
{
  [RuntimeService(typeof (CameraService))]
  public class CameraService : IInitialisable, IUpdatable
  {
    private CameraKindEnum kind = CameraKindEnum.Unknown;
    private IEntity target;
    private GameObject gameObjectTarget;
    private ICameraController emptyCameraController = new EmptyCameraController();
    private bool initialise;
    [Inspected]
    private ICameraController currentCameraController;
    private Dictionary<CameraKindEnum, ICameraController> controllers = new() {
      {
        CameraKindEnum.Fly,
        new FlyCameraController()
      },
      {
        CameraKindEnum.FirstPerson_Controlling,
        new FirstPersonCameraController()
      },
      {
        CameraKindEnum.FirstPerson_Tracking,
        new TrackingCameraController()
      },
      {
        CameraKindEnum.Cutscene,
        new CutsceneCameraController()
      },
      {
        CameraKindEnum.Cutscene_Cinemachine,
        new CutsceneCinemachineCameraController()
      },
      {
        CameraKindEnum.Ragdoll,
        new RagdollCameraController()
      },
      {
        CameraKindEnum.Dialog,
        new DialogCameraController(ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogRigPrefab)
      },
      {
        CameraKindEnum.Trade,
        new DialogCameraController(ScriptableObjectInstance<ResourceFromCodeData>.Instance.TradeRigPrefab)
      }
    };

    [Inspected(Mutable = true)]
    public CameraKindEnum Kind
    {
      get
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return kind;
      }
      set
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (kind == value)
          return;
        kind = value;
        if (currentCameraController == null)
          return;
        currentCameraController.Shutdown();
        if (!controllers.TryGetValue(kind, out currentCameraController))
          currentCameraController = emptyCameraController;
        currentCameraController.Initialise();
      }
    }

    [Inspected]
    public IEntity Target
    {
      get
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return target;
      }
      set
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (target == value)
          return;
        target = value;
      }
    }

    [Inspected]
    public GameObject GameObjectTarget
    {
      get
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return gameObjectTarget;
      }
      set
      {
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        gameObjectTarget = value;
      }
    }

    public void ComputeUpdate()
    {
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      currentCameraController.Update(target, gameObjectTarget);
    }

    public void Initialise()
    {
      initialise = true;
      InstanceByRequest<UpdateService>.Instance.CameraUpdater.AddUpdatable(this);
      currentCameraController = emptyCameraController;
      currentCameraController.Initialise();
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.CameraUpdater.RemoveUpdatable(this);
      initialise = false;
    }
  }
}
