// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.RendererOpacityFloatView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
      if ((Object) this.targetRenderer == (Object) null)
        return;
      Material sharedMaterial = this.targetRenderer.sharedMaterial;
      if ((Object) sharedMaterial == (Object) null)
        return;
      int id = Shader.PropertyToID(this.propertyName);
      if (!sharedMaterial.HasProperty(id))
        return;
      if (RendererOpacityFloatView.propertyBlock == null)
        RendererOpacityFloatView.propertyBlock = new MaterialPropertyBlock();
      Color color = sharedMaterial.GetColor(id);
      color.a *= this.FloatValue;
      RendererOpacityFloatView.propertyBlock.Clear();
      RendererOpacityFloatView.propertyBlock.SetColor(id, color);
      this.targetRenderer.SetPropertyBlock(RendererOpacityFloatView.propertyBlock);
    }
  }
}
