namespace PLVirtualMachine.Common
{
  public class LocalVariable : ContextVariable
  {
    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR;
    }
  }
}
