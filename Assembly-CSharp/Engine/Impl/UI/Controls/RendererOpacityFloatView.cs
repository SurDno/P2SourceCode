namespace Engine.Impl.UI.Controls
{
  public class RendererOpacityFloatView : FloatViewBase
  {
    private static MaterialPropertyBlock propertyBlock;
    private static int propertyId;
    [SerializeField]
    private Renderer targetRenderer;
    [SerializeField]
    private string propertyName = "_Color";

    public override void SkipAnimation()
    {
    }

    protected override void ApplyFloatValue()
    {
      if ((Object) targetRenderer == (Object) null)
        return;
      Material sharedMaterial = targetRenderer.sharedMaterial;
      if ((Object) sharedMaterial == (Object) null)
        return;
      int id = Shader.PropertyToID(propertyName);
      if (!sharedMaterial.HasProperty(id))
        return;
      if (propertyBlock == null)
        propertyBlock = new MaterialPropertyBlock();
      Color color = sharedMaterial.GetColor(id);
      color.a *= FloatValue;
      propertyBlock.Clear();
      propertyBlock.SetColor(id, color);
      targetRenderer.SetPropertyBlock(propertyBlock);
    }
  }
}
