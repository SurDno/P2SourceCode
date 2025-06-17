using System;

namespace Cinemachine
{
  [DocumentationSorting(0.0f, Level.Undoc)]
  public sealed class DocumentationSortingAttribute(
    float sortOrder,
    DocumentationSortingAttribute.Level category)
    : Attribute 
  {
    public float SortOrder { get; private set; } = sortOrder;

    public Level Category { get; private set; } = category;

    public enum Level
    {
      Undoc,
      API,
      UserRef,
    }
  }
}
