using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Dynamic
{
  public class OwnHashInfoEqualityComparer : IEqualityComparer<OwnHashInfo>
  {
    public static readonly OwnHashInfoEqualityComparer Instance = new OwnHashInfoEqualityComparer();

    public bool Equals(OwnHashInfo x, OwnHashInfo y)
    {
      return x.OwnerId == y.OwnerId && (long) x.EventId == (long) y.EventId && x.SendingFSMGuid == y.SendingFSMGuid;
    }

    public int GetHashCode(OwnHashInfo obj)
    {
      Guid guid = obj.OwnerId;
      int num = guid.GetHashCode() ^ obj.EventId.GetHashCode();
      guid = obj.SendingFSMGuid;
      int hashCode = guid.GetHashCode();
      return num ^ hashCode;
    }
  }
}
