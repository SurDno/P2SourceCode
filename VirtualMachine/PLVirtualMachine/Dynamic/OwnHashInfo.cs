// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.OwnHashInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using System;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public struct OwnHashInfo
  {
    public static readonly OwnHashInfo Empty = new OwnHashInfo(Guid.Empty, 0UL, Guid.Empty);
    public readonly Guid OwnerId;
    public readonly ulong EventId;
    public readonly Guid SendingFSMGuid;

    public OwnHashInfo(Guid ownerId, ulong eventId, Guid sendingFSMGuid)
    {
      this.OwnerId = ownerId;
      this.EventId = eventId;
      this.SendingFSMGuid = sendingFSMGuid;
    }
  }
}
