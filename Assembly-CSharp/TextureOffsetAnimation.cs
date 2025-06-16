using UnityEngine;

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
    this.renderer = this.GetComponent<Renderer>();
    this.propertyId = Shader.PropertyToID(this.propertyName + "_ST");
    Material sharedMaterial = this.renderer.sharedMaterial;
    if ((Object) sharedMaterial != (Object) null)
      this.tilingOffset = sharedMaterial.GetVector(this.propertyId);
    this.propertyBlock = new MaterialPropertyBlock();
  }

  private void Update()
  {
    float deltaTime = Time.deltaTime;
    this.tilingOffset.z = Mathf.Repeat(this.tilingOffset.z + this.velocity.x * deltaTime, 1f);
    this.tilingOffset.w = Mathf.Repeat(this.tilingOffset.w + this.velocity.y * deltaTime, 1f);
    this.propertyBlock.SetVector(this.propertyId, this.tilingOffset);
    this.renderer.SetPropertyBlock(this.propertyBlock);
  }
}
