// Decompiled with JetBrains decompiler
// Type: FlowCanvas.FlowInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FlowCanvas
{
  public class FlowInput : Port
  {
    public FlowInput(FlowNode parent, string name, string ID, FlowHandler pointer)
      : base(parent, name, ID)
    {
      this.pointer = pointer;
    }

    public FlowHandler pointer { get; private set; }

    public override Type type => typeof (void);
  }
}
