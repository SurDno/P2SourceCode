// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.VMEvent
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data;
using System;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

#nullable disable
namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TEvent)]
  [DataFactory("Event")]
  public class VMEvent : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    IEvent,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext
  {
    [FieldData("Manual", DataFieldType.None)]
    private bool isManual = true;
    [FieldData("Condition", DataFieldType.Reference)]
    private ICondition eventCondition;
    [FieldData("ChangeTo", DataFieldType.None)]
    private bool changeTo = true;
    [FieldData("Repeated", DataFieldType.None)]
    private bool repeated = true;
    [FieldData("GameTimeContext", DataFieldType.Reference)]
    private IGameMode gameTimeContext;
    [FieldData("MessagesInfo", DataFieldType.None)]
    private List<NameTypeData> messageInfoData;
    [FieldData("EventRaisingType", DataFieldType.None)]
    private EEventRaisingType eventRaisingType;
    [FieldData("EventParameter", DataFieldType.Reference)]
    private VMParameter eventParameter;
    [FieldData("EventTime", DataFieldType.None)]
    private GameTime eventTime;
    private List<BaseMessage> messages;
    private IContainer owner;
    private bool atOnce;
    private bool afterLoaded;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ChangeTo":
              this.changeTo = EditorDataReadUtility.ReadValue(xml, this.changeTo);
              continue;
            case "Condition":
              this.eventCondition = EditorDataReadUtility.ReadReference<ICondition>(xml, creator);
              continue;
            case "EventParameter":
              this.eventParameter = EditorDataReadUtility.ReadReference<VMParameter>(xml, creator);
              continue;
            case "EventRaisingType":
              this.eventRaisingType = EditorDataReadUtility.ReadEnum<EEventRaisingType>(xml);
              continue;
            case "EventTime":
              this.eventTime = EditorDataReadUtility.ReadSerializable<GameTime>(xml);
              continue;
            case "GameTimeContext":
              this.gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
              continue;
            case "Manual":
              this.isManual = EditorDataReadUtility.ReadValue(xml, this.isManual);
              continue;
            case "MessagesInfo":
              this.messageInfoData = EditorDataReadUtility.ReadEditorDataSerializableList<NameTypeData>(xml, creator, this.messageInfoData);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Repeated":
              this.repeated = EditorDataReadUtility.ReadValue(xml, this.repeated);
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

    public VMEvent(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_EVENT;

    public bool IsManual => this.isManual;

    public EEventRaisingType EventRaisingType => this.eventRaisingType;

    public string FunctionalName
    {
      get => this.Parent == null ? this.Name : this.Parent.Name + "." + this.Name;
    }

    public override IContainer Owner => this.owner;

    public bool IsInitial(IObject obj)
    {
      try
      {
        if (!this.afterLoaded)
          this.OnAfterLoad();
        return this.owner != null && obj.IsEqual((IObject) this.owner) && !this.IsManual && this.Name == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_START_OBJECT_FSM, typeof (VMCommon));
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Event {0} initial checking error: {1}", (object) this.Name, (object) ex.ToString()));
      }
      return false;
    }

    public ICondition Condition => this.eventCondition;

    public IParam EventParameter => (IParam) this.eventParameter;

    public GameTime EventTime => this.eventTime;

    public bool ChangeTo => this.changeTo;

    public bool Repeated => this.repeated;

    public bool AtOnce => this.atOnce;

    public IGameMode GameTimeContext => this.gameTimeContext;

    public List<BaseMessage> ReturnMessages
    {
      get
      {
        if (!this.IsUpdated)
          this.Update();
        return this.messages;
      }
    }

    public virtual List<IVariable> GetLocalContextVariables(
      EContextVariableCategory eContextVarCategory,
      IContextElement currentElement,
      int iCounter = 0)
    {
      return new List<IVariable>();
    }

    public IVariable GetLocalContextVariable(string variableUniName, IContextElement currentElement = null)
    {
      return (IVariable) null;
    }

    public void OnAfterLoad()
    {
      this.owner = this.IsManual ? this.Parent : (!typeof (IFunctionalComponent).IsAssignableFrom(this.Parent.GetType()) ? this.Parent : this.Parent.Parent);
      if (this.owner == null)
      {
        Logger.AddError(string.Format("Invalid event: id={0}", (object) this.BaseGuid));
      }
      else
      {
        if (this.messages == null)
          this.LoadEventMessages();
        this.afterLoaded = true;
      }
    }

    public bool IsAfterLoaded => this.afterLoaded;

    public override void Clear()
    {
      if (this.eventCondition != null)
      {
        ((VMPartCondition) this.eventCondition).Clear();
        this.eventCondition = (ICondition) null;
      }
      this.gameTimeContext = (IGameMode) null;
      if (this.messageInfoData != null)
      {
        this.messageInfoData.Clear();
        this.messageInfoData = (List<NameTypeData>) null;
      }
      this.eventParameter = (VMParameter) null;
      this.eventTime = (GameTime) null;
      this.owner = (IContainer) null;
      if (this.messages == null)
        return;
      foreach (ContextVariable message in this.messages)
        message.Clear();
      this.messages.Clear();
      this.messages = (List<BaseMessage>) null;
    }

    private void LoadEventMessages()
    {
      this.messages = new List<BaseMessage>();
      if (!this.IsManual)
      {
        if (this.Parent == null)
        {
          Logger.AddError("Standart messages loading requires parent component");
        }
        else
        {
          APIEventInfo apiEventInfoByName = EngineAPIManager.GetAPIEventInfoByName(this.Parent.Name, this.Name);
          if (apiEventInfoByName == null)
          {
            Logger.AddError(string.Format("Component {0} haven't info for event {1}", (object) this.Parent.Name, (object) this.Name));
          }
          else
          {
            for (int index = 0; index < apiEventInfoByName.MessageParams.Count; ++index)
            {
              VMType type = apiEventInfoByName.MessageParams[index].Type;
              string name = this.Name + "_message_" + apiEventInfoByName.MessageParams[index].Name;
              BaseMessage baseMessage = new BaseMessage();
              baseMessage.Initialize(name, type);
              this.messages.Add(baseMessage);
            }
            this.atOnce = apiEventInfoByName.AtOnce;
          }
        }
      }
      else
      {
        if (this.messageInfoData == null)
          return;
        foreach (NameTypeData nameTypeData in this.messageInfoData)
        {
          BaseMessage baseMessage = new BaseMessage();
          baseMessage.Initialize(nameTypeData.Name, nameTypeData.Type);
          this.messages.Add(baseMessage);
        }
        this.messageInfoData = (List<NameTypeData>) null;
      }
    }
  }
}
