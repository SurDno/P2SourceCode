using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public class ObjectCombinationElement
  {
    private List<ObjectCombinationVariant> combinationVariants = new List<ObjectCombinationVariant>();
    private int spawnProbability = 100;
    private float variantsSumWeight;

    public ObjectCombinationElement()
    {
    }

    public ObjectCombinationElement(ObjectCombinationElement cloneElement)
    {
      for (int index = 0; index < cloneElement.combinationVariants.Count; ++index)
        this.combinationVariants.Add(new ObjectCombinationVariant(cloneElement.combinationVariants[index].ObjectGuid, cloneElement.combinationVariants[index].MinCount, cloneElement.combinationVariants[index].MaxCount)
        {
          Weight = cloneElement.combinationVariants[index].Weight,
          CIParams = new CombinationItemParams(cloneElement.combinationVariants[index].CIParams)
        });
      this.spawnProbability = cloneElement.spawnProbability;
      this.variantsSumWeight = cloneElement.variantsSumWeight;
    }

    public ObjectCombinationElement(string dataStr) => this.Read(dataStr);

    public bool ContainsItem(IBlueprint item)
    {
      for (int index = 0; index < this.combinationVariants.Count; ++index)
      {
        if (this.combinationVariants[index].ContainsItem(item))
          return true;
      }
      return false;
    }

    public int GetVariantsCount() => this.combinationVariants.Count;

    public int SpawnProbability
    {
      get => this.spawnProbability;
      set => this.spawnProbability = value;
    }

    public ObjectCombinationVariant GetRandomVariantByWeight(float randVal)
    {
      if (this.combinationVariants.Count == 0)
        return (ObjectCombinationVariant) null;
      float num1 = this.variantsSumWeight * randVal;
      float num2 = 0.0f;
      for (int index = 0; index < this.combinationVariants.Count; ++index)
      {
        double num3 = (double) num2;
        num2 += (float) this.combinationVariants[index].Weight;
        double num4 = (double) num1;
        if (num3 <= num4 && (double) num1 <= (double) num2)
          return this.combinationVariants[index];
      }
      return this.combinationVariants[0];
    }

    public void Read(string dataStr)
    {
      if (dataStr.Length == 0)
        return;
      if (dataStr.Length < 8)
      {
        Logger.AddError(string.Format("Cannot convert {0} to combibation element data at {1}", (object) dataStr, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
      else
      {
        string[] separator = new string[1]{ "END&VAR" };
        string[] strArray = dataStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        this.combinationVariants.Clear();
        for (int index = 0; index < strArray.Length; ++index)
        {
          string dataStr1 = strArray[index];
          if (dataStr1.StartsWith("Probability_"))
          {
            string text = dataStr1.Substring("Probability_".Length);
            try
            {
              this.spawnProbability = StringUtility.ToInt32(text);
            }
            catch (Exception ex)
            {
              Logger.AddError(string.Format("Cannot read spawn probability value: {0}  isn't correct probability value. Error: {1} at {2}", (object) text, (object) ex.Message, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
            }
          }
          else
            this.combinationVariants.Add(new ObjectCombinationVariant(dataStr1));
        }
        this.variantsSumWeight = 0.0f;
        for (int index = 0; index < this.combinationVariants.Count; ++index)
          this.variantsSumWeight += (float) this.combinationVariants[index].Weight;
      }
    }
  }
}
