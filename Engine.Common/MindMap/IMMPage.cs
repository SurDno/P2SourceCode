// Decompiled with JetBrains decompiler
// Type: Engine.Common.MindMap.IMMPage
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Types;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.MindMap
{
  public interface IMMPage
  {
    bool Global { get; set; }

    LocalizedText Title { get; set; }

    IEnumerable<IMMNode> Nodes { get; }

    void AddNode(IMMNode node);

    void RemoveNode(IMMNode node);

    IEnumerable<IMMLink> Links { get; }

    void AddLink(IMMLink link);

    void RemoveLink(IMMLink link);
  }
}
