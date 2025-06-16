// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.IMMService
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.MindMap;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Services
{
  public interface IMMService
  {
    IMMPage CurrentGlobalPage { get; set; }

    IMMPage CurrentPage { get; set; }

    void AddPage(IMMPage page);

    void ClearPages();

    IMMPage GetPage(int index);

    void RemovePage(IMMPage page);

    void RemovePageAt(int index);

    void SetPage(int index, IMMPage page);

    IEnumerable<IMMContent> Contents { get; }

    void AddContent(IMMContent content);

    void RemoveContent(IMMContent content);
  }
}
