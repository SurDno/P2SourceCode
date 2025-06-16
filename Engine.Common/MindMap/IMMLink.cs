namespace Engine.Common.MindMap
{
  public interface IMMLink
  {
    IMMNode Origin { get; set; }

    IMMNode Target { get; set; }

    MMLinkKind Kind { get; set; }
  }
}
