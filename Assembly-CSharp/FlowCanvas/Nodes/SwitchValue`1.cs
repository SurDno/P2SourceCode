// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SwitchValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  [Description("Returns either one of the two inputs, based on the boolean condition")]
  public class SwitchValue<T> : PureFunctionNode<T, bool, T, T>
  {
    public override T Invoke(bool condition, T isTrue, T isFalse) => condition ? isTrue : isFalse;
  }
}
