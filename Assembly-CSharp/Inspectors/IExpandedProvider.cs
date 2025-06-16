namespace Inspectors
{
  public interface IExpandedProvider
  {
    bool GetExpanded(string name);

    void SetExpanded(string name, bool value);

    string DeepName { get; set; }
  }
}
