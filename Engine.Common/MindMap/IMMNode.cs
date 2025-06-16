// Decompiled with JetBrains decompiler
// Type: Engine.Common.MindMap.IMMNode
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Types;
using System;

#nullable disable
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
