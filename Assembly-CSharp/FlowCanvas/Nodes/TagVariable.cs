// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.TagVariable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [DoNotList]
  [Name("Easy Tag")]
  [Description("An easy way to get a Tag name")]
  public class TagVariable : VariableNode
  {
    [TagField]
    public string value = "Untagged";

    protected override void RegisterPorts()
    {
      this.AddValueOutput<string>("Tag", (ValueHandler<string>) (() => this.value));
    }

    public override void SetVariable(object o)
    {
      if (!(o is string))
        return;
      this.value = (string) o;
    }
  }
}
