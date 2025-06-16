// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.GetOtherVariable`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Get Other Of Type<T>")]
  [Category("Variables/Blackboard")]
  [Description("Use this to get a variable value from blackboards other than the one this flowscript is using")]
  [AppendListTypes]
  public class GetOtherVariable<T> : VariableNode
  {
    public override string name => "Get Variable";

    protected override void RegisterPorts()
    {
      ValueInput<Blackboard> bb = this.AddValueInput<Blackboard>("Blackboard");
      ValueInput<string> varName = this.AddValueInput<string>("Variable");
      this.AddValueOutput<T>("Value", (ValueHandler<T>) (() => bb.value.GetValue<T>(varName.value)));
    }

    public override void SetVariable(object o)
    {
    }
  }
}
