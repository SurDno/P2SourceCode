using System;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  public class StorageCommand : 
    IStorageCommand,
    ISerializeStateSave,
    IDynamicLoadSerializable,
    INeedSave
  {
    private Guid targetStorageEntityLoadedId = Guid.Empty;

    public void Initialize(
      EStorageCommandType commandType,
      VMStorage storage,
      VMWorldObject itemLogicObject,
      bool dropIfBusy = false)
    {
      Initialize(commandType, storage, itemLogicObject, 1, null, null, dropIfBusy: dropIfBusy);
    }

    public void Initialize(
      EStorageCommandType commandType,
      VMStorage storage,
      VMWorldObject itemLogicObject,
      int itemsToAdd,
      OperationTagsInfo containerTypes,
      OperationMultiTagsInfo containerTags,
      CombinationItemParams ciParams = null,
      bool dropIfBusy = false)
    {
      StorageCommandType = commandType;
      TargetStorage = storage;
      TargetItemTemplate = itemLogicObject;
      ItemsCount = itemsToAdd;
      MaxItemsCount = itemsToAdd;
      ContainerTypesInfo = containerTypes;
      ContainerTagsInfo = containerTags;
      CombinationParams = ciParams;
      DropIfBusyMode = dropIfBusy;
    }

    public void Initialize(
      EStorageCommandType commandType,
      VMStorage storage,
      VMWorldObject itemLogicObject,
      int itemsToAdd,
      int maxItemsToAdd)
    {
      StorageCommandType = commandType;
      TargetStorage = storage;
      TargetItemTemplate = itemLogicObject;
      ItemsCount = itemsToAdd;
      MaxItemsCount = maxItemsToAdd;
      ContainerTypesInfo = null;
      ContainerTagsInfo = null;
      CombinationParams = null;
      DropIfBusyMode = false;
    }

    public bool NeedSave => TargetStorage != null && TargetStorage.Parent != null && !TargetStorage.Parent.IsDisposed;

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.SaveEnum(writer, "StorageCommandType", StorageCommandType);
      if (TargetStorage != null)
        SaveManagerUtility.Save(writer, "TargetStorage", TargetStorage.Parent.EngineGuid);
      if (TargetItemTemplate != null)
        SaveManagerUtility.Save(writer, "TargetItemTemplate", TargetItemTemplate.BaseGuid);
      SaveManagerUtility.Save(writer, "ItemsCount", ItemsCount);
      SaveManagerUtility.Save(writer, "MaxItemsCount", MaxItemsCount);
      if (ContainerTypesInfo != null)
        SaveManagerUtility.Save(writer, "ContainerTypesInfo", ContainerTypesInfo.ToString());
      if (ContainerTagsInfo != null)
        SaveManagerUtility.Save(writer, "ContainerTagsInfo", ContainerTagsInfo.ToString());
      SaveManagerUtility.SaveDynamicSerializable(writer, "CombinationParams", CombinationParams);
      SaveManagerUtility.Save(writer, "DropIfBusyMode", DropIfBusyMode);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "StorageCommandType")
          StorageCommandType = VMSaveLoadManager.ReadEnum<EStorageCommandType>(childNode);
        else if (childNode.Name == "TargetStorage")
          targetStorageEntityLoadedId = VMSaveLoadManager.ReadGuid(childNode);
        else if (childNode.Name == "TargetItemTemplate")
        {
          ulong id = VMSaveLoadManager.ReadUlong(childNode);
          if (id != 0UL)
          {
            IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(id);
            if (objectByGuid != null)
              TargetItemTemplate = (VMWorldObject) objectByGuid;
          }
        }
        else if (childNode.Name == "ItemsCount")
          ItemsCount = VMSaveLoadManager.ReadInt(childNode);
        else if (childNode.Name == "MaxItemsCount")
          MaxItemsCount = VMSaveLoadManager.ReadInt(childNode);
        else if (childNode.Name == "DropIfBusyMode")
          DropIfBusyMode = VMSaveLoadManager.ReadBool(childNode);
        else if (childNode.Name == "ContainerTypesInfo")
        {
          string innerText = childNode.InnerText;
          ContainerTypesInfo = new OperationTagsInfo();
          ContainerTypesInfo.Read(innerText);
        }
        else if (childNode.Name == "ContainerTagsInfo")
        {
          string innerText = childNode.InnerText;
          ContainerTagsInfo = new OperationMultiTagsInfo();
          ContainerTagsInfo.Read(innerText);
        }
        else if (childNode.Name == "CombinationParams")
        {
          CombinationParams = new CombinationItemParams();
          CombinationParams.LoadFromXML(childNode);
        }
      }
    }

    public void AfterSaveLoading()
    {
      if (!(targetStorageEntityLoadedId != Guid.Empty))
        return;
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(targetStorageEntityLoadedId);
      if (entityByEngineGuid != null)
      {
        VMComponent componentByName = entityByEngineGuid.GetComponentByName("Storage");
        if (componentByName != null)
          TargetStorage = (VMStorage) componentByName;
        else
          Logger.AddError(string.Format("SaveLoad error: storage component not found in target storage entity {0} at storage group command loading", entityByEngineGuid.Name));
      }
      else
        Logger.AddError(string.Format("SaveLoad error: target storage entity id={0} not found at storage group command loading", targetStorageEntityLoadedId));
    }

    public void Clear()
    {
      TargetStorage = null;
      TargetItemTemplate = null;
      ContainerTypesInfo = null;
      ContainerTagsInfo = null;
      CombinationParams = null;
    }

    public EStorageCommandType StorageCommandType { get; private set; }

    public VMStorage TargetStorage { get; private set; }

    public VMWorldObject TargetItemTemplate { get; private set; }

    public int ItemsCount { get; private set; }

    public int MaxItemsCount { get; private set; }

    public OperationTagsInfo ContainerTypesInfo { get; private set; }

    public OperationMultiTagsInfo ContainerTagsInfo { get; private set; }

    public CombinationItemParams CombinationParams { get; private set; }

    public bool DropIfBusyMode { get; private set; }
  }
}
