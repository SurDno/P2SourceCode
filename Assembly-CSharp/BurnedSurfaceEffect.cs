// Decompiled with JetBrains decompiler
// Type: BurnedSurfaceEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
[ExecuteInEditMode]
public class BurnedSurfaceEffect : MonoBehaviour
{
  private static MaterialPropertyBlock propertyBlock;
  private static int smokePropertyId;
  private static int firePropertyId;
  private static int alphaTexPropertyId;
  private static int alphaOffsetScalePropertyId;
  [SerializeField]
  [FormerlySerializedAs("EffectMaterial")]
  private Material effectMaterial = (Material) null;
  [SerializeField]
  [FormerlySerializedAs("IsAlive")]
  private bool isAlive = true;
  [Range(0.0f, 1f)]
  public float SmokeLevel = 0.0f;
  [Range(0.0f, 1f)]
  public float FireLevel = 0.0f;
  private SkinnedMeshRenderer skinnedMeshRenderer;
  private Mesh bakedMesh;
  private MeshFilter meshFilter;
  private MeshRenderer meshRenderer;
  private Texture[] alphaTextures;
  private Vector4[] alphaOffsetScales;

  private void CollectAlphaTextures(Renderer renderer)
  {
    Material[] sharedMaterials = renderer.sharedMaterials;
    this.alphaTextures = (Texture[]) new Texture2D[sharedMaterials.Length];
    this.alphaOffsetScales = new Vector4[sharedMaterials.Length];
    for (int index = 0; index < sharedMaterials.Length; ++index)
    {
      Material material = sharedMaterials[index];
      if (!((Object) material == (Object) null))
      {
        Shader shader = material.shader;
        if (!((Object) shader == (Object) null) && shader.name.Contains("Cutout"))
        {
          this.alphaTextures[index] = material.mainTexture;
          Vector2 mainTextureScale = material.mainTextureScale;
          Vector2 mainTextureOffset = material.mainTextureOffset;
          this.alphaOffsetScales[index] = new Vector4(mainTextureScale.x, mainTextureScale.y, mainTextureOffset.x, mainTextureOffset.y);
        }
      }
    }
  }

  private void DrawMesh(Mesh mesh, int subMeshIndex)
  {
    Vector4 vector4 = new Vector4(1f, 1f, 0.0f, 0.0f);
    Texture texture;
    if (subMeshIndex < this.alphaTextures.Length)
    {
      texture = this.alphaTextures[subMeshIndex];
      if ((Object) texture == (Object) null)
        texture = (Texture) Texture2D.whiteTexture;
      else
        vector4 = this.alphaOffsetScales[subMeshIndex];
    }
    else
      texture = (Texture) Texture2D.whiteTexture;
    BurnedSurfaceEffect.propertyBlock.SetTexture(BurnedSurfaceEffect.alphaTexPropertyId, texture);
    BurnedSurfaceEffect.propertyBlock.SetVector(BurnedSurfaceEffect.alphaOffsetScalePropertyId, vector4);
    Graphics.DrawMesh(mesh, this.transform.localToWorldMatrix, this.effectMaterial, this.gameObject.layer, (Camera) null, subMeshIndex, BurnedSurfaceEffect.propertyBlock);
  }

  private void LateUpdate()
  {
    if ((Object) this.effectMaterial == (Object) null || (double) this.SmokeLevel <= 0.0)
      return;
    BurnedSurfaceEffect.propertyBlock.SetFloat(BurnedSurfaceEffect.smokePropertyId, this.SmokeLevel);
    BurnedSurfaceEffect.propertyBlock.SetFloat(BurnedSurfaceEffect.firePropertyId, this.FireLevel);
    if ((Object) this.skinnedMeshRenderer != (Object) null)
    {
      if (!this.skinnedMeshRenderer.isVisible || !((Object) this.skinnedMeshRenderer.sharedMesh != (Object) null))
        return;
      if ((Object) this.bakedMesh == (Object) null)
      {
        this.bakedMesh = new Mesh();
        this.skinnedMeshRenderer.BakeMesh(this.bakedMesh);
      }
      else if (this.isAlive)
        this.skinnedMeshRenderer.BakeMesh(this.bakedMesh);
      for (int subMeshIndex = 0; subMeshIndex < this.bakedMesh.subMeshCount; ++subMeshIndex)
        this.DrawMesh(this.bakedMesh, subMeshIndex);
    }
    else
    {
      if (!((Object) this.meshFilter != (Object) null) || !((Object) this.meshRenderer != (Object) null) || !this.meshRenderer.isVisible)
        return;
      Mesh sharedMesh = this.meshFilter.sharedMesh;
      if ((Object) sharedMesh != (Object) null)
      {
        for (int subMeshIndex = 0; subMeshIndex < sharedMesh.subMeshCount; ++subMeshIndex)
          this.DrawMesh(this.meshFilter.sharedMesh, subMeshIndex);
      }
    }
  }

  private void OnDisable()
  {
    this.skinnedMeshRenderer = (SkinnedMeshRenderer) null;
    this.meshFilter = (MeshFilter) null;
    this.meshRenderer = (MeshRenderer) null;
    this.alphaTextures = (Texture[]) null;
    if (!((Object) this.bakedMesh != (Object) null))
      return;
    Object.Destroy((Object) this.bakedMesh);
    this.bakedMesh = (Mesh) null;
  }

  private void OnEnable()
  {
    if ((Object) this.effectMaterial == (Object) null)
      this.effectMaterial = ScriptableObjectInstance<ResourceFromCodeData>.Instance.BurnedEffect;
    if ((Object) this.effectMaterial == (Object) null)
      return;
    if (BurnedSurfaceEffect.propertyBlock == null)
    {
      BurnedSurfaceEffect.propertyBlock = new MaterialPropertyBlock();
      BurnedSurfaceEffect.smokePropertyId = Shader.PropertyToID("_SmokeLevel");
      BurnedSurfaceEffect.firePropertyId = Shader.PropertyToID("_FireLevel");
      BurnedSurfaceEffect.alphaTexPropertyId = Shader.PropertyToID("_AlphaTex");
      BurnedSurfaceEffect.alphaOffsetScalePropertyId = Shader.PropertyToID("_AlphaTex_ST");
    }
    this.skinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>();
    if ((Object) this.skinnedMeshRenderer != (Object) null)
    {
      this.CollectAlphaTextures((Renderer) this.skinnedMeshRenderer);
    }
    else
    {
      this.meshFilter = this.GetComponent<MeshFilter>();
      this.meshRenderer = this.GetComponent<MeshRenderer>();
      if ((Object) this.meshRenderer != (Object) null)
        this.CollectAlphaTextures((Renderer) this.meshRenderer);
    }
  }
}
