using Cofe.Loggers;
using PLVirtualMachine.Common;
using System.Collections.Generic;

namespace PLVirtualMachine.Dynamic
{
  public class LoopInfo
  {
    private ICommonList loopList;
    private int currentIndex;
    private object currentListElement;
    private Dictionary<string, object> loopLocalVariableValuesDict;
    private string loopIndexVariableName = "";
    private string loopListElementVariableName = "";

    public LoopInfo(IContextElement ownerContextElement, ICommonList loopList = null)
    {
      this.currentIndex = 0;
      this.loopList = loopList;
      List<IVariable> contextVariables = ownerContextElement.LocalContextVariables;
      if (contextVariables.Count == 1)
        this.loopIndexVariableName = contextVariables[0].Name;
      else if (contextVariables.Count == 2)
      {
        this.loopListElementVariableName = contextVariables[0].Name;
        this.loopIndexVariableName = contextVariables[1].Name;
      }
      else
        Logger.AddError(string.Format("Invalid action loop guid={0} context variables count : {1}", (object) ownerContextElement.BaseGuid, (object) contextVariables.Count));
    }

    public void RegistrLoopLocalVarsDict(
      Dictionary<string, object> loopLocalVariableValuesDict)
    {
      this.loopLocalVariableValuesDict = loopLocalVariableValuesDict;
    }

    public int CurrentLoopIndex
    {
      get => this.currentIndex;
      set
      {
        this.currentIndex = value;
        if (this.loopLocalVariableValuesDict == null)
        {
          Logger.AddError(string.Format("Loop local variables dictionary not regitered in loop info !!!"));
        }
        else
        {
          this.loopLocalVariableValuesDict[this.loopIndexVariableName] = (object) this.currentIndex;
          if (this.loopList == null)
            return;
          this.currentListElement = this.loopList.GetObject(this.currentIndex);
          this.loopLocalVariableValuesDict[this.loopListElementVariableName] = this.currentListElement;
        }
      }
    }
  }
}
