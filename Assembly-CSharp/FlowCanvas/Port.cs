// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Port
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas
{
  [SpoofAOT]
  public abstract class Port
  {
    public Port()
    {
    }

    public Port(FlowNode parent, string name, string id)
    {
      this.parent = parent;
      this.name = name;
      this.id = id;
    }

    public FlowNode parent { get; private set; }

    public string id { get; set; }

    public string name { get; set; }

    public Vector2 pos { get; set; }

    public int connections { get; set; }

    public bool isConnected => this.connections > 0;

    public abstract System.Type type { get; }

    public bool CanAcceptConnections()
    {
      int num1;
      switch (this)
      {
        case ValueOutput _:
          num1 = 1;
          break;
        case FlowOutput _:
          num1 = !this.isConnected ? 1 : 0;
          break;
        default:
          num1 = 0;
          break;
      }
      if (num1 != 0)
        return true;
      int num2;
      switch (this)
      {
        case FlowInput _:
          num2 = 1;
          break;
        case ValueInput _:
          num2 = !this.isConnected ? 1 : 0;
          break;
        default:
          num2 = 0;
          break;
      }
      return num2 != 0;
    }
  }
}
