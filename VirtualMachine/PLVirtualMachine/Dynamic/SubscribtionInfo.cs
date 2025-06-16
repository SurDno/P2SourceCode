// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.SubscribtionInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class SubscribtionInfo
  {
    public SubscribtionInfo(DynamicFSM fsm)
    {
      this.SubscribingFSM = fsm;
      this.Count = 1;
    }

    public void Add() => ++this.Count;

    public void Remove() => --this.Count;

    public DynamicFSM SubscribingFSM { get; private set; }

    public int Count { get; private set; }
  }
}
