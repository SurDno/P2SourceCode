using System.Collections.Generic;
using PLVirtualMachine.Common;
using PLVirtualMachine.Objects;

namespace VirtualMachine.Data
{
  public class GameDataInfo
  {
    public string Name;
    public Dictionary<ulong, IObject> Objects;
    public VMGameRoot Root;
  }
}
