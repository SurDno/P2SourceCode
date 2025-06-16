namespace PLVirtualMachine.Common.EngineAPI
{
  public class APIParamInfo
  {
    public APIParamInfo(VMType type, string name = "")
    {
      this.Type = type;
      this.Name = name;
    }

    public VMType Type { get; private set; }

    public string Name { get; private set; }
  }
}
