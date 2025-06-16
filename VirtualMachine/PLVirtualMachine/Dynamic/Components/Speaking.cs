using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Objects;
using System;
using System.Linq;
using System.Xml;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMSpeaking))]
  public class Speaking : VMSpeaking, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Speaking);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "BeginTalkingEvent":
          this.BeginTalkingEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "EndTalkingEvent":
          this.EndTalkingEvent += (Action<IStateRef>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnSpeechReplyEvent":
          this.OnSpeechReplyEvent += (Action<ulong>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.Save(writer, "ContextTalkingAvailable", this.contextTalkingAvailable);
      SaveManagerUtility.SaveSerializable(writer, "CurrentTalking", (IVMStringSerializable) this.currentTalking);
      SaveManagerUtility.SaveList(writer, "PassedOnlyOnceTalkigList", this.passedOnlyOnceTalkigs.Select<IFiniteStateMachine, ulong>((Func<IFiniteStateMachine, ulong>) (o => o.BaseGuid)));
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode1.Name == "ContextTalkingAvailable")
          this.contextTalkingAvailable = VMSaveLoadManager.ReadBool((XmlNode) childNode1);
        else if (childNode1.Name == "CurrentTalking")
          this.currentTalking = VMSaveLoadManager.ReadValue<IStateRef>((XmlNode) childNode1);
        else if (childNode1.Name == "PassedOnlyOnceTalkigList")
        {
          this.passedOnlyOnceTalkigs.Clear();
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            ulong stateId = VMSaveLoadManager.ReadUlong(childNode2);
            if (this.Parent.EditorTemplate == null)
            {
              Logger.AddError(string.Format("SaveLoad error: editor template not defined at {0}", (object) this.Parent.Name));
              return;
            }
            if (this.Parent.EditorTemplate.StateGraph == null)
            {
              Logger.AddError(string.Format("SaveLoad error: state graph not defined at talking object {0}", (object) this.Parent.Name));
              return;
            }
            IState stateByGuid = this.Parent.EditorTemplate.StateGraph.GetStateByGuid(stateId);
            if (stateByGuid != null)
              this.passedOnlyOnceTalkigs.Add((IFiniteStateMachine) stateByGuid);
          }
        }
      }
    }
  }
}
