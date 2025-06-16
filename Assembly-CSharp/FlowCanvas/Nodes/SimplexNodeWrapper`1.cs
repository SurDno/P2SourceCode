// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SimplexNodeWrapper`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [DoNotList]
  public class SimplexNodeWrapper<T> : FlowNode where T : SimplexNode
  {
    private T _simplexNode;

    private T simplexNode
    {
      get
      {
        if ((object) this._simplexNode == null)
        {
          this._simplexNode = (T) Activator.CreateInstance(typeof (T));
          if ((object) this._simplexNode != null)
            this.GatherPorts();
        }
        return this._simplexNode;
      }
    }

    public override string name
    {
      get => (object) this.simplexNode != null ? this.simplexNode.name : "NULL";
    }

    public override string description
    {
      get => (object) this.simplexNode != null ? this.simplexNode.description : "NULL";
    }

    public override void OnGraphStarted()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.OnGraphStarted();
    }

    public override void OnGraphPaused()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.OnGraphPaused();
    }

    public override void OnGraphUnpaused()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.OnGraphUnpaused();
    }

    public override void OnGraphStoped()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.OnGraphStoped();
    }

    protected override void RegisterPorts()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.RegisterPorts((FlowNode) this);
    }
  }
}
