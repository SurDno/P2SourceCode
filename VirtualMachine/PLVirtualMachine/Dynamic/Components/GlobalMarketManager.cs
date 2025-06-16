using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System.Collections.Generic;
using System.Xml;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMGlobalMarketManager))]
  public class GlobalMarketManager : 
    VMGlobalMarketManager,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    private Dictionary<string, ItemTradePriceInfo> tradePricesTable;
    private bool loaded;

    public override string GetComponentTypeName() => nameof (GlobalMarketManager);

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
      if (VMGlobalMarketManager.Instance == null)
        VMGlobalMarketManager.Instance = (VMGlobalMarketManager) this;
      else
        Logger.AddError(string.Format("Global Market Manager component creation dublicate!"));
    }

    public static void ResetInstance()
    {
      VMGlobalMarketManager.Instance = (VMGlobalMarketManager) null;
    }

    public override void SetBaseItemTradePrices(
      string itemTemplatesNames,
      string itemTemplatesPrices,
      string itemTemplatesBuyPrices)
    {
      if (this.tradePricesTable == null)
      {
        Logger.AddError(string.Format("Market prices table not inited!!!"));
      }
      else
      {
        string[] strArray1 = itemTemplatesNames.Split(';');
        string[] strArray2 = itemTemplatesPrices.Split(';');
        string[] strArray3 = itemTemplatesBuyPrices.Split(';');
        if (strArray1.Length != strArray2.Length)
          Logger.AddError(string.Format("Set prices items count not corresponded to prices count at {0}", (object) DynamicFSM.CurrentStateInfo));
        if (strArray1.Length == 0)
          return;
        IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
        float num1 = 0.0f;
        if ("" != strArray2[0])
          num1 = GameComponent.ReadContextFloatParamValue(methodExecInitiator, strArray2[0]);
        else
          Logger.AddError(string.Format("Cannot set base price for {0}, price not defined ", (object) DynamicFSM.CurrentStateInfo));
        float num2 = 0.0f;
        if ("" != strArray3[0])
          num2 = GameComponent.ReadContextFloatParamValue(methodExecInitiator, strArray3[0]);
        else
          Logger.AddError(string.Format("Cannot set base buy price for {0}, buy price not defined ", (object) DynamicFSM.CurrentStateInfo));
        for (int index = 0; index < strArray1.Length; ++index)
        {
          string key = strArray1[index];
          if (!this.tradePricesTable.ContainsKey(key))
          {
            Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", (object) key, (object) DynamicFSM.CurrentStateInfo));
          }
          else
          {
            this.tradePricesTable[key].Price = num1;
            this.tradePricesTable[key].BuyPrice = num2;
          }
        }
        this.OnModify();
      }
    }

    public override void SetBaseItemTradePriceFactors(
      string itemTemplatesNames,
      string itemTemplatesFactors,
      string itemTemplatesBuyFactors)
    {
      if (this.tradePricesTable == null)
      {
        Logger.AddError(string.Format("Market prices table not inited!!!"));
      }
      else
      {
        string[] strArray1 = itemTemplatesNames.Split(';');
        string[] strArray2 = itemTemplatesFactors.Split(';');
        string[] strArray3 = itemTemplatesBuyFactors.Split(';');
        if (strArray1.Length != strArray2.Length)
          Logger.AddError(string.Format("Set prices items count not corresponded to prices count at {0}", (object) DynamicFSM.CurrentStateInfo));
        if (strArray1.Length == 0)
          return;
        IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
        float num1 = GameComponent.ReadContextFloatParamValue(methodExecInitiator, strArray2[0]);
        float num2 = GameComponent.ReadContextFloatParamValue(methodExecInitiator, strArray3[0]);
        for (int index = 0; index < strArray1.Length; ++index)
        {
          string key = strArray1[index];
          if (!this.tradePricesTable.ContainsKey(key))
          {
            Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", (object) key, (object) DynamicFSM.CurrentStateInfo));
          }
          else
          {
            this.tradePricesTable[key].PriceFactor = num1;
            this.tradePricesTable[key].BuyPriceFactor = num2;
          }
        }
        this.OnModify();
      }
    }

    public override void SetBaseItemTradePrice(IBlueprintRef template, float price)
    {
      if (this.tradePricesTable == null)
      {
        Logger.AddError(string.Format("Market prices table not inited!!!"));
      }
      else
      {
        string name = template.Blueprint.Name;
        if (!this.tradePricesTable.ContainsKey(name))
        {
          Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", (object) name, (object) DynamicFSM.CurrentStateInfo));
        }
        else
        {
          this.tradePricesTable[name].Price = price;
          this.tradePricesTable[name].BuyPrice = price;
          this.OnModify();
        }
      }
    }

    public override void SetBaseItemTradeBuyPrice(
      IBlueprintRef template,
      float price,
      float buyPrice)
    {
      if (this.tradePricesTable == null)
      {
        Logger.AddError(string.Format("Market prices table not inited!!!"));
      }
      else
      {
        string name = template.Blueprint.Name;
        if (!this.tradePricesTable.ContainsKey(name))
        {
          Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", (object) name, (object) DynamicFSM.CurrentStateInfo));
        }
        else
        {
          this.tradePricesTable[name].Price = price;
          this.tradePricesTable[name].BuyPrice = buyPrice;
          this.OnModify();
        }
      }
    }

    public override void SetItemTradePriceFactor(IBlueprintRef template, float priceFactor)
    {
      if (this.tradePricesTable == null)
      {
        Logger.AddError(string.Format("Market prices table not inited!!!"));
      }
      else
      {
        string name = template.Blueprint.Name;
        if (!this.tradePricesTable.ContainsKey(name))
        {
          Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", (object) name, (object) DynamicFSM.CurrentStateInfo));
        }
        else
        {
          this.tradePricesTable[name].PriceFactor = priceFactor;
          this.tradePricesTable[name].BuyPriceFactor = priceFactor;
          this.OnModify();
        }
      }
    }

    public override void SetItemTradeBuyPriceFactor(
      IBlueprintRef template,
      float priceFactor,
      float buyPriceFactor)
    {
      if (this.tradePricesTable == null)
      {
        Logger.AddError(string.Format("Market prices table not inited!!!"));
      }
      else
      {
        string name = template.Blueprint.Name;
        if (!this.tradePricesTable.ContainsKey(name))
        {
          Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", (object) name, (object) DynamicFSM.CurrentStateInfo));
        }
        else
        {
          this.tradePricesTable[name].PriceFactor = priceFactor;
          this.tradePricesTable[name].BuyPriceFactor = buyPriceFactor;
          this.OnModify();
        }
      }
    }

    public override void SetItemStackCount(
      string itemTemplatesNames,
      string itemTemplatesStackCountValues)
    {
    }

    public override float GetCurrentItemTradeGlobalPrice(string templateName, bool buyPrice = false)
    {
      if (this.tradePricesTable == null)
      {
        Logger.AddError(string.Format("Market prices table not inited!!!"));
        return 0.0f;
      }
      string key = templateName;
      if (!this.tradePricesTable.ContainsKey(key))
        key = "Pathologic";
      if (!this.tradePricesTable.ContainsKey(key))
      {
        Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", (object) templateName, (object) DynamicFSM.CurrentStateInfo));
        return 0.0f;
      }
      return buyPrice ? this.tradePricesTable[key].ResultBuyPrice : this.tradePricesTable[key].ResultPrice;
    }

    public void InitTradePricesTable(IEnumerable<IBlueprint> allTemplates)
    {
      if (this.tradePricesTable == null)
        this.tradePricesTable = new Dictionary<string, ItemTradePriceInfo>();
      this.tradePricesTable.Clear();
      this.tradePricesTable.Add("Pathologic", new ItemTradePriceInfo());
      foreach (IBlueprint allTemplate in allTemplates)
      {
        if ((allTemplate.GetCategory() == EObjectCategory.OBJECT_CATEGORY_ITEM || allTemplate.GetCategory() == EObjectCategory.OBJECT_CATEGORY_OTHERS) && allTemplate.IsFunctionalSupport("Storable"))
        {
          ItemTradePriceInfo itemTradePriceInfo = new ItemTradePriceInfo((IWorldBlueprint) allTemplate);
          if (!this.tradePricesTable.ContainsKey(allTemplate.Name))
            this.tradePricesTable.Add(allTemplate.Name, itemTradePriceInfo);
          else
            Logger.AddError(string.Format("Storable name {0} is dublicated !", (object) allTemplate.Name));
        }
      }
      this.loaded = true;
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.SaveDynamicSerializableList<ItemTradePriceInfo>(writer, "TradePricesTable", (IEnumerable<ItemTradePriceInfo>) this.tradePricesTable.Values);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i1];
        if (childNode1.Name == "TradePricesTable")
        {
          this.tradePricesTable = new Dictionary<string, ItemTradePriceInfo>(childNode1.ChildNodes.Count);
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            XmlElement childNode2 = (XmlElement) childNode1.ChildNodes[i2];
            ItemTradePriceInfo itemTradePriceInfo = new ItemTradePriceInfo();
            itemTradePriceInfo.LoadFromXML(childNode2);
            if (itemTradePriceInfo.ItemTemplate != null)
              this.tradePricesTable.Add(itemTradePriceInfo.ItemTemplate.Name, itemTradePriceInfo);
          }
        }
      }
    }

    public override void OnCreate()
    {
      if (this.loaded)
        return;
      this.InitTradePricesTable(((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).Blueprints);
    }

    public override void Clear() => this.loaded = false;
  }
}
