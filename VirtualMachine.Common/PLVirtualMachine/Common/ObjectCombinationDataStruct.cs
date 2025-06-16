using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  [VMType("ObjectCombinationDataStruct")]
  public class ObjectCombinationDataStruct : IVMStringSerializable
  {
    private List<ObjectCombinationElement> combinationElements = new List<ObjectCombinationElement>();

    public int GetElementsCount() => this.combinationElements.Count;

    public ObjectCombinationElement GetCombinationElement(int elemInd)
    {
      return elemInd < 0 || elemInd >= this.combinationElements.Count ? (ObjectCombinationElement) null : this.combinationElements[elemInd];
    }

    public bool ContainsItem(IBlueprint item)
    {
      for (int index = 0; index < this.combinationElements.Count; ++index)
      {
        if (this.combinationElements[index].ContainsItem(item))
          return true;
      }
      return false;
    }

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null object combination data at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        default:
          if (data.Length < 12)
          {
            Logger.AddError(string.Format("Cannot convert {0} to object combination data struct at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
            break;
          }
          string[] separator = new string[1]{ "END&ELEM" };
          string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
          this.combinationElements.Clear();
          for (int index = 0; index < strArray.Length; ++index)
            this.combinationElements.Add(new ObjectCombinationElement(strArray[index]));
          break;
      }
    }

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }
  }
}
