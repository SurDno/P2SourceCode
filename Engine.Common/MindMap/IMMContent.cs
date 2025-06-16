using Engine.Common.Types;
using System;

namespace Engine.Common.MindMap
{
  public interface IMMContent
  {
    Guid Id { get; set; }

    LocalizedText Description { get; set; }

    MMContentKind Kind { get; set; }

    IMMPlaceholder Placeholder { get; set; }
  }
}
