using Engine.Common.Services;
using UnityEngine;

namespace Engine.Source.Services.Gizmos
{
  public class GizmoRenderBehaviour : MonoBehaviour
  {
    private UnityEngine.Camera camera;
    private UnityEngine.Camera targetCamera;
    private GizmoService gizmoService;

    public static GizmoRenderBehaviour Create(UnityEngine.Camera targetCamera)
    {
      if (targetCamera == null)
        return null;
      GameObject gameObject = new GameObject("[Camera] Debug", typeof (UnityEngine.Camera), typeof (GizmoRenderBehaviour), typeof (DynamicResolutionCamera));
      gameObject.transform.SetParent(targetCamera.transform, false);
      GizmoRenderBehaviour component = gameObject.GetComponent<GizmoRenderBehaviour>();
      component.camera = gameObject.GetComponent<UnityEngine.Camera>();
      component.targetCamera = targetCamera;
      component.UpdateCameraSettings();
      return component;
    }

    private void UpdateCameraSettings()
    {
      camera.CopyFrom(targetCamera);
      ++camera.depth;
      camera.cullingMask = 0;
      camera.clearFlags = CameraClearFlags.Nothing;
      camera.renderingPath = RenderingPath.Forward;
      camera.useOcclusionCulling = false;
      camera.eventMask = 0;
      camera.allowHDR = false;
    }

    private void OnEnable() => gizmoService = ServiceLocator.GetService<GizmoService>();

    private void OnDisable() => gizmoService.FireClear();

    private void OnPreCull() => UpdateCameraSettings();

    private void OnPostRender() => gizmoService.FireDrawShapes();

    public void OnGUI() => gizmoService.FireDrawTexts(camera);

    public void Update() => gizmoService.Check();
  }
}
