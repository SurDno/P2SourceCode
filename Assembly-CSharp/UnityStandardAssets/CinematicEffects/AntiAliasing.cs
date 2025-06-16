// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CinematicEffects.AntiAliasing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Anti-aliasing")]
  [ImageEffectAllowedInSceneView]
  public class AntiAliasing : MonoBehaviour
  {
    [SerializeField]
    private SMAA m_SMAA = new SMAA();
    [SerializeField]
    private FXAA m_FXAA = new FXAA();
    [SerializeField]
    [HideInInspector]
    private int m_Method = 0;
    private Camera m_Camera;

    public int method
    {
      get => this.m_Method;
      set
      {
        if (this.m_Method == value)
          return;
        this.m_Method = value;
      }
    }

    public IAntiAliasing current
    {
      get => this.method == 0 ? (IAntiAliasing) this.m_SMAA : (IAntiAliasing) this.m_FXAA;
    }

    public Camera cameraComponent
    {
      get
      {
        if ((Object) this.m_Camera == (Object) null)
          this.m_Camera = this.GetComponent<Camera>();
        return this.m_Camera;
      }
    }

    private void OnEnable()
    {
      this.m_SMAA.OnEnable(this);
      this.m_FXAA.OnEnable(this);
    }

    private void OnDisable()
    {
      this.m_SMAA.OnDisable();
      this.m_FXAA.OnDisable();
    }

    private void OnPreCull() => this.current.OnPreCull(this.cameraComponent);

    private void OnPostRender() => this.current.OnPostRender(this.cameraComponent);

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.current.OnRenderImage(this.cameraComponent, source, destination);
    }

    public enum Method
    {
      Smaa,
      Fxaa,
    }
  }
}
