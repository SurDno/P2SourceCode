using UnityEngine;
using UnityEngine.Serialization;

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
  private Material effectMaterial;
  [SerializeField]
  [FormerlySerializedAs("IsAlive")]
  private bool isAlive = true;
  [Range(0.0f, 1f)]
  public float SmokeLevel;
  [Range(0.0f, 1f)]
  public float FireLevel;
  private SkinnedMeshRenderer skinnedMeshRenderer;
  private Mesh bakedMesh;
  private MeshFilter meshFilter;
  private MeshRenderer meshRenderer;
  private Texture[] alphaTextures;
  private Vector4[] alphaOffsetScales;

  private void CollectAlphaTextures(Renderer renderer)
  {
    Material[] sharedMaterials = renderer.sharedMaterials;
    alphaTextures = new Texture2D[sharedMaterials.Length];
    alphaOffsetScales = new Vector4[sharedMaterials.Length];
    for (int index = 0; index < sharedMaterials.Length; ++index)
    {
      Material material = sharedMaterials[index];
      if (!(material == null))
      {
        Shader shader = material.shader;
        if (!(shader == null) && shader.name.Contains("Cutout"))
        {
          alphaTextures[index] = material.mainTexture;
          Vector2 mainTextureScale = material.mainTextureScale;
          Vector2 mainTextureOffset = material.mainTextureOffset;
          alphaOffsetScales[index] = new Vector4(mainTextureScale.x, mainTextureScale.y, mainTextureOffset.x, mainTextureOffset.y);
        }
      }
    }
  }

  private void DrawMesh(Mesh mesh, int subMeshIndex)
  {
    Vector4 vector4 = new Vector4(1f, 1f, 0.0f, 0.0f);
    Texture texture;
    if (subMeshIndex < alphaTextures.Length)
    {
      texture = alphaTextures[subMeshIndex];
      if (texture == null)
        texture = Texture2D.whiteTexture;
      else
        vector4 = alphaOffsetScales[subMeshIndex];
    }
    else
      texture = Texture2D.whiteTexture;
    propertyBlock.SetTexture(alphaTexPropertyId, texture);
    propertyBlock.SetVector(alphaOffsetScalePropertyId, vector4);
    Graphics.DrawMesh(mesh, transform.localToWorldMatrix, effectMaterial, gameObject.layer, null, subMeshIndex, propertyBlock);
  }

  private void LateUpdate()
  {
    if (effectMaterial == null || SmokeLevel <= 0.0)
      return;
    propertyBlock.SetFloat(smokePropertyId, SmokeLevel);
    propertyBlock.SetFloat(firePropertyId, FireLevel);
    if (skinnedMeshRenderer != null)
    {
      if (!skinnedMeshRenderer.isVisible || !(skinnedMeshRenderer.sharedMesh != null))
        return;
      if (bakedMesh == null)
      {
        bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);
      }
      else if (isAlive)
        skinnedMeshRenderer.BakeMesh(bakedMesh);
      for (int subMeshIndex = 0; subMeshIndex < bakedMesh.subMeshCount; ++subMeshIndex)
        DrawMesh(bakedMesh, subMeshIndex);
    }
    else
    {
      if (!(meshFilter != null) || !(meshRenderer != null) || !meshRenderer.isVisible)
        return;
      Mesh sharedMesh = meshFilter.sharedMesh;
      if (sharedMesh != null)
      {
        for (int subMeshIndex = 0; subMeshIndex < sharedMesh.subMeshCount; ++subMeshIndex)
          DrawMesh(meshFilter.sharedMesh, subMeshIndex);
      }
    }
  }

  private void OnDisable()
  {
    skinnedMeshRenderer = null;
    meshFilter = null;
    meshRenderer = null;
    alphaTextures = null;
    if (!(bakedMesh != null))
      return;
    Destroy(bakedMesh);
    bakedMesh = null;
  }

  private void OnEnable()
  {
    if (effectMaterial == null)
      effectMaterial = ScriptableObjectInstance<ResourceFromCodeData>.Instance.BurnedEffect;
    if (effectMaterial == null)
      return;
    if (propertyBlock == null)
    {
      propertyBlock = new MaterialPropertyBlock();
      smokePropertyId = Shader.PropertyToID("_SmokeLevel");
      firePropertyId = Shader.PropertyToID("_FireLevel");
      alphaTexPropertyId = Shader.PropertyToID("_AlphaTex");
      alphaOffsetScalePropertyId = Shader.PropertyToID("_AlphaTex_ST");
    }
    skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    if (skinnedMeshRenderer != null)
    {
      CollectAlphaTextures(skinnedMeshRenderer);
    }
    else
    {
      meshFilter = GetComponent<MeshFilter>();
      meshRenderer = GetComponent<MeshRenderer>();
      if (meshRenderer != null)
        CollectAlphaTextures(meshRenderer);
    }
  }
}
