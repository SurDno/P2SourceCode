using Engine.Common.Types;
using System;

namespace Engine.Common.MindMap
{
  public interface IMMNode
  {
    Guid Id { get; set; }

    Position Position { get; set; }

    MMNodeKind NodeKind { get; set; }

    IMMContent Content { get; set; }

    bool Undiscovered { get; set; }
  }
}
