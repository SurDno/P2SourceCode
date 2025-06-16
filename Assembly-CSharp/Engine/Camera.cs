// Decompiled with JetBrains decompiler
// Type: Engine.Camera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine
{
  [ExecuteInEditMode]
  public class Camera : MonoBehaviour
  {
    public event Action<UnityEngine.Camera, RenderTexture, RenderTexture> RenderEvent;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      Action<UnityEngine.Camera, RenderTexture, RenderTexture> renderEvent = this.RenderEvent;
      if (renderEvent == null)
        return;
      renderEvent(this.gameObject.GetComponent<UnityEngine.Camera>(), source, destination);
    }
  }
}
