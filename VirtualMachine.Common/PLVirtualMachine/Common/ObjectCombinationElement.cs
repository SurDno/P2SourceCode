using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  public class ObjectCombinationElement
  {
    private List<ObjectCombinationVariant> combinationVariants = [];
    private int spawnProbability = 100;
    private float variantsSumWeight;

    public ObjectCombinationElement()
    {
    }

    public ObjectCombinationElement(ObjectCombinationElement cloneElement)
    {
      for (int index = 0; index < cloneElement.combinationVariants.Count; ++index)
        combinationVariants.Add(new ObjectCombinationVariant(cloneElement.combinationVariants[index].ObjectGuid, cloneElement.combinationVariants[index].MinCount, cloneElement.combinationVariants[index].MaxCount)
        {
          Weight = cloneElement.combinationVariants[index].Weight,
          CIParams = new CombinationItemParams(cloneElement.combinationVariants[index].CIParams)
        });
      spawnProbability = cloneElement.spawnProbability;
      variantsSumWeight = cloneElement.variantsSumWeight;
    }

    public ObjectCombinationElement(string dataStr) => Read(dataStr);

    public bool ContainsItem(IBlueprint item)
    {
      for (int index = 0; index < combinationVariants.Count; ++index)
      {
        if (combinationVariants[index].ContainsItem(item))
          return true;
      }
      return false;
    }

    public int GetVariantsCount() => combinationVariants.Count;

    public int SpawnProbability
    {
      get => spawnProbability;
      set => spawnProbability = value;
    }

    public ObjectCombinationVariant GetRandomVariantByWeight(float randVal)
    {
      if (combinationVariants.Count == 0)
        return null;
      float num1 = variantsSumWeight * randVal;
      float num2 = 0.0f;
      for (int index = 0; index < combinationVariants.Count; ++index)
      {
        double num3 = num2;
        num2 += combinationVariants[index].Weight;
        double num4 = num1;
        if (num3 <= num4 && num1 <= (double) num2)
          return combinationVariants[index];
      }
      return combinationVariants[0];
    }

    public void Read(string dataStr)
    {
      if (dataStr.Length == 0)
        return;
      if (dataStr.Length < 8)
      {
        Logger.AddError(string.Format("Cannot convert {0} to combibation element data at {1}", dataStr, EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
      else
      {
        string[] separator = ["END&VAR"];
        string[] strArray = dataStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        combinationVariants.Clear();
        for (int index = 0; index < strArray.Length; ++index)
        {
          string dataStr1 = strArray[index];
          if (dataStr1.StartsWith("Probability_"))
          {
            string text = dataStr1.Substring("Probability_".Length);
            try
            {
              spawnProbability = StringUtility.ToInt32(text);
            }
            catch (Exception ex)
            {
              Logger.AddError(string.Format("Cannot read spawn probability value: {0}  isn't correct probability value. Error: {1} at {2}", text, ex.Message, EngineAPIManager.Instance.CurrentFSMStateInfo));
            }
          }
          else
            combinationVariants.Add(new ObjectCombinationVariant(dataStr1));
        }
        variantsSumWeight = 0.0f;
        for (int index = 0; index < combinationVariants.Count; ++index)
          variantsSumWeight += combinationVariants[index].Weight;
      }
    }
  }
}
