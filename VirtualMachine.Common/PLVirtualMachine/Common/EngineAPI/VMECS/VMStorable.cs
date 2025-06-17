using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Storable", typeof (IStorableComponent))]
  public class VMStorable : VMEngineComponent<IStorableComponent>
  {
    public const string ComponentName = "Storable";
    private ITextRef objectTooltip;
    private ITextRef objectDescription;
    private ITextRef objectTitle;
    private ITextRef objectSpecialDescription;
    private string storeTag;
    private static Dictionary<string, IEntity> StorableTemplateNamesDict = new();

    [Property("Storable class", "", true, "Pathologic")]
    public string StorableClass { get; set; }

    [Property("Tag", "", true)]
    public string StoreTag
    {
      get => storeTag;
      set
      {
        if (value == null)
          return;
        storeTag = value;
      }
    }

    [Property("Default stack count", "", true, 1)]
    public int DefaultStackCount
    {
      get => Component.Max;
      set => Component.Max = value;
    }

    public static IStorableComponent MakeStorableByTemplate(
      VMBaseEntity storageEntity,
      IEntity template)
    {
      if (template.GetComponent<IStorableComponent>() == null)
      {
        Logger.AddError(string.Format("Invalid storable object template {0}: there is no storable component!", template.Name));
        return null;
      }
      try
      {
        if (!StorableTemplateNamesDict.ContainsKey(template.Name))
          StorableTemplateNamesDict.Add(template.Name, template);
        IStorableComponent component = EngineAPIManager.Instance.CreateWorldTemplateDynamicChildInstance(template, null, ServiceCache.Simulation.Storables).Instance.GetComponent<IStorableComponent>();
        Invoice invoice;
        invoice.SellPrice = 0.0f;
        invoice.BuyPrice = VMMarket.DEFAULT_ITEM_PRICE;
        component.Invoice = invoice;
        return component;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set storable {0} market data error: {1}", template.Name, ex));
      }
      return null;
    }

    public static void MakeStorableByInstance(
      VMStorage storage,
      IStorableComponent storable,
      IEntity template)
    {
      IEntity owner = storable.Owner;
      if (template.GetComponent<IStorableComponent>() == null)
        Logger.AddError(string.Format("Invalid storable object template {0}: there is no storable component!", template.Name));
      else
        EngineAPIManager.Instance.CreateWorldTemplateDynamicChildInstance(template, owner);
    }

    public static IEntity GetEngTemplateByStorableName(string storableName)
    {
      return StorableTemplateNamesDict.ContainsKey(storableName) ? StorableTemplateNamesDict[storableName] : null;
    }

    [Property("Title", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_STORABLE_TITLE)]
    public ITextRef Title
    {
      get => objectTitle;
      set
      {
        objectTitle = value;
        Component.Title = EngineAPIManager.CreateEngineTextInstance(objectTitle);
      }
    }

    [Property("Tooltip", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_STORABLE_TOOLTIP)]
    public ITextRef Tooltip
    {
      get => objectTooltip;
      set
      {
        objectTooltip = value;
        Component.Tooltip = EngineAPIManager.CreateEngineTextInstance(objectTooltip);
      }
    }

    [Property("Description", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_STORABLE_DESCRIPTION)]
    public ITextRef Description
    {
      get => objectDescription;
      set
      {
        objectDescription = value;
        Component.Description = EngineAPIManager.CreateEngineTextInstance(objectDescription);
      }
    }

    [Property("SpecialDescription", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_STORABLE_SPECIALDESCRIPTION)]
    public ITextRef SpecialDescription
    {
      get => objectSpecialDescription;
      set
      {
        objectSpecialDescription = value;
        Component.SpecialDescription = EngineAPIManager.CreateEngineTextInstance(objectSpecialDescription);
      }
    }

    public static void DoSetStorableTextField(
      IStorableComponent item,
      string textFieldName,
      LocalizedText textInstance)
    {
      switch (textFieldName)
      {
        case "title":
          item.Title = textInstance;
          break;
        case "description":
          item.Description = textInstance;
          break;
        case "specialdescription":
          item.SpecialDescription = textInstance;
          break;
        case "tooltip":
          item.Tooltip = textInstance;
          break;
      }
    }

    public static void ClearAll() => StorableTemplateNamesDict.Clear();
  }
}
