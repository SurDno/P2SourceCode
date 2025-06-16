using System;

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
      OwnerId = ownerId;
      EventId = eventId;
      SendingFSMGuid = sendingFSMGuid;
    }
  }
}
