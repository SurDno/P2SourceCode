using System.Collections.Generic;

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
