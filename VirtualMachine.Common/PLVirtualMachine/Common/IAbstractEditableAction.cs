namespace PLVirtualMachine.Common
{
  public interface IAbstractEditableAction : 
    IAbstractAction,
    IBaseAction,
    IFunctionalPoint,
    IStaticUpdateable
  {
    void CheckFunctionUpdate();
  }
}
