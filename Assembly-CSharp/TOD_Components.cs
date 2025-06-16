// Decompiled with JetBrains decompiler
// Type: TOD_Components
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class TOD_Components : MonoBehaviour
{
  public GameObject Space = (GameObject) null;
  public GameObject Stars = (GameObject) null;
  public GameObject Sun = (GameObject) null;
  public GameObject Moon = (GameObject) null;
  public GameObject Atmosphere = (GameObject) null;
  public GameObject Clear = (GameObject) null;
  public GameObject Clouds = (GameObject) null;
  public GameObject Billboards = (GameObject) null;
  public GameObject Light = (GameObject) null;

  public Transform DomeTransform { get; set; }

  public Transform SpaceTransform { get; set; }

  public Transform StarTransform { get; set; }

  public Transform SunTransform { get; set; }

  public Transform MoonTransform { get; set; }

  public Transform AtmosphereTransform { get; set; }

  public Transform ClearTransform { get; set; }

  public Transform CloudTransform { get; set; }

  public Transform BillboardTransform { get; set; }

  public Transform LightTransform { get; set; }

  public Renderer SpaceRenderer { get; set; }

  public Renderer StarRenderer { get; set; }

  public Renderer SunRenderer { get; set; }

  public Renderer MoonRenderer { get; set; }

  public Renderer AtmosphereRenderer { get; set; }

  public Renderer ClearRenderer { get; set; }

  public Renderer CloudRenderer { get; set; }

  public Renderer[] BillboardRenderers { get; set; }

  public MeshFilter SpaceMeshFilter { get; set; }

  public MeshFilter StarMeshFilter { get; set; }

  public MeshFilter SunMeshFilter { get; set; }

  public MeshFilter MoonMeshFilter { get; set; }

  public MeshFilter AtmosphereMeshFilter { get; set; }

  public MeshFilter ClearMeshFilter { get; set; }

  public MeshFilter CloudMeshFilter { get; set; }

  public MeshFilter[] BillboardMeshFilters { get; set; }

  public Material SpaceMaterial { get; set; }

  public Material StarMaterial { get; set; }

  public Material SunMaterial { get; set; }

  public Material MoonMaterial { get; set; }

  public Material AtmosphereMaterial { get; set; }

  public Material ClearMaterial { get; set; }

  public Material CloudMaterial { get; set; }

  public Material[] BillboardMaterials { get; set; }

  public UnityEngine.Light LightSource { get; set; }

  public TOD_Sky Sky { get; set; }

  public TOD_Animation Animation { get; set; }

  public TOD_Time Time { get; set; }

  public TOD_Camera Camera { get; set; }

  public TOD_Rays Rays { get; set; }

  public TOD_Scattering Scattering { get; set; }

  public TOD_Shadows Shadows { get; set; }

  public void Initialize()
  {
    this.DomeTransform = this.GetComponent<Transform>();
    this.Sky = this.GetComponent<TOD_Sky>();
    this.Animation = this.GetComponent<TOD_Animation>();
    this.Time = this.GetComponent<TOD_Time>();
    if ((bool) (Object) this.Space)
    {
      this.SpaceTransform = this.Space.GetComponent<Transform>();
      this.SpaceRenderer = this.Space.GetComponent<Renderer>();
      this.SpaceMeshFilter = this.Space.GetComponent<MeshFilter>();
      this.SpaceMaterial = this.SpaceRenderer.sharedMaterial;
    }
    if ((bool) (Object) this.Stars)
    {
      this.StarTransform = this.Stars.GetComponent<Transform>();
      this.StarRenderer = this.Stars.GetComponent<Renderer>();
      this.StarMeshFilter = this.Stars.GetComponent<MeshFilter>();
      this.StarMaterial = this.StarRenderer.sharedMaterial;
    }
    if ((bool) (Object) this.Sun)
    {
      this.SunTransform = this.Sun.GetComponent<Transform>();
      this.SunRenderer = this.Sun.GetComponent<Renderer>();
      this.SunMeshFilter = this.Sun.GetComponent<MeshFilter>();
      this.SunMaterial = this.SunRenderer.sharedMaterial;
    }
    if ((bool) (Object) this.Moon)
    {
      this.MoonTransform = this.Moon.GetComponent<Transform>();
      this.MoonRenderer = this.Moon.GetComponent<Renderer>();
      this.MoonMeshFilter = this.Moon.GetComponent<MeshFilter>();
      this.MoonMaterial = this.MoonRenderer.sharedMaterial;
    }
    if ((bool) (Object) this.Atmosphere)
    {
      this.AtmosphereTransform = this.Atmosphere.GetComponent<Transform>();
      this.AtmosphereRenderer = this.Atmosphere.GetComponent<Renderer>();
      this.AtmosphereMeshFilter = this.Atmosphere.GetComponent<MeshFilter>();
      this.AtmosphereMaterial = this.AtmosphereRenderer.sharedMaterial;
    }
    if ((bool) (Object) this.Clear)
    {
      this.ClearTransform = this.Clear.GetComponent<Transform>();
      this.ClearRenderer = this.Clear.GetComponent<Renderer>();
      this.ClearMeshFilter = this.Clear.GetComponent<MeshFilter>();
      this.ClearMaterial = this.ClearRenderer.sharedMaterial;
    }
    if ((bool) (Object) this.Clouds)
    {
      this.CloudTransform = this.Clouds.GetComponent<Transform>();
      this.CloudRenderer = this.Clouds.GetComponent<Renderer>();
      this.CloudMeshFilter = this.Clouds.GetComponent<MeshFilter>();
      this.CloudMaterial = this.CloudRenderer.sharedMaterial;
    }
    if ((bool) (Object) this.Billboards)
    {
      this.BillboardTransform = this.Billboards.GetComponent<Transform>();
      this.BillboardRenderers = this.Billboards.GetComponentsInChildren<Renderer>();
      this.BillboardMeshFilters = this.Billboards.GetComponentsInChildren<MeshFilter>();
      this.BillboardMaterials = new Material[this.BillboardRenderers.Length];
      for (int index = 0; index < this.BillboardRenderers.Length; ++index)
        this.BillboardMaterials[index] = this.BillboardRenderers[index].sharedMaterial;
    }
    if (!(bool) (Object) this.Light)
      return;
    this.LightTransform = this.Light.GetComponent<Transform>();
    this.LightSource = this.Light.GetComponent<UnityEngine.Light>();
  }
}
