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
      if ((UnityEngine.Object) targetCamera == (UnityEngine.Object) null)
        return (GizmoRenderBehaviour) null;
      GameObject gameObject = new GameObject("[Camera] Debug", new System.Type[3]
      {
        typeof (UnityEngine.Camera),
        typeof (GizmoRenderBehaviour),
        typeof (DynamicResolutionCamera)
      });
      gameObject.transform.SetParent(targetCamera.transform, false);
      GizmoRenderBehaviour component = gameObject.GetComponent<GizmoRenderBehaviour>();
      component.camera = gameObject.GetComponent<UnityEngine.Camera>();
      component.targetCamera = targetCamera;
      component.UpdateCameraSettings();
      return component;
    }

    private void UpdateCameraSettings()
    {
      this.camera.CopyFrom(this.targetCamera);
      ++this.camera.depth;
      this.camera.cullingMask = 0;
      this.camera.clearFlags = CameraClearFlags.Nothing;
      this.camera.renderingPath = RenderingPath.Forward;
      this.camera.useOcclusionCulling = false;
      this.camera.eventMask = 0;
      this.camera.allowHDR = false;
    }

    private void OnEnable() => this.gizmoService = ServiceLocator.GetService<GizmoService>();

    private void OnDisable() => this.gizmoService.FireClear();

    private void OnPreCull() => this.UpdateCameraSettings();

    private void OnPostRender() => this.gizmoService.FireDrawShapes();

    public void OnGUI() => this.gizmoService.FireDrawTexts(this.camera);

    public void Update() => this.gizmoService.Check();
  }
}
