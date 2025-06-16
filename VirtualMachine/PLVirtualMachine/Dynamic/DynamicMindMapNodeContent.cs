// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.DynamicMindMapNodeContent
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.MindMap;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.LogicMap;
using System;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class DynamicMindMapNodeContent : 
    IDynamicMindMapObject,
    ISerializeStateSave,
    IDynamicLoadSerializable,
    INamed
  {
    private VMLogicMapNodeContent staticNodeContent;
    private DynamicMindMapNode parentNode;
    private IMMContent engineContent;
    private Guid dynamicGuid;
    private bool active;
    private DynamicEventBody nodeContentActivateEventBody;
    private bool firstThink = true;

    public DynamicMindMapNodeContent(
      DynamicMindMapNode parentNode,
      VMLogicMapNodeContent staticNodeContent)
    {
      this.parentNode = parentNode;
      this.staticNodeContent = staticNodeContent;
      this.dynamicGuid = DynamicMindMap.RegistrDynamicMMObject((IDynamicMindMapObject) this);
      this.active = false;
      if (staticNodeContent == null)
        return;
      MMContentKind byNodeContentType = DynamicMindMap.GetEngineNodeContentKindByNodeContentType(staticNodeContent.ContentType);
      IMMPlaceholder mmPlaceholder = (IMMPlaceholder) null;
      if (staticNodeContent.Picture != null)
      {
        Guid engineTemplateGuid = staticNodeContent.Picture.EngineTemplateGuid;
        mmPlaceholder = ServiceCache.TemplateService.GetTemplate<IMMPlaceholder>(engineTemplateGuid);
        if (mmPlaceholder == null)
          Logger.AddError(string.Format("Mindmap placeholder with guid={0} not found", (object) engineTemplateGuid));
      }
      this.engineContent = ServiceCache.Factory.Create<IMMContent>(this.dynamicGuid);
      this.engineContent.Kind = byNodeContentType;
      this.engineContent.Placeholder = mmPlaceholder;
      this.engineContent.Description = EngineAPIManager.CreateEngineTextInstance(staticNodeContent.DescriptionText);
      ServiceCache.MindMapService.AddContent(this.engineContent);
      this.nodeContentActivateEventBody = new DynamicEventBody((VMPartCondition) staticNodeContent.ContentCondition, (INamed) this, parentNode.GameTimeContext, new DynamicEventBody.OnEventBodyRise(this.OnCheckRise), false);
    }

    public string Name
    {
      get
      {
        string str = "none";
        if (this.staticNodeContent != null)
          str = this.staticNodeContent.ContentNumber.ToString();
        return string.Format("Node {0} content index {1}", (object) this.parentNode.Name, (object) str);
      }
    }

    public ulong StaticGuid
    {
      get => this.staticNodeContent == null ? 0UL : this.staticNodeContent.BaseGuid;
    }

    public Guid DynamicGuid => this.dynamicGuid;

    public bool Active
    {
      get => this.active;
      set => this.active = value;
    }

    public void OnCheckRise(object newConditionValue, EEventRaisingMode raisingMode)
    {
      bool active = this.Active;
      bool bActive = (bool) newConditionValue;
      if (bActive == active)
        return;
      this.parentNode.SetContentActive(this, bActive);
    }

    public void Think()
    {
      if (this.firstThink)
      {
        this.OnCheckRise((object) this.nodeContentActivateEventBody.CalculateConditionResult(), EEventRaisingMode.ERM_ADD_TO_QUEUE);
        this.firstThink = false;
      }
      else
      {
        if (!this.NeedUpdate())
          return;
        this.nodeContentActivateEventBody.Think();
      }
    }

    public void Free()
    {
      this.nodeContentActivateEventBody.ClearSubscribtions();
      ServiceCache.MindMapService.RemoveContent(this.engineContent);
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "StaticGuid", this.staticNodeContent != null ? this.staticNodeContent.BaseGuid : 0UL);
      SaveManagerUtility.Save(writer, "DynamicGuid", this.dynamicGuid);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "DynamicGuid")
        {
          this.dynamicGuid = VMSaveLoadManager.ReadGuid((XmlNode) childNode);
          this.engineContent.Id = this.dynamicGuid;
        }
      }
      if (this.staticNodeContent != null)
        return;
      Logger.AddError(string.Format("SaveLoad error: dynamic map node content id={0} hasn't his static object, probably it was removed from GameData", (object) this.dynamicGuid));
    }

    public void AfterSaveLoading()
    {
      if (this.nodeContentActivateEventBody != null)
        this.nodeContentActivateEventBody.AfterSaveLoading();
      this.Active = this.nodeContentActivateEventBody.CalculateConditionResult();
      this.firstThink = true;
    }

    public bool Modified => true;

    public void OnModify()
    {
    }

    public bool NeedUpdate()
    {
      return this.nodeContentActivateEventBody != null && this.nodeContentActivateEventBody.NeedUpdate();
    }
  }
}
