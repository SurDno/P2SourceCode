using PLVirtualMachine.Common;
using PLVirtualMachine.Objects;
using System.Collections.Generic;

namespace VirtualMachine.Data
{
  public class GameDataInfo
  {
    public string Name;
    public Dictionary<ulong, IObject> Objects;
    public VMGameRoot Root;
  }
}
