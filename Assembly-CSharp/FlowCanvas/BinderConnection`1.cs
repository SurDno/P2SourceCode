// Decompiled with JetBrains decompiler
// Type: FlowCanvas.BinderConnection`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace FlowCanvas
{
  public class BinderConnection<T> : BinderConnection
  {
    public override void Bind()
    {
      if (!this.isActive)
        return;
      this.DoNormalBinding(this.sourcePort, this.targetPort);
    }

    public override void UnBind()
    {
      if (!(this.targetPort is ValueInput))
        return;
      (this.targetPort as ValueInput).UnBind();
    }

    private void DoNormalBinding(Port source, Port target)
    {
      (target as ValueInput<T>).BindTo((ValueOutput) source);
    }
  }
}
