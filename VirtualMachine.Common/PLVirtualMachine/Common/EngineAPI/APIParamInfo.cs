namespace PLVirtualMachine.Common.EngineAPI
{
  public class APIParamInfo(VMType type, string name = "") {
    public VMType Type { get; private set; } = type;

    public string Name { get; private set; } = name;
  }
}
