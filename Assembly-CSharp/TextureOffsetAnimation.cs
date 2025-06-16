[RequireComponent(typeof (Renderer))]
public class TextureOffsetAnimation : MonoBehaviour
{
  [SerializeField]
  private string propertyName;
  [SerializeField]
  private Vector2 velocity;
  private Renderer renderer;
  private int propertyId;
  private MaterialPropertyBlock propertyBlock;
  private Vector4 tilingOffset;

  private void Start()
  {
    renderer = this.GetComponent<Renderer>();
    propertyId = Shader.PropertyToID(propertyName + "_ST");
    Material sharedMaterial = renderer.sharedMaterial;
    if ((Object) sharedMaterial != (Object) null)
      tilingOffset = sharedMaterial.GetVector(propertyId);
    propertyBlock = new MaterialPropertyBlock();
  }

  private void Update()
  {
    float deltaTime = Time.deltaTime;
    tilingOffset.z = Mathf.Repeat(tilingOffset.z + velocity.x * deltaTime, 1f);
    tilingOffset.w = Mathf.Repeat(tilingOffset.w + velocity.y * deltaTime, 1f);
    propertyBlock.SetVector(propertyId, tilingOffset);
    renderer.SetPropertyBlock(propertyBlock);
  }
}
