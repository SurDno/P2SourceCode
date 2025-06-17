using System;

namespace PLVirtualMachine.Dynamic
{
  public struct OwnHashInfo(Guid ownerId, ulong eventId, Guid sendingFsmGuid) {
    public static readonly OwnHashInfo Empty = new(Guid.Empty, 0UL, Guid.Empty);
    public readonly Guid OwnerId = ownerId;
    public readonly ulong EventId = eventId;
    public readonly Guid SendingFSMGuid = sendingFsmGuid;
  }
}
