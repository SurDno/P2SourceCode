using Engine.Common.Binders;
using System.ComponentModel;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [EnumType("GateState")]
  public enum GateState
  {
    [Description("Closed")] Closed,
    [Description("Opened")] Opened,
  }
}
