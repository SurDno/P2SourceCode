namespace Engine.Source.Services.Consoles
{
  public struct ConsoleParameter
  {
    public static readonly ConsoleParameter Empty = new();
    public string Parameter;
    public string Value;
  }
}
