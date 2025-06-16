namespace Engine.Common.Components.Prototype
{
  public class Message
  {
    public object Content;
    public Kind Type;

    public enum Kind
    {
      Unknown,
      Text,
      Int,
      Float,
    }
  }
}
