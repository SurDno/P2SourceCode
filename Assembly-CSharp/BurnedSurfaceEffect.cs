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
    alphaTextures = (Texture[]) new Texture2D[sharedMaterials.Length];
    alphaOffsetScales = new Vector4[sharedMaterials.Length];
    for (int index = 0; index < sharedMaterials.Length; ++index)
    {
      Material material = sharedMaterials[index];
      if (!((Object) material == (Object) null))
      {
        Shader shader = material.shader;
        if (!((Object) shader == (Object) null) && shader.name.Contains("Cutout"))
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
      if ((Object) texture == (Object) null)
        texture = (Texture) Texture2D.whiteTexture;
      else
        vector4 = alphaOffsetScales[subMeshIndex];
    }
    else
      texture = (Texture) Texture2D.whiteTexture;
    propertyBlock.SetTexture(alphaTexPropertyId, texture);
    propertyBlock.SetVector(alphaOffsetScalePropertyId, vector4);
    Graphics.DrawMesh(mesh, this.transform.localToWorldMatrix, effectMaterial, this.gameObject.layer, (Camera) null, subMeshIndex, propertyBlock);
  }

  private void LateUpdate()
  {
    if ((Object) effectMaterial == (Object) null || SmokeLevel <= 0.0)
      return;
    propertyBlock.SetFloat(smokePropertyId, SmokeLevel);
    propertyBlock.SetFloat(firePropertyId, FireLevel);
    if ((Object) skinnedMeshRenderer != (Object) null)
    {
      if (!skinnedMeshRenderer.isVisible || !((Object) skinnedMeshRenderer.sharedMesh != (Object) null))
        return;
      if ((Object) bakedMesh == (Object) null)
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
      if (!((Object) meshFilter != (Object) null) || !((Object) meshRenderer != (Object) null) || !meshRenderer.isVisible)
        return;
      Mesh sharedMesh = meshFilter.sharedMesh;
      if ((Object) sharedMesh != (Object) null)
      {
        for (int subMeshIndex = 0; subMeshIndex < sharedMesh.subMeshCount; ++subMeshIndex)
          DrawMesh(meshFilter.sharedMesh, subMeshIndex);
      }
    }
  }

  private void OnDisable()
  {
    skinnedMeshRenderer = (SkinnedMeshRenderer) null;
    meshFilter = (MeshFilter) null;
    meshRenderer = (MeshRenderer) null;
    alphaTextures = (Texture[]) null;
    if (!((Object) bakedMesh != (Object) null))
      return;
    Object.Destroy((Object) bakedMesh);
    bakedMesh = (Mesh) null;
  }

  private void OnEnable()
  {
    if ((Object) effectMaterial == (Object) null)
      effectMaterial = ScriptableObjectInstance<ResourceFromCodeData>.Instance.BurnedEffect;
    if ((Object) effectMaterial == (Object) null)
      return;
    if (propertyBlock == null)
    {
      propertyBlock = new MaterialPropertyBlock();
      smokePropertyId = Shader.PropertyToID("_SmokeLevel");
      firePropertyId = Shader.PropertyToID("_FireLevel");
      alphaTexPropertyId = Shader.PropertyToID("_AlphaTex");
      alphaOffsetScalePropertyId = Shader.PropertyToID("_AlphaTex_ST");
    }
    skinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>();
    if ((Object) skinnedMeshRenderer != (Object) null)
    {
      CollectAlphaTextures((Renderer) skinnedMeshRenderer);
    }
    else
    {
      meshFilter = this.GetComponent<MeshFilter>();
      meshRenderer = this.GetComponent<MeshRenderer>();
      if ((Object) meshRenderer != (Object) null)
        CollectAlphaTextures((Renderer) meshRenderer);
    }
  }
}
