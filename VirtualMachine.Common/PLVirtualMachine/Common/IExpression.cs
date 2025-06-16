using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  public interface IExpression : IObject, IEditorBaseTemplate, IFunctionalPoint, IStaticUpdateable
  {
    bool IsValid { get; }

    ExpressionType Type { get; }

    VMType ResultType { get; }

    bool Inversion { get; }

    IParam TargetConstant { get; }

    int ChildExpressionsCount { get; }

    IExpression GetChildExpression(int childIndex);

    FormulaOperation GetChildOperations(int childIndex);
  }
}
