// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.MMService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Data;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using Engine.Source.UI;
using Engine.Source.UI.Menu.Protagonist.MindMap;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (MMService), typeof (IMMService)})]
  public class MMService : IMMService, ISavesController
  {
    private MMWindow map;

    public event Action ChangeUndiscoveredEvent;

    [Inspected]
    public IEnumerable<IMMContent> Contents => this.Map.Contents;

    [Inspected]
    public IMMPage CurrentGlobalPage
    {
      get => (IMMPage) this.Map.GlobalPage;
      set => this.Map.GlobalPage = (MMPage) value;
    }

    [Inspected]
    public IMMPage CurrentPage
    {
      get => (IMMPage) this.Map.LastPage;
      set => this.Map.LastPage = (MMPage) value;
    }

    private MMWindow Map
    {
      get
      {
        if ((UnityEngine.Object) this.map == (UnityEngine.Object) null)
          this.map = (MMWindow) ServiceLocator.GetService<UIService>().Get<IMMWindow>();
        return this.map;
      }
    }

    [Inspected]
    public IEnumerable<IMMPage> Pages => this.Map.Pages;

    public void AddContent(IMMContent content) => this.Map.AddContent(content);

    public void AddPage(IMMPage page) => this.Map.AddPage((MMPage) page);

    public void ClearPages() => this.Map.ClearPages();

    public IMMPage GetPage(int index) => (IMMPage) this.Map.GetPage(index);

    public bool HasUndiscovered()
    {
      IMMPage currentGlobalPage = this.CurrentGlobalPage;
      if (currentGlobalPage != null && ((MMPage) currentGlobalPage).HasUndiscovered())
        return true;
      foreach (IMMPage page in this.Pages)
      {
        if (page != null && ((MMPage) page).HasUndiscovered())
          return true;
      }
      return false;
    }

    public void RemoveContent(IMMContent content) => this.Map.RemoveContent(content);

    public void RemovePage(IMMPage page) => this.Map.RemovePage((MMPage) page);

    public void RemovePageAt(int index) => this.Map.RemovePageAt(index);

    public void SetPage(int index, IMMPage page) => this.Map.SetPage(index, (MMPage) page);

    public void FireChangeUndiscoveredEvent()
    {
      Action undiscoveredEvent = this.ChangeUndiscoveredEvent;
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

    public void Unload() => this.ClearPages();

    public void Save(IDataWriter writer, string context)
    {
    }
  }
}
