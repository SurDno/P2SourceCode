namespace PLVirtualMachine.Common
{
  public class GlobalVariable : ContextVariable
  {
    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_GLOBAL_VAR;
    }
  }
}
