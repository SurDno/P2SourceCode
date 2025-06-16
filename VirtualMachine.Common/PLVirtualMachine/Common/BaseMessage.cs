namespace PLVirtualMachine.Common
{
  public class BaseMessage : ContextVariable
  {
    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_MESSAGE;
    }
  }
}
