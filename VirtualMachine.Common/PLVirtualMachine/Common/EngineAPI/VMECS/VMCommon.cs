using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using System;
using System.Xml;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Common", null)]
  public class VMCommon : VMComponent
  {
    public const string ComponentName = "Common";
    private string customTag = VMCommon.defaultTag;
    public static string defaultTag = "Default";

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    [Event("OnAwake", "")]
    [SpecialEvent(ESpecialEventName.SEN_START_OBJECT_FSM)]
    public event Action StartEvent;

    [Event("OnRemove", "", false)]
    public event Action RemoveEvent;

    [Method("Initialize", "", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_OBJECT_INIT)]
    public virtual void Init()
    {
    }

    [Method("Instantiate", "", "")]
    public virtual void Instantiate()
    {
      if (!this.Parent.Instantiated)
        EngineAPIManager.Instance.InstantiateObject(this.Parent);
      this.RemoveEvent();
    }

    public void OnRemove()
    {
      Action removeEvent = this.RemoveEvent;
      if (removeEvent == null)
        return;
      removeEvent();
    }

    [Property("Object enabled", "", false, true)]
    public bool ObjectEnabled
    {
      get
      {
        if (this.Parent == null)
        {
          Logger.AddError(string.Format("Parent entity for common component not defined !"));
          return false;
        }
        if (this.Parent.Instance != null)
          return this.Parent.Instance.IsEnabled;
        Logger.AddError(string.Format("Parent engine entity instance for vm entity {0} common component not defined !", (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Parent == null)
        {
          Logger.AddError(string.Format("Parent entity for common component not defined !"));
        }
        else
        {
          if (this.Parent.Instance == null)
            return;
          this.Parent.Instance.IsEnabled = value;
        }
      }
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.Save(writer, "CustomTag", this.customTag);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "CustomTag")
          this.customTag = xmlNode.ChildNodes[i].InnerText;
      }
    }

    public string CustomTag
    {
      get => this.customTag;
      set
      {
        this.customTag = value;
        this.OnModify();
      }
    }
  }
}
