// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.CreateCollection`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Description("Create a collection of <T> objects")]
  public class CreateCollection<T> : VariableNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    public override void SetVariable(object o)
    {
    }

    protected override void RegisterPorts()
    {
      List<ValueInput<T>> ins = new List<ValueInput<T>>();
      for (int index = 0; index < this.portCount; ++index)
        ins.Add(this.AddValueInput<T>("Element" + index.ToString()));
      this.AddValueOutput<T[]>("Collection", (ValueHandler<T[]>) (() => ins.Select<ValueInput<T>, T>((Func<ValueInput<T>, T>) (p => p.value)).ToArray<T>()));
    }
  }
}
