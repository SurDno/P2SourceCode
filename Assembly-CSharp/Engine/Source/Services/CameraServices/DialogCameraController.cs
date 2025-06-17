using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Engine.Source.Services.CameraServices
{
  public class DialogCameraController(GameObject rigPrefab) : ICameraController 
  {
    private GameObject unityRigInstance;
    private GameObject secondaryCameraObject;
    private DepthOfField dof;
    private PostProcessingStackOverride postProcessing;
    private int storedCullingMask;
    private Dictionary<GameObject, int> storedLayers = new();
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

    public void Initialise()
    {
    }

    public void Shutdown()
    {
      if (!initialised)
        return;
      initialised = false;
      if (secondaryCameraObject != null)
      {
        Object.DestroyImmediate(secondaryCameraObject);
        secondaryCameraObject = null;
      }
      if (unityRigInstance != null)
      {
        Object.DestroyImmediate(unityRigInstance);
        unityRigInstance = null;
      }
      foreach (KeyValuePair<GameObject, int> storedLayer in storedLayers)
      {
        if (storedLayer.Key != null)
          storedLayer.Key.layer = storedLayer.Value;
      }
      storedLayers.Clear();
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      if (camera != null)
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
      if (unityRigInstance == null || cameraPivotTransform == null || gameObject == null)
        return;
      Pivot component = gameObject.GetComponent<Pivot>();
      if (component == null || component.AnchorCameraFPS == null)
        return;
      if (unityRigInstance != null)
      {
        Transform transform = component.AnchorCameraFPS.transform;
        unityRigInstance.transform.localPosition = transform.position;
        unityRigInstance.transform.localRotation = transform.rotation;
        Vector3 localScale = transform.localScale;
        float num = (float) ((Mathf.Abs(localScale.x) + (double) Mathf.Abs(localScale.y) + Mathf.Abs(localScale.z)) / 3.0);
        unityRigInstance.transform.localScale = new Vector3(num, num, num);
        DialogLightingProfile dialogLightingProfile = component.DialogLightingProfile;
        if (dialogLightingProfile == null)
          dialogLightingProfile = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultDialogLightingProfile;
        if (dialogLightingProfile != null)
        {
          if (keyLightPivot != null)
            keyLightPivot.localEulerAngles = dialogLightingProfile.KeyLightRotation;
          if (backLightPivot != null)
            backLightPivot.localEulerAngles = dialogLightingProfile.BackLightRotation;
          if (fillLightPivot != null)
            fillLightPivot.localEulerAngles = dialogLightingProfile.FillLightRotation;
        }
        keyLight.range = baseKeyLightRange * num;
        backLight.range = baseBackLightRange * num;
        fillLight.range = baseFillLightRange * num;
      }
      if (GameCamera.Instance.CameraTransform != null)
      {
        GameCamera.Instance.CameraTransform.position = cameraPivotTransform.transform.position;
        GameCamera.Instance.CameraTransform.rotation = cameraPivotTransform.transform.rotation;
      }
      if (!(secondaryCameraObject != null))
        return;
      secondaryCameraObject.transform.localPosition = cameraPivotTransform.transform.position;
      secondaryCameraObject.transform.localRotation = cameraPivotTransform.transform.rotation;
      if (dof != null)
      {
        dof.focalTransform = component.DialogDOFTarget != null ? component.DialogDOFTarget.transform : null;
        dof.enabled = InstanceByRequest<GraphicsGameSettings>.Instance.DOF.Value && dof.focalTransform != null;
      }
      if (postProcessing != null)
        postProcessing.Antialiasing.Override = !InstanceByRequest<GraphicsGameSettings>.Instance.Antialiasing.Value;
    }

    private void InternalInitialise(GameObject gameObjectTarget)
    {
      unityRigInstance = Object.Instantiate(rigPrefab);
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
      if (gameObjectTarget != null)
      {
        foreach (Component componentsInChild in gameObjectTarget.GetComponentsInChildren<Transform>())
        {
          GameObject gameObject = componentsInChild.gameObject;
          storedLayers.Add(gameObject, gameObject.layer);
          gameObject.layer = ScriptableObjectInstance<GameSettingsData>.Instance.DialogLayer.GetIndex();
        }
      }
      GameObject dialogCameraPrefab = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogCameraPrefab;
      if (dialogCameraPrefab != null)
      {
        secondaryCameraObject = Object.Instantiate(dialogCameraPrefab);
        dof = secondaryCameraObject.GetComponent<DepthOfField>();
        postProcessing = secondaryCameraObject.GetComponent<PostProcessingStackOverride>();
        UnityEngine.Camera component = secondaryCameraObject.GetComponent<UnityEngine.Camera>();
        if ((bool) (Object) component)
          GameCamera.Instance.SetCutsceneFov(component.fieldOfView);
      }
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      if (!(camera != null))
        return;
      storedCullingMask = camera.cullingMask;
      camera.cullingMask &= ~ScriptableObjectInstance<GameSettingsData>.Instance.DialogLayer;
    }
  }
}
