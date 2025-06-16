// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.PostProcessingComponent`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace UnityEngine.PostProcessing
{
  public abstract class PostProcessingComponent<T> : PostProcessingComponentBase where T : PostProcessingModel
  {
    public T model { get; internal set; }

    public virtual void Init(PostProcessingContext pcontext, T pmodel)
    {
      this.context = pcontext;
      this.model = pmodel;
    }

    public override PostProcessingModel GetModel() => (PostProcessingModel) this.model;
  }
}
