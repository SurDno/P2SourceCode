using System;

namespace Inspectors
{
  [Flags]
  public enum ExecuteMode
  {
    None = 0,
    Runtime = 1,
    Edit = 2,
    EditAndRuntime = Edit | Runtime, // 0x00000003
  }
}
