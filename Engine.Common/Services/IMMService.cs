using Engine.Common.MindMap;
using System.Collections.Generic;

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
