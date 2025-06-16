namespace PLVirtualMachine.Dynamic
{
  public interface IDependedEventRef
  {
    void OnParamUpdate(bool bValueChange, DynamicParameter dynParamInstance);
  }
}
