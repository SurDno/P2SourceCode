using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Cofe.Serializations.Data;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using Engine.Source.UI;
using Engine.Source.UI.Menu.Protagonist.MindMap;
using Inspectors;

namespace Engine.Source.Services
{
  [GameService(typeof (MMService), typeof (IMMService))]
  public class MMService : IMMService, ISavesController
  {
    private MMWindow map;

    public event Action ChangeUndiscoveredEvent;

    [Inspected]
    public IEnumerable<IMMContent> Contents => Map.Contents;

    [Inspected]
    public IMMPage CurrentGlobalPage
    {
      get => Map.GlobalPage;
      set => Map.GlobalPage = (MMPage) value;
    }

    [Inspected]
    public IMMPage CurrentPage
    {
      get => Map.LastPage;
      set => Map.LastPage = (MMPage) value;
    }

    private MMWindow Map
    {
      get
      {
        if ((UnityEngine.Object) map == (UnityEngine.Object) null)
          map = (MMWindow) ServiceLocator.GetService<UIService>().Get<IMMWindow>();
        return map;
      }
    }

    [Inspected]
    public IEnumerable<IMMPage> Pages => Map.Pages;

    public void AddContent(IMMContent content) => Map.AddContent(content);

    public void AddPage(IMMPage page) => Map.AddPage((MMPage) page);

    public void ClearPages() => Map.ClearPages();

    public IMMPage GetPage(int index) => Map.GetPage(index);

    public bool HasUndiscovered()
    {
      IMMPage currentGlobalPage = CurrentGlobalPage;
      if (currentGlobalPage != null && ((MMPage) currentGlobalPage).HasUndiscovered())
        return true;
      foreach (IMMPage page in Pages)
      {
        if (page != null && ((MMPage) page).HasUndiscovered())
          return true;
      }
      return false;
    }

    public void RemoveContent(IMMContent content) => Map.RemoveContent(content);

    public void RemovePage(IMMPage page) => Map.RemovePage((MMPage) page);

    public void RemovePageAt(int index) => Map.RemovePageAt(index);

    public void SetPage(int index, IMMPage page) => Map.SetPage(index, (MMPage) page);

    public void FireChangeUndiscoveredEvent()
    {
      Action undiscoveredEvent = ChangeUndiscoveredEvent;
      if (undiscoveredEvent == null)
        return;
      undiscoveredEvent();
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public void Unload() => ClearPages();

    public void Save(IDataWriter writer, string context)
    {
    }
  }
}
