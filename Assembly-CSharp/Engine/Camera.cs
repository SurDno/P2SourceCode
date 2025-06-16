using System;
using UnityEngine;

namespace Engine
{
  [ExecuteInEditMode]
  public class Camera : MonoBehaviour
  {
    public event Action<UnityEngine.Camera, RenderTexture, RenderTexture> RenderEvent;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      Action<UnityEngine.Camera, RenderTexture, RenderTexture> renderEvent = RenderEvent;
      if (renderEvent == null)
        return;
      renderEvent(gameObject.GetComponent<UnityEngine.Camera>(), source, destination);
    }
  }
}
