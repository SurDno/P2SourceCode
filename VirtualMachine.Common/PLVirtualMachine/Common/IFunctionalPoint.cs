// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IFunctionalPoint
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common
{
  public interface IFunctionalPoint : IStaticUpdateable
  {
    string TargetFunction { get; }

    BaseFunction TargetFunctionInstance { get; }

    ILocalContext LocalContext { get; }

    CommonVariable TargetObject { get; }

    CommonVariable TargetParam { get; }

    List<CommonVariable> SourceParams { get; }
  }
}
