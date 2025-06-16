using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Settings;
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
      if (!initialised)
        return;
      initialised = false;
      if ((Object) secondaryCameraObject != (Object) null)
      {
        Object.DestroyImmediate((Object) secondaryCameraObject);
        secondaryCameraObject = (GameObject) null;
      }
      if ((Object) unityRigInstance != (Object) null)
      {
        Object.DestroyImmediate((Object) unityRigInstance);
        unityRigInstance = (GameObject) null;
      }
      foreach (KeyValuePair<GameObject, int> storedLayer in storedLayers)
      {
        if ((Object) storedLayer.Key != (Object) null)
          storedLayer.Key.layer = storedLayer.Value;
      }
      storedLayers.Clear();
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      if ((Object) camera != (Object) null)
        camera.cullingMask = storedCullingMask;
      GameCamera.Instance.ResetCutsceneFov();
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      if (target == null)
        return;
      GameObject gameObject = ((IEntityView) target).GameObject;
      if (!initialised)
      {
        initialised = true;
        InternalInitialise(gameObject);
      }
      if ((Object) unityRigInstance == (Object) null || (Object) cameraPivotTransform == (Object) null || (Object) gameObject == (Object) null)
        return;
      Pivot component = gameObject.GetComponent<Pivot>();
      if ((Object) component == (Object) null || (Object) component.AnchorCameraFPS == (Object) null)
        return;
      if ((Object) unityRigInstance != (Object) null)
      {
        Transform transform = component.AnchorCameraFPS.transform;
        unityRigInstance.transform.localPosition = transform.position;
        unityRigInstance.transform.localRotation = transform.rotation;
        Vector3 localScale = transform.localScale;
        float num = (float) (((double) Mathf.Abs(localScale.x) + (double) Mathf.Abs(localScale.y) + (double) Mathf.Abs(localScale.z)) / 3.0);
        unityRigInstance.transform.localScale = new Vector3(num, num, num);
        DialogLightingProfile dialogLightingProfile = component.DialogLightingProfile;
        if ((Object) dialogLightingProfile == (Object) null)
          dialogLightingProfile = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultDialogLightingProfile;
        if ((Object) dialogLightingProfile != (Object) null)
        {
          if ((Object) keyLightPivot != (Object) null)
            keyLightPivot.localEulerAngles = (Vector3) dialogLightingProfile.KeyLightRotation;
          if ((Object) backLightPivot != (Object) null)
            backLightPivot.localEulerAngles = (Vector3) dialogLightingProfile.BackLightRotation;
          if ((Object) fillLightPivot != (Object) null)
            fillLightPivot.localEulerAngles = (Vector3) dialogLightingProfile.FillLightRotation;
        }
        keyLight.range = baseKeyLightRange * num;
        backLight.range = baseBackLightRange * num;
        fillLight.range = baseFillLightRange * num;
      }
      if ((Object) GameCamera.Instance.CameraTransform != (Object) null)
      {
        GameCamera.Instance.CameraTransform.position = cameraPivotTransform.transform.position;
        GameCamera.Instance.CameraTransform.rotation = cameraPivotTransform.transform.rotation;
      }
      if (!((Object) secondaryCameraObject != (Object) null))
        return;
      secondaryCameraObject.transform.localPosition = cameraPivotTransform.transform.position;
      secondaryCameraObject.transform.localRotation = cameraPivotTransform.transform.rotation;
      if ((Object) dof != (Object) null)
      {
        dof.focalTransform = (Object) component.DialogDOFTarget != (Object) null ? component.DialogDOFTarget.transform : (Transform) null;
        dof.enabled = InstanceByRequest<GraphicsGameSettings>.Instance.DOF.Value && (Object) dof.focalTransform != (Object) null;
      }
      if ((Object) postProcessing != (Object) null)
        postProcessing.Antialiasing.Override = !InstanceByRequest<GraphicsGameSettings>.Instance.Antialiasing.Value;
    }

    private void InternalInitialise(GameObject gameObjectTarget)
    {
      unityRigInstance = Object.Instantiate<GameObject>(rigPrefab);
      cameraPivotTransform = unityRigInstance.transform.Find("Camera");
      keyLightPivot = unityRigInstance.transform.Find("KeyLight");
      backLightPivot = unityRigInstance.transform.Find("BackLight");
      fillLightPivot = unityRigInstance.transform.Find("FillLight");
      keyLight = keyLightPivot.GetComponentInChildren<Light>();
      backLight = backLightPivot.GetComponentInChildren<Light>();
      fillLight = fillLightPivot.GetComponentInChildren<Light>();
      baseKeyLightRange = keyLight.range;
      baseBackLightRange = backLight.range;
      baseFillLightRange = fillLight.range;
      if ((Object) gameObjectTarget != (Object) null)
      {
        foreach (Component componentsInChild in gameObjectTarget.GetComponentsInChildren<Transform>())
        {
          GameObject gameObject = componentsInChild.gameObject;
          storedLayers.Add(gameObject, gameObject.layer);
          gameObject.layer = ScriptableObjectInstance<GameSettingsData>.Instance.DialogLayer.GetIndex();
        }
      }
      GameObject dialogCameraPrefab = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogCameraPrefab;
      if ((Object) dialogCameraPrefab != (Object) null)
      {
        secondaryCameraObject = Object.Instantiate<GameObject>(dialogCameraPrefab);
        dof = secondaryCameraObject.GetComponent<DepthOfField>();
        postProcessing = secondaryCameraObject.GetComponent<PostProcessingStackOverride>();
        UnityEngine.Camera component = secondaryCameraObject.GetComponent<UnityEngine.Camera>();
        if ((bool) (Object) component)
          GameCamera.Instance.SetCutsceneFov(component.fieldOfView);
      }
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      if (!((Object) camera != (Object) null))
        return;
      storedCullingMask = camera.cullingMask;
      camera.cullingMask &= ~(int) ScriptableObjectInstance<GameSettingsData>.Instance.DialogLayer;
    }
  }
}
