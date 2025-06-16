using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Types;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMGlobalStorageManager))]
  public class GlobalStorageManager : 
    VMGlobalStorageManager,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    private static int combinationDepth;
    private static List<VMStorage> LinearItemsAddingStorageList = new List<VMStorage>(1000);
    private const int MAX_COMBINATION_DEPTH = 10;
    private const int MAX_ATONCE_LINEAR_ADDING_STORAGES_COUNT = 1000;
    public static double SetContainerTagsDistributionTimeMaxRT = 0.0;
    public static double SetStorageContainerParamsTimeMaxRT = 0.0;
    public static double AddItemsToStoragesLinearTimeMaxRT = 0.0;
    public static double DoResetTagsTimeMaxRT = 0.0;
    public static int MaxLinearAddingStoragesCount = 0;

    public override string GetComponentTypeName() => nameof (GlobalStorageManager);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }

    public override void Initialize(VMBaseEntity parent)
    {
      base.Initialize(parent);
      if (Instance == null)
      {
        Instance = this;
        StorageGroupCommandProcessor = new StorageGroupCommandProcessor();
        AssyncProcessManager.RegistrAssyncUpdateableObject(StorageGroupCommandProcessor);
      }
      else
        Logger.AddError("Global Storage Manager component creation dublicate!");
    }

    public static void ResetInstance()
    {
      Instance = null;
    }

    public override void FreeStorages(
      string storagesRootInfo,
      string storageTags,
      string storageTypes)
    {
      DoFreeStorages(storagesRootInfo, storageTags, storageTypes, false);
    }

    public override void FreeStoragesAssync(
      string storagesRootInfo,
      string storageTags,
      string storageTypes)
    {
      DoFreeStorages(storagesRootInfo, storageTags, storageTypes, true);
    }

    public void DoFreeStorages(
      string storagesRootInfo,
      string storageTags,
      string storageTypes,
      bool assync)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning("Free storages: empty storages guids list");
      }
      else
      {
        List<VMEntity> entityListByRootInfo = GameComponent.GetStorageEntityListByRootInfo(storagesRootInfo);
        OperationTagsInfo operationTagsInfo = new OperationTagsInfo();
        operationTagsInfo.Read(storageTypes);
        OperationMultiTagsInfo operationMultiTagsInfo = new OperationMultiTagsInfo();
        operationMultiTagsInfo.Read(storageTags);
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          VMEntity vmEntity = entityListByRootInfo[index];
          if (vmEntity.Instance == null)
          {
            Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
            break;
          }
          if (vmEntity != null)
          {
            VMCommon entityCommonComponent = vmEntity.EntityCommonComponent;
            if (operationMultiTagsInfo.CheckTag(entityCommonComponent.CustomTag))
            {
              VMStorage storageComponent = vmEntity.EntityStorageComponent;
              if (storageComponent == null)
                Logger.AddError(string.Format("Cannot process storage types filtering in storage object {0}, because storage component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
              else if (operationTagsInfo.CheckTag(storageComponent.Component.Tag) && vmEntity.IsWorldEntity)
              {
                if (vmEntity.Instance == null)
                {
                  Logger.AddError("Storage entity not found, cannot process storage operation at " + DynamicFSM.CurrentStateInfo);
                }
                else
                {
                  StorageCommand storageCommand = new StorageCommand();
                  storageCommand.Initialize(EStorageCommandType.StorageCommandClear, storageComponent, null);
                  StorageGroupCommandProcessor.MakeStorageCommand(storageCommand, assync);
                }
              }
            }
          }
        }
      }
    }

    public override void ResetTags(string storagesRootInfo)
    {
      ((GameComponent) GlobalRootManager).DoResetTags(storagesRootInfo, "Storage");
    }

    public override void SetTagsDistribution(
      string storagesRootInfo,
      string storagTagDistributionInfo,
      string storageTypes)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning(string.Format("Set tags distribution: empty storage operation root info at {0}", DynamicFSM.CurrentStateInfo));
      }
      else
      {
        List<VMEntity> entityListByRootInfo = GameComponent.GetStorageEntityListByRootInfo(storagesRootInfo);
        OperationTagsInfo operationTagsInfo = new OperationTagsInfo();
        operationTagsInfo.Read(storageTypes);
        List<VMEntity> dObjects = new List<VMEntity>();
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          VMEntity vmEntity = entityListByRootInfo[index];
          if (vmEntity != null)
          {
            if (vmEntity.Instance == null)
            {
              Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
              return;
            }
            VMStorage storageComponent = vmEntity.EntityStorageComponent;
            if (storageComponent != null && operationTagsInfo.CheckTag(storageComponent.Component.Tag))
              dObjects.Add(vmEntity);
          }
        }
        ((GameComponent) GlobalRootManager).DoSetTagsDistribution(dObjects, storagTagDistributionInfo);
      }
    }

    public override void SetContainerTagsDistribution(
      string storagesRootInfo,
      string storageTags,
      string containerTypes,
      string storagTagDistributionInfo,
      string storageTypes)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning("Set container tags distribution: empty storage operation root info");
      }
      else
      {
        OperationTagsInfo operationTagsInfo1 = new OperationTagsInfo();
        operationTagsInfo1.Read(storageTypes);
        OperationTagsInfo operationTagsInfo2 = new OperationTagsInfo();
        operationTagsInfo2.Read(containerTypes);
        OperationMultiTagsInfo operationMultiTagsInfo = new OperationMultiTagsInfo();
        operationMultiTagsInfo.Read(storageTags);
        List<VMEntity> entityListByRootInfo = GameComponent.GetStorageEntityListByRootInfo(storagesRootInfo);
        Dictionary<int, KeyValuePair<VMStorage, int>> dictionary = new Dictionary<int, KeyValuePair<VMStorage, int>>();
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          VMEntity vmEntity = entityListByRootInfo[index];
          if (vmEntity.Instance == null)
          {
            Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
            return;
          }
          if (vmEntity != null)
          {
            VMCommon entityCommonComponent = vmEntity.EntityCommonComponent;
            if (entityCommonComponent == null)
              Logger.AddError(string.Format("Object {0} hasn't common component, cannot set inner containers tags at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
            else if (operationMultiTagsInfo.CheckTag(entityCommonComponent.CustomTag))
            {
              VMStorage storageComponent = vmEntity.EntityStorageComponent;
              if (storageComponent == null)
                Logger.AddError(string.Format("Object {0} hasn't storage component, cannot set inner containers tags at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
              else if (operationTagsInfo1.CheckTag(storageComponent.Component.Tag))
              {
                int innerContainersCount = storageComponent.InnerContainersCount;
                for (int containerIndex = 0; containerIndex < innerContainersCount; ++containerIndex)
                {
                  IInventoryComponent innerContainer = storageComponent.GetInnerContainer(containerIndex);
                  if (operationTagsInfo2.CheckTag(innerContainer.Owner.Name))
                  {
                    bool flag = false;
                    do
                    {
                      int randomInt = VMMath.GetRandomInt();
                      if (!dictionary.ContainsKey(randomInt))
                      {
                        dictionary.Add(randomInt, new KeyValuePair<VMStorage, int>(storageComponent, containerIndex));
                        flag = true;
                      }
                    }
                    while (!flag);
                  }
                }
              }
            }
          }
        }
        List<int> list = dictionary.Keys.ToList();
        list.Sort();
        TagDistributionInfo distributionInfo = new TagDistributionInfo();
        distributionInfo.Read(storagTagDistributionInfo);
        float num1 = 0.01f;
        if (!distributionInfo.DistribInPercentage)
        {
          int num2 = 0;
          for (int index = 0; index < distributionInfo.TagInfoList.Count; ++index)
            num2 += distributionInfo.TagInfoList[index].Percentage;
          if (num2 == 0)
          {
            Logger.AddError(string.Format("Distributing tags summ is zero at {0}!", DynamicFSM.CurrentStateInfo));
            return;
          }
          num1 = 1f / num2;
        }
        List<int> intList = new List<int>();
        for (int index = 0; index < distributionInfo.TagInfoList.Count; ++index)
          intList.Add(0);
        for (int index1 = 0; index1 < list.Count; ++index1)
        {
          float num3 = (0.5f + index1) / list.Count;
          float num4 = 0.0f;
          for (int index2 = 0; index2 < distributionInfo.TagInfoList.Count; ++index2)
          {
            float num5 = num4 + num1 * distributionInfo.TagInfoList[index2].Percentage;
            if (num3 > (double) num4 && num3 <= (double) num5)
            {
              if (!distributionInfo.DistribInPercentage)
              {
                if (intList[index2] < distributionInfo.TagInfoList[index2].Percentage)
                {
                  KeyValuePair<VMStorage, int> keyValuePair = dictionary[list[index1]];
                  keyValuePair.Key.SetInnerContainerTag(keyValuePair.Value, distributionInfo.TagInfoList[index2].Tag);
                }
                intList[index2]++;
                break;
              }
              KeyValuePair<VMStorage, int> keyValuePair1 = dictionary[list[index1]];
              keyValuePair1.Key.SetInnerContainerTag(keyValuePair1.Value, distributionInfo.TagInfoList[index2].Tag);
              break;
            }
            num4 = num5;
          }
        }
      }
    }

    public override void SetContainerTagsComplexDistribution(
      string storagesRootInfo,
      string storageTags,
      string containerTypes,
      string storagTagDistributionInfo,
      string storageTypes)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning(string.Format("Set container tags complex distribution: empty operation root info at {0}", DynamicFSM.CurrentStateInfo));
      }
      else
      {
        OperationTagsInfo operationTagsInfo = new OperationTagsInfo();
        operationTagsInfo.Read(storageTypes);
        OperationMultiTagsInfo operationMultiTagsInfo = new OperationMultiTagsInfo();
        operationMultiTagsInfo.Read(storageTags);
        OperationTagsInfo containerTypesInfo = new OperationTagsInfo();
        containerTypesInfo.Read(containerTypes);
        TagDistributionInfo distributionInfo = new TagDistributionInfo();
        distributionInfo.Read(storagTagDistributionInfo);
        if (!distributionInfo.Complex)
        {
          Logger.AddError(string.Format("Invalid container tags complex distribution setting: tsgs distribution info isn't complex at {0}", DynamicFSM.CurrentStateInfo));
        }
        else
        {
          List<VMEntity> entityListByRootInfo = GameComponent.GetStorageEntityListByRootInfo(storagesRootInfo);
          Dictionary<int, VMStorage> dictionary = new Dictionary<int, VMStorage>();
          for (int index = 0; index < entityListByRootInfo.Count; ++index)
          {
            VMEntity vmEntity = entityListByRootInfo[index];
            if (vmEntity.Instance == null)
            {
              Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
              return;
            }
            if (vmEntity != null)
            {
              VMCommon entityCommonComponent = vmEntity.EntityCommonComponent;
              if (entityCommonComponent == null)
                Logger.AddError(string.Format("Cannot process tags filtering in storage object {0}, because common component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
              else if (operationMultiTagsInfo.CheckTag(entityCommonComponent.CustomTag))
              {
                VMStorage storageComponent = vmEntity.EntityStorageComponent;
                if (storageComponent == null)
                  Logger.AddError(string.Format("Cannot process storage types filtering in storage object {0}, because storage component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
                else if (operationTagsInfo.CheckTag(storageComponent.Component.Tag))
                {
                  bool flag = false;
                  do
                  {
                    int randomInt = VMMath.GetRandomInt();
                    if (!dictionary.ContainsKey(randomInt))
                    {
                      dictionary.Add(randomInt, storageComponent);
                      flag = true;
                    }
                  }
                  while (!flag);
                }
              }
            }
          }
          List<int> list = dictionary.Keys.ToList();
          list.Sort();
          float num1 = 0.01f;
          List<int> intList = new List<int>();
          for (int index = 0; index < distributionInfo.TagInfoList.Count; ++index)
            intList.Add(0);
          for (int index1 = 0; index1 < list.Count; ++index1)
          {
            float num2 = (0.5f + index1) / list.Count;
            float num3 = 0.0f;
            for (int index2 = 0; index2 < distributionInfo.TagInfoList.Count; ++index2)
            {
              float num4 = num3 + num1 * distributionInfo.TagInfoList[index2].Percentage;
              if (num2 > (double) num3 && num2 <= (double) num4)
              {
                if (typeof (TagDistributionInfo) != distributionInfo.TagInfoList[index2].GetType())
                {
                  Logger.AddError(string.Format("Invalid complex tags distribution data: distribution part {0} by index {1} must be distribution at {2}", "unknown", index2, DynamicFSM.CurrentStateInfo));
                  break;
                }
                if (!distributionInfo.DistribInPercentage)
                {
                  if (intList[index2] < distributionInfo.TagInfoList[index2].Percentage)
                    SetInnerContainersTagsDistribution(dictionary[list[index1]], containerTypesInfo, (TagDistributionInfo) distributionInfo.TagInfoList[index2]);
                  intList[index2]++;
                  break;
                }
                SetInnerContainersTagsDistribution(dictionary[list[index1]], containerTypesInfo, (TagDistributionInfo) distributionInfo.TagInfoList[index2]);
                break;
              }
              num3 = num4;
            }
          }
        }
      }
    }

    public override void SetStorageContainerParams(
      string storagesRootInfo,
      string storageTags,
      string containerTypes,
      string containerTags,
      string containerParamsData,
      string storageTypes)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning(string.Format("Set storage container params: empty operation root info at {0}", DynamicFSM.CurrentStateInfo));
      }
      else
      {
        try
        {
          OperationTagsInfo operationTagsInfo1 = new OperationTagsInfo();
          operationTagsInfo1.Read(storageTypes);
          OperationMultiTagsInfo operationMultiTagsInfo1 = new OperationMultiTagsInfo();
          operationMultiTagsInfo1.Read(storageTags);
          OperationTagsInfo operationTagsInfo2 = new OperationTagsInfo();
          operationTagsInfo2.Read(containerTypes);
          OperationMultiTagsInfo operationMultiTagsInfo2 = new OperationMultiTagsInfo();
          operationMultiTagsInfo2.Read(containerTags);
          List<VMEntity> entityListByRootInfo = GameComponent.GetStorageEntityListByRootInfo(storagesRootInfo);
          List<KeyValuePair<VMStorage, int>> keyValuePairList = new List<KeyValuePair<VMStorage, int>>();
          for (int index = 0; index < entityListByRootInfo.Count; ++index)
          {
            VMEntity vmEntity = entityListByRootInfo[index];
            if (vmEntity.Instance == null)
            {
              Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
              return;
            }
            if (vmEntity != null)
            {
              VMCommon entityCommonComponent = vmEntity.EntityCommonComponent;
              if (entityCommonComponent == null)
                Logger.AddError(string.Format("Cannot process tags filtering in storage object {0}, because common component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
              else if (operationMultiTagsInfo1.CheckTag(entityCommonComponent.CustomTag))
              {
                VMStorage storageComponent = vmEntity.EntityStorageComponent;
                if (storageComponent == null)
                  Logger.AddError(string.Format("Cannot process storage type filtering in storage object {0}, because storage component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
                else if (operationTagsInfo1.CheckTag(storageComponent.Component.Tag))
                {
                  int innerContainersCount = storageComponent.InnerContainersCount;
                  for (int containerIndex = 0; containerIndex < innerContainersCount; ++containerIndex)
                  {
                    IInventoryComponent innerContainer = storageComponent.GetInnerContainer(containerIndex);
                    string innerContainerTag = storageComponent.GetInnerContainerTag(containerIndex);
                    if (operationTagsInfo2.CheckTag(innerContainer.Owner.Name) && operationMultiTagsInfo2.CheckTag(innerContainerTag))
                      keyValuePairList.Add(new KeyValuePair<VMStorage, int>(storageComponent, containerIndex));
                  }
                }
              }
            }
          }
          IEnumerable<IVariable> byFunctionalName = EngineAPIManager.GetAbstractParametricFunctionsByFunctionalName("Storage", new List<VMType> {
            new VMType(typeof (IBlueprintRef), "Inventory"),
            new VMType(typeof (object))
          });
          Dictionary<string, object> dictionary = ParamsArray.Read(containerParamsData);
          List<string> stringList = new List<string>();
          List<object> objectList = new List<object>();
          foreach (INamed named in byFunctionalName)
          {
            string name = named.Name;
            if (dictionary.ContainsKey(name))
            {
              object obj = dictionary[name];
              objectList.Add(obj);
              if (name.Contains('.'))
                name = name.Split('.')[1];
              stringList.Add(name);
            }
          }
          object[] dFactParams = new object[2];
          for (int index1 = 0; index1 < keyValuePairList.Count; ++index1)
          {
            KeyValuePair<VMStorage, int> keyValuePair = keyValuePairList[index1];
            IInventoryComponent innerContainer = keyValuePair.Key.GetInnerContainer(keyValuePair.Value);
            if (innerContainer != null)
            {
              dFactParams[0] = innerContainer.Owner.Template;
              for (int index2 = 0; index2 < stringList.Count; ++index2)
              {
                string methodName = stringList[index2];
                object obj = objectList[index2];
                dFactParams[1] = obj;
                VMEngineAPIManager.ExecMethodOnComponentDirect(keyValuePair.Key, methodName, dFactParams);
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Set storage container params error: {0} at {1}", ex, DynamicFSM.CurrentStateInfo));
        }
      }
    }

    public override void AddItemsToStorages(
      string storagesRootInfo,
      string storageTags,
      string sMinValue,
      string sMaxValue,
      string itemTemplateGuidStr,
      string storageTypes)
    {
      try
      {
        IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
        int num1 = GameComponent.ReadContextIntParamValue(methodExecInitiator, sMinValue);
        int num2 = GameComponent.ReadContextIntParamValue(methodExecInitiator, sMaxValue);
        OperationTagsInfo operationTagsInfo = new OperationTagsInfo();
        operationTagsInfo.Read(storageTypes);
        OperationMultiTagsInfo operationMultiTagsInfo = new OperationMultiTagsInfo();
        operationMultiTagsInfo.Read(storageTags);
        List<VMEntity> entityListByRootInfo = GameComponent.GetStorageEntityListByRootInfo(storagesRootInfo);
        List<ItemsStorageAddingInfo> storageAddingInfoList = new List<ItemsStorageAddingInfo>();
        float minRand = 0.0f;
        for (int index = 0; index < entityListByRootInfo.Count; ++index)
        {
          VMEntity vmEntity = entityListByRootInfo[index];
          if (vmEntity.Instance == null)
          {
            Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
            return;
          }
          if (vmEntity != null)
          {
            if (vmEntity.IsWorldEntity)
            {
              IEntity instance = vmEntity.Instance;
              if (vmEntity == null)
              {
                Logger.AddError(string.Format("Storage entity not found, cannot process storage operation at {0}!", DynamicFSM.CurrentStateInfo));
              }
              else
              {
                VMCommon componentByName1 = (VMCommon) vmEntity.GetComponentByName("Common");
                if (componentByName1 == null)
                  Logger.AddError(string.Format("Cannot process tags filtering in storage object {0}, because common component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
                else if (operationMultiTagsInfo.CheckTag(componentByName1.CustomTag))
                {
                  VMStorage componentByName2 = (VMStorage) vmEntity.GetComponentByName("Storage");
                  if (componentByName2 == null)
                    Logger.AddError(string.Format("Cannot process storage types filtering in storage object {0}, because storage component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
                  else if (operationTagsInfo.CheckTag(componentByName2.Component.Tag))
                  {
                    float num3 = 1f;
                    float maxRand = minRand + num3;
                    ItemsStorageAddingInfo storageAddingInfo = new ItemsStorageAddingInfo(minRand, maxRand, instance, componentByName2);
                    minRand = maxRand;
                    storageAddingInfoList.Add(storageAddingInfo);
                  }
                }
              }
            }
          }
          else
            Logger.AddError("Storage vm entity not found, cannot process storage operation at " + DynamicFSM.CurrentStateInfo);
        }
        if (storageAddingInfoList.Count == 0)
          return;
        IWorldBlueprint templateByGuidStr = GetTemplateByGuidStr(itemTemplateGuidStr);
        if (templateByGuidStr == null)
          Logger.AddError(string.Format("Template with guid {0} for linear adding not found at {1} !", itemTemplateGuidStr, DynamicFSM.CurrentStateInfo));
        else if (!typeof (IWorldObject).IsAssignableFrom(templateByGuidStr.GetType()))
        {
          Logger.AddError(string.Format("Template guid {0} for linear adding is invalid, this storable template must be world object at {1} !", itemTemplateGuidStr, DynamicFSM.CurrentStateInfo));
        }
        else
        {
          IParam obj = null;
          if (templateByGuidStr != null && templateByGuidStr.IsFunctionalSupport("Combination"))
            obj = templateByGuidStr.GetProperty("Combination", EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_COMBINATION_DATA, typeof (VMCombination)));
          float randomDouble1 = (float) VMMath.GetRandomDouble();
          int num4 = (int) Math.Round(num1 + randomDouble1 * (double) (num2 - num1));
          int itemsToAdd = num4 / storageAddingInfoList.Count;
          if (itemsToAdd < 1)
            itemsToAdd = 1;
          int num5 = 0;
          int randomInt = VMMath.GetRandomInt(storageAddingInfoList.Count);
          for (int index1 = 0; index1 < num4; ++index1)
          {
            int index2 = randomInt + index1;
            if (index2 >= storageAddingInfoList.Count)
              index2 -= storageAddingInfoList.Count;
            if (obj == null)
            {
              float randomDouble2 = (float) VMMath.GetRandomDouble();
              for (int index3 = 0; index3 < storageAddingInfoList.Count; ++index3)
              {
                if (randomDouble2 >= (double) storageAddingInfoList[index3].MinRandInterval && randomDouble2 < (double) storageAddingInfoList[index3].MaxRandInterval)
                {
                  index2 = index3;
                  break;
                }
              }
            }
            if (index2 >= 0)
            {
              IEntity storageEntity = storageAddingInfoList[index2].StorageEntity;
              VMStorage storage1 = storageAddingInfoList[index2].Storage;
              if (obj != null && obj.Value != null && typeof (ObjectCombinationDataStruct) == obj.Value.GetType())
              {
                ObjectCombinationDataStruct combinationData = (ObjectCombinationDataStruct) obj.Value;
                if (combinationData.GetElementsCount() > 0)
                {
                  AddCombinationToStorage(combinationData, storage1, null, null);
                  num5 += itemsToAdd;
                  continue;
                }
              }
              StorageCommand storageCommand = new StorageCommand();
              storageCommand.Initialize(EStorageCommandType.StorageCommandTypeRandomAddItem, storage1, (VMWorldObject) templateByGuidStr, itemsToAdd, num4 - num5);
              int storage2 = StorageGroupCommandProcessor.ProcessRandomAddItemsToStorage(storageCommand);
              num5 += storage2;
            }
            if (num5 >= num4)
              break;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Add items to storages error: {0} at {1}", ex, DynamicFSM.CurrentStateInfo));
      }
    }

    public override void AddItemsToStoragesLinear(
      string storagesRootInfo,
      string storageTags,
      string containerTypes,
      string containerTags,
      string itemTemplateGuidStr,
      string storageTypes)
    {
      if (storagesRootInfo == "0")
      {
        Logger.AddWarning(string.Format("Add items to storages linear: empty storages guids list at {0}", DynamicFSM.CurrentStateInfo));
      }
      else
      {
        try
        {
          OperationTagsInfo operationTagsInfo = new OperationTagsInfo();
          operationTagsInfo.Read(storageTypes);
          OperationMultiTagsInfo operationMultiTagsInfo = new OperationMultiTagsInfo();
          operationMultiTagsInfo.Read(storageTags);
          OperationTagsInfo containerTypes1 = new OperationTagsInfo();
          containerTypes1.Read(containerTypes);
          OperationMultiTagsInfo containerTags1 = new OperationMultiTagsInfo();
          containerTags1.Read(containerTags);
          List<VMEntity> entityListByRootInfo = GameComponent.GetStorageEntityListByRootInfo(storagesRootInfo);
          LinearItemsAddingStorageList.Clear();
          for (int index = 0; index < entityListByRootInfo.Count; ++index)
          {
            VMEntity vmEntity = entityListByRootInfo[index];
            if (vmEntity.Instance == null)
              Logger.AddError(string.Format("Group operation processing: entity {0} not inited in engine at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
            else if (vmEntity != null)
            {
              VMCommon entityCommonComponent = vmEntity.EntityCommonComponent;
              if (entityCommonComponent == null)
                Logger.AddError(string.Format("Cannot process tags filtering in storage object {0}, because component component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
              else if (operationMultiTagsInfo.CheckTag(entityCommonComponent.CustomTag))
              {
                VMStorage storageComponent = vmEntity.EntityStorageComponent;
                if (storageComponent == null)
                  Logger.AddError(string.Format("Cannot process storage types filtering in storage object {0}, because storage component not found at {1}", vmEntity.Name, DynamicFSM.CurrentStateInfo));
                else if (operationTagsInfo.CheckTag(storageComponent.Component.Tag) && vmEntity.IsWorldEntity)
                {
                  IEntity instance = vmEntity.Instance;
                  if (vmEntity == null)
                    Logger.AddError(string.Format("Storage entity not found, cannot process storage operation at {0} !", DynamicFSM.CurrentStateInfo));
                  else
                    LinearItemsAddingStorageList.Add(storageComponent);
                }
              }
            }
            else
              Logger.AddError(string.Format("Storage vm entity not found, cannot process storage operation at {0} !", DynamicFSM.CurrentStateInfo));
          }
          IWorldBlueprint templateByGuidStr = GetTemplateByGuidStr(itemTemplateGuidStr);
          if (templateByGuidStr == null)
            Logger.AddError(string.Format("Template with guid {0} for linear adding not found at {1} !", itemTemplateGuidStr, DynamicFSM.CurrentStateInfo));
          else if (!typeof (IWorldObject).IsAssignableFrom(templateByGuidStr.GetType()))
          {
            Logger.AddError(string.Format("Template guid {0} for linear adding is invalid, this storable template must be world object at {1} !", itemTemplateGuidStr, DynamicFSM.CurrentStateInfo));
          }
          else
          {
            AssyncStorageGroupCommandMode = true;
            IParam obj = null;
            if (templateByGuidStr != null && templateByGuidStr.IsFunctionalSupport("Combination"))
              obj = templateByGuidStr.GetProperty("Combination", EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_COMBINATION_DATA, typeof (VMCombination)));
            for (int index = 0; index < LinearItemsAddingStorageList.Count; ++index)
            {
              VMStorage itemsAddingStorage = LinearItemsAddingStorageList[index];
              if (obj != null)
              {
                if (obj.Value != null && typeof (ObjectCombinationDataStruct) == obj.Value.GetType())
                {
                  ObjectCombinationDataStruct combinationData = (ObjectCombinationDataStruct) obj.Value;
                  if (combinationData.GetElementsCount() > 0)
                    AddCombinationToStorage(combinationData, itemsAddingStorage, containerTypes1, containerTags1);
                }
              }
              else
              {
                StorageCommand storageCommand = new StorageCommand();
                storageCommand.Initialize(EStorageCommandType.StorageCommandTypeAddItem, itemsAddingStorage, (VMWorldObject) templateByGuidStr, 1, containerTypes1, containerTags1);
                StorageGroupCommandProcessor.MakeStorageCommand(storageCommand, AssyncStorageGroupCommandMode);
              }
            }
            AssyncStorageGroupCommandMode = false;
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Add items to storages error: {0} at {1}", ex, DynamicFSM.CurrentStateInfo));
        }
      }
    }

    public override void SetAllStorablesTitle([Template] IEntity template, ITextRef text)
    {
      DoSetAllStorablesTextField(template, text, "title");
    }

    public override void SetAllStorablesDescription([Template] IEntity template, ITextRef text)
    {
      DoSetAllStorablesTextField(template, text, "description");
    }

    public override void SetAllStorablesSpecialDescription([Template] IEntity template, ITextRef text)
    {
      DoSetAllStorablesTextField(template, text, "specialdescription");
    }

    public override void SetAllStorablesTooltip([Template] IEntity template, ITextRef text)
    {
      DoSetAllStorablesTextField(template, text, "tooltip");
    }

    public void DoSetAllStorablesTextField([Template] IEntity template, ITextRef text, string textFieldName)
    {
      if (template == null)
        Logger.AddError("Storable entity for storable text field setting not defined !");
      if (text == null)
        Logger.AddError("text object for storable text field setting not defined !");
      try
      {
        LocalizedText engineTextInstance = EngineAPIManager.CreateEngineTextInstance(text);
        List<VMBaseEntity> vmBaseEntityList = new List<VMBaseEntity>();
        List<VMBaseEntity> allChildEntities1 = VirtualMachine.Instance.WorldHierarchyRootEntity.GetAllChildEntities();
        List<VMBaseEntity> allChildEntities2 = VirtualMachine.Instance.GameRootEntity.GetAllChildEntities();
        if (allChildEntities1 != null)
          vmBaseEntityList.AddRange(allChildEntities1);
        if (allChildEntities2 != null)
          vmBaseEntityList.AddRange(allChildEntities2);
        for (int index = 0; index < vmBaseEntityList.Count; ++index)
        {
          if (vmBaseEntityList[index].Instantiated)
          {
            VMStorage storageComponent = vmBaseEntityList[index].EntityStorageComponent;
            if (storageComponent != null && storageComponent.Component.Items.Count() > 0)
            {
              foreach (IStorableComponent storableComponent in storageComponent.Component.Items)
              {
                if (storableComponent != null && !storableComponent.IsDisposed && storableComponent.Owner.TemplateId == template.Id)
                  VMStorable.DoSetStorableTextField(storableComponent, textFieldName, engineTextInstance);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set storables text field by template {0} error: {1} at {2}", template.Name, ex, DynamicFSM.CurrentStateInfo));
      }
    }

    public override bool IsStorableExistInCombibation(
      IBlueprintRef combination,
      IBlueprintRef storable)
    {
      IBlueprint blueprint1 = null;
      IBlueprint blueprint2 = null;
      if (combination != null && combination.Blueprint != null)
        blueprint1 = combination.Blueprint;
      if (storable != null && storable.Blueprint != null)
        blueprint2 = storable.Blueprint;
      if (blueprint1 == null)
      {
        Logger.AddError(string.Format("Combination for checking storable containing not defined at {0} !", DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (blueprint2 == null)
      {
        Logger.AddError(string.Format("Storable for checking containing in combination not defined at {0} !", DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (blueprint1.IsFunctionalSupport("Combination"))
      {
        IParam property = blueprint1.GetProperty("Combination", EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_COMBINATION_DATA, typeof (VMCombination)));
        if (property != null)
        {
          if (property.Value != null)
          {
            if (typeof (ObjectCombinationDataStruct) == property.Value.GetType())
            {
              ObjectCombinationDataStruct combinationDataStruct = (ObjectCombinationDataStruct) property.Value;
              if (combinationDataStruct.GetElementsCount() > 0)
                return combinationDataStruct.ContainsItem(blueprint2);
            }
            else
              Logger.AddError(string.Format("Combination object {0} for checking item containing not contains Combination data at", blueprint1.Name, DynamicFSM.CurrentStateInfo));
          }
          else
            Logger.AddError(string.Format("Combination object {0} for checking item containing not contains Combination data at", blueprint1.Name, DynamicFSM.CurrentStateInfo));
        }
        else
          Logger.AddError(string.Format("Combination object {0} for checking item containing not contains ObjectCombinationDataStruct param", blueprint1.Name, DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Combination object {0} for checking item containing not contains Combination component at {1}", blueprint1.Name, DynamicFSM.CurrentStateInfo));
      return false;
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      writer.Begin("TemplateRTFieldInfoList", null, true);
      foreach (KeyValuePair<string, ITextRef> keyValuePair in templateRTFieldValuesDict)
      {
        string key = keyValuePair.Key;
        ITextRef textRef = keyValuePair.Value;
        if (textRef != null)
        {
          VMType vmType = new VMType(textRef.GetType());
          writer.Begin("Item", null, true);
          SaveManagerUtility.Save(writer, "FieldKey", key);
          SaveManagerUtility.Save(writer, "FieldType", vmType.Write());
          SaveManagerUtility.SaveCommon(writer, "FieldValue", textRef);
          writer.End("Item", true);
        }
      }
      writer.End("TemplateRTFieldInfoList", true);
      if (StorageGroupCommandProcessor == null)
        return;
      writer.Begin("StorageCommandProcessor", null, true);
      StorageGroupCommandProcessor.StateSave(writer);
      writer.End("StorageCommandProcessor", true);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      templateRTFieldValuesDict.Clear();
      for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
      {
        if (xmlNode.ChildNodes[i1].Name == "TemplateRTFieldInfoList")
        {
          XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i1];
          for (int i2 = 0; i2 < childNode.ChildNodes.Count; ++i2)
          {
            XmlNode firstChild = childNode.ChildNodes[i2].FirstChild;
            string innerText = firstChild.InnerText;
            XmlNode nextSibling = firstChild.NextSibling;
            ITextRef textRef = (ITextRef) StringSerializer.ReadValue(nextSibling.NextSibling.InnerText, VMSaveLoadManager.ReadValue<VMType>(nextSibling).BaseType);
            templateRTFieldValuesDict.Add(innerText, textRef);
            templateRTDataUpdated = true;
          }
        }
        else if (xmlNode.ChildNodes[i1].Name == "StorageCommandProcessor" && StorageGroupCommandProcessor != null)
          StorageGroupCommandProcessor.LoadFromXML((XmlElement) xmlNode.ChildNodes[i1]);
      }
    }

    public override void AfterSaveLoading()
    {
      if (StorageGroupCommandProcessor == null)
        return;
      StorageGroupCommandProcessor.AfterSaveLoading();
    }

    public void AddCombinationToStorage(
      ObjectCombinationDataStruct combinationData,
      VMStorage storage,
      OperationTagsInfo containerTypes,
      OperationMultiTagsInfo containerTags,
      bool dropIfBusy = false)
    {
      ++combinationDepth;
      if (combinationDepth > 10)
      {
        Logger.AddError(string.Format("Items inner combination cycling at {0} at {1}", storage.Parent.Name, DynamicFSM.CurrentStateInfo));
        --combinationDepth;
      }
      else
      {
        for (int elemInd = 0; elemInd < combinationData.GetElementsCount(); ++elemInd)
        {
          ObjectCombinationElement combinationElement = combinationData.GetCombinationElement(elemInd);
          if (combinationElement.SpawnProbability >= 100 || VMMath.GetRandomInt(100) < combinationElement.SpawnProbability)
          {
            try
            {
              if (combinationElement.GetVariantsCount() > 0)
              {
                ObjectCombinationVariant randomVariantByWeight = combinationElement.GetRandomVariantByWeight((float) VMMath.GetRandomDouble());
                if (randomVariantByWeight.MinCount > randomVariantByWeight.MaxCount)
                {
                  Logger.AddError(string.Format("Invalid adding combination to storage {0}, combination variant {1} is invalid: min value={2} and max value={3} at {4}", storage.Parent.Name, randomVariantByWeight, randomVariantByWeight.MinCount, randomVariantByWeight.MaxCount, DynamicFSM.CurrentStateInfo));
                  return;
                }
                int randomInt = VMMath.GetRandomInt(randomVariantByWeight.MinCount, randomVariantByWeight.MaxCount);
                IBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(randomVariantByWeight.ObjectGuid);
                if (engineTemplateByGuid != null)
                {
                  if (typeof (IWorldObject).IsAssignableFrom(engineTemplateByGuid.GetType()))
                  {
                    if (engineTemplateByGuid.IsFunctionalSupport("Combination"))
                    {
                      IParam property = engineTemplateByGuid.GetProperty("Combination", EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_COMBINATION_DATA, typeof (VMCombination)));
                      if (property != null)
                      {
                        if (property.Value != null)
                        {
                          if (typeof (ObjectCombinationDataStruct) == property.Value.GetType())
                          {
                            ObjectCombinationDataStruct combinationData1 = (ObjectCombinationDataStruct) property.Value;
                            for (int index = 0; index < randomInt; ++index)
                              AddCombinationToStorage(combinationData1, storage, containerTypes, containerTags, dropIfBusy);
                          }
                        }
                        else
                          Logger.AddError(string.Format("Invalid inner combination: {0}, combination param value not defined at {1}", engineTemplateByGuid.Name, DynamicFSM.CurrentStateInfo));
                      }
                      else
                        Logger.AddError(string.Format("Invalid inner combination: {0}, combination param not defined at {1}", engineTemplateByGuid.Name, DynamicFSM.CurrentStateInfo));
                    }
                    else
                    {
                      StorageCommand storageCommand = new StorageCommand();
                      storageCommand.Initialize(EStorageCommandType.StorageCommandTypeAddItem, storage, (VMWorldObject) engineTemplateByGuid, randomInt, containerTypes, containerTags, randomVariantByWeight.CIParams, dropIfBusy);
                      StorageGroupCommandProcessor.MakeStorageCommand(storageCommand, AssyncStorageGroupCommandMode);
                    }
                  }
                }
              }
            }
            catch (Exception ex)
            {
              Logger.AddError(string.Format("Add combination to storages error: {0} at {1}", ex, DynamicFSM.CurrentStateInfo));
            }
          }
        }
        --combinationDepth;
      }
    }

    public bool IsStorageOperationsProcessing
    {
      get
      {
        return StorageGroupCommandProcessor != null && StorageGroupCommandProcessor.IsStorageOperationsProcessing;
      }
    }

    private void SetInnerContainersTagsDistribution(
      VMStorage storage,
      OperationTagsInfo containerTypesInfo,
      TagDistributionInfo innerContainersTagDistribution)
    {
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      for (int containerIndex = 0; containerIndex < storage.InnerContainersCount; ++containerIndex)
      {
        IInventoryComponent innerContainer = storage.GetInnerContainer(containerIndex);
        if (containerTypesInfo.CheckTag(innerContainer.Owner.Name))
        {
          bool flag = false;
          do
          {
            int randomInt = VMMath.GetRandomInt();
            if (!dictionary.ContainsKey(randomInt))
            {
              dictionary.Add(randomInt, containerIndex);
              flag = true;
            }
          }
          while (!flag);
        }
      }
      List<int> list = dictionary.Keys.ToList();
      list.Sort();
      float num1 = 0.01f;
      if (!innerContainersTagDistribution.DistribInPercentage)
      {
        int num2 = 0;
        for (int index = 0; index < innerContainersTagDistribution.TagInfoList.Count; ++index)
          num2 += innerContainersTagDistribution.TagInfoList[index].Percentage;
        if (num2 == 0)
        {
          Logger.AddError(string.Format("Distributing tags summ is zero at {0}!", DynamicFSM.CurrentStateInfo));
          return;
        }
        num1 = 1f / num2;
      }
      List<int> intList = new List<int>();
      for (int index = 0; index < innerContainersTagDistribution.TagInfoList.Count; ++index)
        intList.Add(0);
      float num3 = (float) (1.0 / list.Count * VMMath.GetRandomDouble());
      for (int index1 = 0; index1 < list.Count; ++index1)
      {
        float num4 = (0.0f + index1) / list.Count;
        float num5 = 0.0f;
        for (int index2 = 0; index2 < innerContainersTagDistribution.TagInfoList.Count; ++index2)
        {
          float num6 = num5 + num1 * innerContainersTagDistribution.TagInfoList[index2].Percentage;
          float num7 = num4 + num3;
          if (num7 > (double) num5 && num7 <= (double) num6)
          {
            if (!innerContainersTagDistribution.DistribInPercentage)
            {
              if (intList[index2] < innerContainersTagDistribution.TagInfoList[index2].Percentage)
              {
                int containerIndex = dictionary[list[index1]];
                storage.SetInnerContainerTag(containerIndex, innerContainersTagDistribution.TagInfoList[index2].Tag);
              }
              intList[index2]++;
              break;
            }
            int containerIndex1 = dictionary[list[index1]];
            storage.SetInnerContainerTag(containerIndex1, innerContainersTagDistribution.TagInfoList[index2].Tag);
            break;
          }
          num5 = num6;
        }
      }
    }

    public static bool AssyncStorageGroupCommandMode { get; set; }

    private IWorldBlueprint GetTemplateByGuidStr(string itemTemplateGuidStr)
    {
      EGuidFormat guidFormat = GuidUtility.GetGuidFormat(itemTemplateGuidStr);
      if (guidFormat == EGuidFormat.GT_BASE)
        return ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(DefaultConverter.ParseUlong(itemTemplateGuidStr));
      return EGuidFormat.GT_ENGINE == guidFormat ? ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(DefaultConverter.ParseGuid(itemTemplateGuidStr)) : null;
    }

    protected StorageGroupCommandProcessor StorageGroupCommandProcessor { get; private set; }
  }
}
