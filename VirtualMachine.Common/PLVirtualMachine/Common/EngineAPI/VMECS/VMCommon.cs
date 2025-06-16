using System;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Common")]
  public class VMCommon : VMComponent
  {
    public const string ComponentName = "Common";
    private string customTag = defaultTag;
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
      if (!Parent.Instantiated)
        EngineAPIManager.Instance.InstantiateObject(Parent);
      RemoveEvent();
    }

    public void OnRemove()
    {
      Action removeEvent = RemoveEvent;
      if (removeEvent == null)
        return;
      removeEvent();
    }

    [Property("Object enabled", "", false, true)]
    public bool ObjectEnabled
    {
      get
      {
        if (Parent == null)
        {
          Logger.AddError("Parent entity for common component not defined !");
          return false;
        }
        if (Parent.Instance != null)
          return Parent.Instance.IsEnabled;
        Logger.AddError(string.Format("Parent engine entity instance for vm entity {0} common component not defined !", Parent.Name));
        return false;
      }
      set
      {
        if (Parent == null)
        {
          Logger.AddError("Parent entity for common component not defined !");
        }
        else
        {
          if (Parent.Instance == null)
            return;
          Parent.Instance.IsEnabled = value;
        }
      }
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.Save(writer, "CustomTag", customTag);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "CustomTag")
          customTag = xmlNode.ChildNodes[i].InnerText;
      }
    }

    public string CustomTag
    {
      get => customTag;
      set
      {
        customTag = value;
        OnModify();
      }
    }
  }
}
