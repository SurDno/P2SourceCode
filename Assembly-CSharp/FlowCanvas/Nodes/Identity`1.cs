// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.Identity`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Identity Value<T>")]
  [Category("Functions/Utility")]
  [Description("Use this for organization. It returns exactly what is provided in the input.")]
  public class Identity<T> : PureFunctionNode<T, T>
  {
    public override string name => (string) null;

    public override T Invoke(T value) => value;
  }
}
