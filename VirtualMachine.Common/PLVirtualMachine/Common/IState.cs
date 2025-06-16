// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IState
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common
{
  public interface IState : 
    IGraphObject,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext
  {
    List<IEntryPoint> EntryPoints { get; }

    int GetExitPointsCount();

    bool Initial { get; }

    bool IgnoreBlock { get; }

    bool IsProcedure { get; }

    bool IsStable { get; }

    EStateType StateType { get; }
  }
}
