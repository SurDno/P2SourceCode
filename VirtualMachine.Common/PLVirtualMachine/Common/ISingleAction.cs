namespace PLVirtualMachine.Common
{
  public interface ISingleAction : 
    IGameAction,
    IOrderedChild,
    IContextElement,
    IObject,
    IEditorBaseTemplate,
    INamed,
    IBaseAction,
    IStaticUpdateable,
    IAbstractAction,
    IFunctionalPoint
  {
    IExpression SourceExpression { get; }
  }
}
