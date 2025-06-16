// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.DynamicMindMapNode
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Serializations.Data;
using Engine.Common.MindMap;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.LogicMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class DynamicMindMapNode : 
    IDynamicMindMapObject,
    ISerializeStateSave,
    IDynamicLoadSerializable
  {
    private DynamicMindMap parentMindMap;
    private VMLogicMapNode staticNode;
    private Guid dynamicGuid;
    private List<DynamicMindMapNodeContent> nodeContentList = new List<DynamicMindMapNodeContent>();
    private DynamicMindMapNodeContent activeContent;
    private bool undiscovered;

    public DynamicMindMapNode(DynamicMindMap parentMindMap, VMLogicMapNode staticNode)
    {
      this.parentMindMap = parentMindMap;
      this.staticNode = staticNode;
      this.dynamicGuid = DynamicMindMap.RegistrDynamicMMObject((IDynamicMindMapObject) this);
      List<ILink> linksByDestNode = parentMindMap.GetLinksByDestNode(this);
      linksByDestNode.AddRange((IEnumerable<ILink>) linksByDestNode);
      List<ILink> linksBySourceNode = parentMindMap.GetLinksBySourceNode(this);
      linksBySourceNode.AddRange((IEnumerable<ILink>) linksBySourceNode);
      this.LoadContent();
      this.activeContent = (DynamicMindMapNodeContent) null;
    }

    public ulong StaticGuid => this.staticNode == null ? 0UL : this.staticNode.BaseGuid;

    public Guid DynamicGuid => this.dynamicGuid;

    public VMLogicMapNode StaticNode => this.staticNode;

    public IMMNode EngineNode { get; set; }

    public void Think()
    {
      DynamicFSM.SetCurrentDebugState((IGraphObject) this.StaticNode);
      for (int index = 0; index < this.nodeContentList.Count; ++index)
        this.nodeContentList[index].Think();
      DynamicFSM.SetCurrentDebugState((IGraphObject) null);
    }

    public void SetContentActive(DynamicMindMapNodeContent content, bool bActive)
    {
      content.Active = bActive;
      this.MakeCurrentActiveContent();
    }

    public IGameMode GameTimeContext => this.parentMindMap.GameTimeContext;

    public void Free()
    {
      for (int index = 0; index < this.nodeContentList.Count; ++index)
        this.nodeContentList[index].Free();
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "StaticGuid", this.staticNode.BaseGuid);
      SaveManagerUtility.Save(writer, "DynamicGuid", this.dynamicGuid);
      SaveManagerUtility.Save(writer, "Undiscovered", this.EngineNode.Undiscovered);
      SaveManagerUtility.SaveDynamicSerializableList<DynamicMindMapNodeContent>(writer, "MMNodeContentList", this.nodeContentList);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode1.Name == "DynamicGuid")
        {
          this.dynamicGuid = VMSaveLoadManager.ReadGuid((XmlNode) childNode1);
          this.EngineNode.Id = this.dynamicGuid;
        }
        else if (childNode1.Name == "Undiscovered")
          this.undiscovered = VMSaveLoadManager.ReadBool((XmlNode) childNode1);
        else if (childNode1.Name == "MMNodeContentList")
        {
          foreach (XmlElement childNode2 in childNode1.ChildNodes)
          {
            ulong num = VMSaveLoadManager.ReadUlong(childNode2.FirstChild);
            if (num != 0UL && this.staticNode.GetContentByGuid(num) != null)
              ((DynamicMindMapNodeContent) DynamicMindMap.GetDynamicMMObjectByStaticguid(num))?.LoadFromXML(childNode2);
          }
        }
      }
    }

    public void AfterSaveLoading()
    {
      for (int index = 0; index < this.nodeContentList.Count; ++index)
        this.nodeContentList[index].AfterSaveLoading();
      this.MakeCurrentActiveContent();
      this.EngineNode.Undiscovered = this.undiscovered;
    }

    public string Name
    {
      get
      {
        return string.Format("{0}.{1}", (object) this.parentMindMap.Name, (object) this.staticNode.Name);
      }
    }

    public bool Modified => true;

    public void OnModify()
    {
    }

    private void MakeCurrentActiveContent()
    {
      this.activeContent = (DynamicMindMapNodeContent) null;
      for (int index = 0; index < this.nodeContentList.Count; ++index)
      {
        if (this.nodeContentList[index].Active)
          this.activeContent = this.nodeContentList[index];
      }
      IMMService mindMapService = ServiceCache.MindMapService;
      if (this.activeContent != null)
        this.EngineNode.Content = mindMapService.Contents.FirstOrDefault<IMMContent>((Func<IMMContent, bool>) (o => o.Id == this.activeContent.DynamicGuid));
      else
        this.EngineNode.Content = (IMMContent) null;
    }

    private void LoadContent()
    {
      if (this.staticNode == null)
        return;
      for (int index = 0; index < this.staticNode.Contents.Count; ++index)
        this.nodeContentList.Add(new DynamicMindMapNodeContent(this, this.staticNode.Contents[index]));
    }
  }
}
