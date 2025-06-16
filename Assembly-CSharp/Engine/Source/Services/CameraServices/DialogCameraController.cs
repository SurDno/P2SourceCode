using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Engine.Source.Services.CameraServices
{
  public class DialogCameraController : ICameraController
  {
    private readonly GameObject rigPrefab;
    private GameObject unityRigInstance;
    private GameObject secondaryCameraObject;
    private DepthOfField dof;
    private PostProcessingStackOverride postProcessing;
    private int storedCullingMask;
    private Dictionary<GameObject, int> storedLayers = new Dictionary<GameObject, int>();
    private bool initialised;
    private Transform cameraPivotTransform;
    private Transform keyLightPivot;
    private Transform backLightPivot;
    private Transform fillLightPivot;
    private Light keyLight;
    private Light backLight;
    private Light fillLight;
    private float baseKeyLightRange;
    private float baseBackLightRange;
    private float baseFillLightRange;

    public DialogCameraController(GameObject rigPrefab) => this.rigPrefab = rigPrefab;

    public void Initialise()
    {
    }

    public void Shutdown()
    {
      if (!this.initialised)
        return;
      this.initialised = false;
      if ((Object) this.secondaryCameraObject != (Object) null)
      {
        Object.DestroyImmediate((Object) this.secondaryCameraObject);
        this.secondaryCameraObject = (GameObject) null;
      }
      if ((Object) this.unityRigInstance != (Object) null)
      {
        Object.DestroyImmediate((Object) this.unityRigInstance);
        this.unityRigInstance = (GameObject) null;
      }
      foreach (KeyValuePair<GameObject, int> storedLayer in this.storedLayers)
      {
        if ((Object) storedLayer.Key != (Object) null)
          storedLayer.Key.layer = storedLayer.Value;
      }
      this.storedLayers.Clear();
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      if ((Object) camera != (Object) null)
        camera.cullingMask = this.storedCullingMask;
      GameCamera.Instance.ResetCutsceneFov();
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      if (target == null)
        return;
      GameObject gameObject = ((IEntityView) target).GameObject;
      if (!this.initialised)
      {
        this.initialised = true;
        this.InternalInitialise(gameObject);
      }
      if ((Object) this.unityRigInstance == (Object) null || (Object) this.cameraPivotTransform == (Object) null || (Object) gameObject == (Object) null)
        return;
      Pivot component = gameObject.GetComponent<Pivot>();
      if ((Object) component == (Object) null || (Object) component.AnchorCameraFPS == (Object) null)
        return;
      if ((Object) this.unityRigInstance != (Object) null)
      {
        Transform transform = component.AnchorCameraFPS.transform;
        this.unityRigInstance.transform.localPosition = transform.position;
        this.unityRigInstance.transform.localRotation = transform.rotation;
        Vector3 localScale = transform.localScale;
        float num = (float) (((double) Mathf.Abs(localScale.x) + (double) Mathf.Abs(localScale.y) + (double) Mathf.Abs(localScale.z)) / 3.0);
        this.unityRigInstance.transform.localScale = new Vector3(num, num, num);
        DialogLightingProfile dialogLightingProfile = component.DialogLightingProfile;
        if ((Object) dialogLightingProfile == (Object) null)
          dialogLightingProfile = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultDialogLightingProfile;
        if ((Object) dialogLightingProfile != (Object) null)
        {
          if ((Object) this.keyLightPivot != (Object) null)
            this.keyLightPivot.localEulerAngles = (Vector3) dialogLightingProfile.KeyLightRotation;
          if ((Object) this.backLightPivot != (Object) null)
            this.backLightPivot.localEulerAngles = (Vector3) dialogLightingProfile.BackLightRotation;
          if ((Object) this.fillLightPivot != (Object) null)
            this.fillLightPivot.localEulerAngles = (Vector3) dialogLightingProfile.FillLightRotation;
        }
        this.keyLight.range = this.baseKeyLightRange * num;
        this.backLight.range = this.baseBackLightRange * num;
        this.fillLight.range = this.baseFillLightRange * num;
      }
      if ((Object) GameCamera.Instance.CameraTransform != (Object) null)
      {
        GameCamera.Instance.CameraTransform.position = this.cameraPivotTransform.transform.position;
        GameCamera.Instance.CameraTransform.rotation = this.cameraPivotTransform.transform.rotation;
      }
      if (!((Object) this.secondaryCameraObject != (Object) null))
        return;
      this.secondaryCameraObject.transform.localPosition = this.cameraPivotTransform.transform.position;
      this.secondaryCameraObject.transform.localRotation = this.cameraPivotTransform.transform.rotation;
      if ((Object) this.dof != (Object) null)
      {
        this.dof.focalTransform = (Object) component.DialogDOFTarget != (Object) null ? component.DialogDOFTarget.transform : (Transform) null;
        this.dof.enabled = InstanceByRequest<GraphicsGameSettings>.Instance.DOF.Value && (Object) this.dof.focalTransform != (Object) null;
      }
      if ((Object) this.postProcessing != (Object) null)
        this.postProcessing.Antialiasing.Override = !InstanceByRequest<GraphicsGameSettings>.Instance.Antialiasing.Value;
    }

    private void InternalInitialise(GameObject gameObjectTarget)
    {
      this.unityRigInstance = Object.Instantiate<GameObject>(this.rigPrefab);
      this.cameraPivotTransform = this.unityRigInstance.transform.Find("Camera");
      this.keyLightPivot = this.unityRigInstance.transform.Find("KeyLight");
      this.backLightPivot = this.unityRigInstance.transform.Find("BackLight");
      this.fillLightPivot = this.unityRigInstance.transform.Find("FillLight");
      this.keyLight = this.keyLightPivot.GetComponentInChildren<Light>();
      this.backLight = this.backLightPivot.GetComponentInChildren<Light>();
      this.fillLight = this.fillLightPivot.GetComponentInChildren<Light>();
      this.baseKeyLightRange = this.keyLight.range;
      this.baseBackLightRange = this.backLight.range;
      this.baseFillLightRange = this.fillLight.range;
      if ((Object) gameObjectTarget != (Object) null)
      {
        foreach (Component componentsInChild in gameObjectTarget.GetComponentsInChildren<Transform>())
        {
          GameObject gameObject = componentsInChild.gameObject;
          this.storedLayers.Add(gameObject, gameObject.layer);
          gameObject.layer = ScriptableObjectInstance<GameSettingsData>.Instance.DialogLayer.GetIndex();
        }
      }
      GameObject dialogCameraPrefab = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogCameraPrefab;
      if ((Object) dialogCameraPrefab != (Object) null)
      {
        this.secondaryCameraObject = Object.Instantiate<GameObject>(dialogCameraPrefab);
        this.dof = this.secondaryCameraObject.GetComponent<DepthOfField>();
        this.postProcessing = this.secondaryCameraObject.GetComponent<PostProcessingStackOverride>();
        UnityEngine.Camera component = this.secondaryCameraObject.GetComponent<UnityEngine.Camera>();
        if ((bool) (Object) component)
          GameCamera.Instance.SetCutsceneFov(component.fieldOfView);
      }
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      if (!((Object) camera != (Object) null))
        return;
      this.storedCullingMask = camera.cullingMask;
      camera.cullingMask &= ~(int) ScriptableObjectInstance<GameSettingsData>.Instance.DialogLayer;
    }
  }
}
