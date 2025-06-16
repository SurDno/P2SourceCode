// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.FSM.VMSpeech
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.GameLogic;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
namespace PLVirtualMachine.FSM
{
  [TypeData(EDataType.TSpeech)]
  [DataFactory("Speech")]
  public class VMSpeech : 
    VMState,
    IStub,
    IEditorDataReader,
    ISpeech,
    IState,
    IGraphObject,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext
  {
    [FieldData("Replyes", DataFieldType.Reference)]
    private List<ISpeechReply> exitPoints = new List<ISpeechReply>();
    [FieldData("Text", DataFieldType.Reference)]
    private VMGameString text;
    [FieldData("AuthorGuid", DataFieldType.None)]
    private ulong speechAuthorObjGuid;
    [FieldData("OnlyOnce", DataFieldType.None)]
    private bool onlyOnce;
    [FieldData("IsTrade", DataFieldType.None)]
    private bool isTrade;
    [FieldData("ParamText", DataFieldType.Reference)]
    private VMParameter speechParam;
    private IVariable speechAuthor;

    public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "AuthorGuid":
              this.speechAuthorObjGuid = EditorDataReadUtility.ReadValue(xml, this.speechAuthorObjGuid);
              continue;
            case "EntryPoints":
              this.entryPoints = EditorDataReadUtility.ReadReferenceList<IEntryPoint>(xml, creator, this.entryPoints);
              continue;
            case "IgnoreBlock":
              this.ignoreBlock = EditorDataReadUtility.ReadValue(xml, this.ignoreBlock);
              continue;
            case "Initial":
              this.initial = EditorDataReadUtility.ReadValue(xml, this.initial);
              continue;
            case "InputLinks":
              this.inputLinks = EditorDataReadUtility.ReadReferenceList<VMEventLink>(xml, creator, this.inputLinks);
              continue;
            case "IsTrade":
              this.isTrade = EditorDataReadUtility.ReadValue(xml, this.isTrade);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "OnlyOnce":
              this.onlyOnce = EditorDataReadUtility.ReadValue(xml, this.onlyOnce);
              continue;
            case "OutputLinks":
              this.outputLinks = EditorDataReadUtility.ReadReferenceList<ILink>(xml, creator, this.outputLinks);
              continue;
            case "Owner":
              this.owner = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "ParamText":
              this.speechParam = EditorDataReadUtility.ReadReference<VMParameter>(xml, creator);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Replyes":
              this.exitPoints = EditorDataReadUtility.ReadReferenceList<ISpeechReply>(xml, creator, this.exitPoints);
              continue;
            case "Text":
              this.text = EditorDataReadUtility.ReadReference<VMGameString>(xml, creator);
              continue;
            default:
              if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
                Logger.AddError(typeContext + " : " + xml.Name);
              XmlReaderUtility.SkipNode(xml);
              continue;
          }
        }
        else if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public VMSpeech(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public override EStateType StateType => EStateType.STATE_TYPE_SPEECH;

    public IGameString Text => (IGameString) this.text;

    public IParam TextParam => (IParam) this.speechParam;

    public IObjRef Author
    {
      get
      {
        if (this.speechAuthor != null)
        {
          if (typeof (VMParameter).IsAssignableFrom(this.speechAuthor.GetType()))
          {
            VMParameter speechAuthor = (VMParameter) this.speechAuthor;
            if (typeof (IObjRef).IsAssignableFrom(speechAuthor.Type.BaseType))
              return (IObjRef) speechAuthor.Value;
          }
          else if (typeof (IObjRef).IsAssignableFrom(this.speechAuthor.GetType()))
            return (IObjRef) this.speechAuthor;
        }
        return (IObjRef) null;
      }
    }

    public ulong SpeechAuthorObjGuid => this.speechAuthorObjGuid;

    public bool OnlyOnce => this.onlyOnce;

    public bool IsTrade => this.isTrade;

    public override int GetExitPointsCount() => this.exitPoints.Count;

    public List<ISpeechReply> Replies => this.exitPoints;

    public IActionLine ActionLine
    {
      get => this.entryPoints.Count > 0 ? this.entryPoints[0].ActionLine : (IActionLine) null;
    }

    public override bool IgnoreBlock => !this.IsTrade || base.IgnoreBlock;

    public override void Update()
    {
    }

    public override void OnAfterLoad()
    {
      if (this.IsAfterLoaded)
        return;
      if (!VMBaseObjectUtility.CheckOrders<ISpeechReply>(this.exitPoints))
        Logger.AddError(string.Format("Speech line id={0} has invalid replyes ordering", (object) this.BaseGuid));
      base.OnAfterLoad();
      this.BindAuthor();
      for (int index = 0; index < this.Replies.Count; ++index)
      {
        IActionLine actionLine = this.Replies[index].ActionLine;
        if (actionLine != null)
          this.MakeLocalContextElementsDependencys((IContextElement) actionLine);
      }
    }

    public override void Clear()
    {
      base.Clear();
      if (this.exitPoints != null)
      {
        foreach (ISpeechReply exitPoint in this.exitPoints)
        {
          if (typeof (VMSpeechReply) == exitPoint.GetType())
            ((VMBaseObject) exitPoint).Clear();
        }
        this.exitPoints.Clear();
        this.exitPoints = (List<ISpeechReply>) null;
      }
      this.text = (VMGameString) null;
      this.speechParam = (VMParameter) null;
      this.speechAuthor = (IVariable) null;
    }

    protected void BindAuthor()
    {
      this.Parent.Update();
      this.speechAuthor = ((VMTalkingGraph) this.Parent).GetSpeechAuthorInfo(this.speechAuthorObjGuid);
      if (this.speechAuthor != null)
        return;
      Logger.AddError("Cannot bind speech author: unknown author object guid: " + this.speechAuthorObjGuid.ToString());
    }
  }
}
