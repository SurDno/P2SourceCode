using System.Collections.Generic;
using System.Linq;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Market", typeof(IMarketComponent))]
[Depended("Storage")]
public class VMMarket : VMEngineComponent<IMarketComponent> {
	public const string ComponentName = "Market";
	protected Dictionary<string, PriceInfo> marketPricesTable = new();
	protected PriceInfo defaultPriceInfo;
	public static float DEFAULT_ITEM_PRICE;

	[Property("Enabled", "", false, true)]
	public bool MarketEnabled {
		get => Component.IsEnabled;
		set => Component.IsEnabled = value;
	}

	[Method("", ",,", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_SET_MULTI_STORABLES_FIXED_PRICES)]
	public void SetMultiStorablesFixedPrices(
		string itemTemplatesNames,
		string priceValuesInfo,
		string buyPriceValuesInfo) {
		var strArray1 = itemTemplatesNames.Split(';');
		var strArray2 = priceValuesInfo.Split(';');
		var strArray3 = buyPriceValuesInfo.Split(';');
		if (strArray1.Length == 0)
			return;
		var instance = VMGameComponent.Instance;
		var priceValue = instance.ReadFloatParamValue(strArray2[0]);
		var buyPriceValue = instance.ReadFloatParamValue(strArray3[0]);
		for (var index = 0; index < strArray1.Length; ++index)
			SetStorablePriceByTemplateName(strArray1[index], priceValue, buyPriceValue);
	}

	[Method("", ",,", "")]
	public void SetMultiStorablesPricesFactor(
		string itemTemplatesNames,
		string sPriceFactorValues,
		string sBuyPriceFactorValues) {
		var strArray1 = itemTemplatesNames.Split(';');
		var strArray2 = sPriceFactorValues.Split(';');
		var strArray3 = sBuyPriceFactorValues.Split(';');
		if (strArray1.Length == 0)
			return;
		var instance = VMGameComponent.Instance;
		var priceFactorValue = instance.ReadFloatParamValue(strArray2[0]);
		var buyPriceFactorValue = instance.ReadFloatParamValue(strArray3[0]);
		for (var index = 0; index < strArray1.Length; ++index)
			SetStorablePriceFactorByTemplateName(strArray1[index], priceFactorValue, buyPriceFactorValue);
	}

	public void SetStorablePriceByTemplateName(
		string storableTemplateName,
		float priceValue,
		float buyPriceValue) {
		if ("Pathologic" == storableTemplateName) {
			if (defaultPriceInfo == null)
				defaultPriceInfo = new PriceInfo();
			defaultPriceInfo.Price = priceValue;
			defaultPriceInfo.BuyPrice = buyPriceValue;
		}

		if (marketPricesTable.ContainsKey(storableTemplateName)) {
			var priceInfo = marketPricesTable[storableTemplateName];
			priceInfo.Price = priceValue;
			priceInfo.BuyPrice = buyPriceValue;
		} else
			marketPricesTable.Add(storableTemplateName, new PriceInfo(storableTemplateName) {
				Price = priceValue,
				BuyPrice = buyPriceValue
			});

		OnModify();
	}

	public void SetStorablePriceFactorByTemplateName(
		string storableTemplateName,
		float priceFactorValue,
		float buyPriceFactorValue) {
		if ("Pathologic" == storableTemplateName) {
			if (defaultPriceInfo == null)
				defaultPriceInfo = new PriceInfo();
			defaultPriceInfo.PriceFactor = priceFactorValue;
			defaultPriceInfo.BuyPriceFactor = buyPriceFactorValue;
		}

		if (marketPricesTable.ContainsKey(storableTemplateName)) {
			var priceInfo = marketPricesTable[storableTemplateName];
			priceInfo.PriceFactor = priceFactorValue;
			priceInfo.BuyPriceFactor = buyPriceFactorValue;
		} else
			marketPricesTable.Add(storableTemplateName, new PriceInfo(storableTemplateName) {
				PriceFactor = priceFactorValue,
				BuyPriceFactor = buyPriceFactorValue
			});

		OnModify();
	}

	public PriceInfo GetTraderPriceInfoByTemplateName(string templateName) {
		if (marketPricesTable.ContainsKey(templateName))
			return marketPricesTable[templateName];
		return defaultPriceInfo != null ? defaultPriceInfo : new PriceInfo(templateName);
	}

	[Method("Set fixed price", "Template:Storable,Price", "")]
	public void SetStorablePriceByTemplate([Template] IEntity template, float priceValue) {
		if (template == null)
			Logger.AddError(string.Format("Storable template for market {0} price setting is not defined !",
				Component.Owner.Name));
		else
			SetStorablePriceByTemplateName(template.Name, priceValue, priceValue);
	}

	[Method("Set fixed prices", "Template:Storable,Price,BuyPrice", "")]
	public void SetStorablePricesByTemplate(
		[Template] IEntity template,
		float priceValue,
		float buyPriceValue) {
		if (template == null)
			Logger.AddError(string.Format("Storable template for market {0} price setting is not defined !",
				Component.Owner.Name));
		else
			SetStorablePriceByTemplateName(template.Name, priceValue, buyPriceValue);
	}

	private void FillPrices() {
		FillPrices(this, Component.Owner.GetComponent<IStorageComponent>());
		FillPrices(this, ServiceCache.Simulation.Player.GetComponent<IStorageComponent>());
	}

	public static void FillPrices(VMMarket market, IStorageComponent storage) {
		for (var index = 0; index < storage.Items.Count(); ++index) {
			var storableComponent = storage.Items.ElementAt(index);
			if (storableComponent != null && !storableComponent.IsDisposed) {
				var name = storableComponent.Owner.Name;
				var itemTradePrice = CalculateItemTradePrice(market, name);
				storableComponent.Invoice = new Invoice {
					BuyPrice = itemTradePrice.BuyPrice,
					SellPrice = itemTradePrice.Price
				};
			}
		}
	}

	private static PriceInfo CalculateItemTradePrice(VMMarket market, string templateName) {
		var infoByTemplateName = market.GetTraderPriceInfoByTemplateName(templateName);
		var num1 = infoByTemplateName.Price;
		var num2 = infoByTemplateName.BuyPrice;
		if (infoByTemplateName.Price < -9.9999999747524271E-07)
			num1 = VMGlobalMarketManager.Instance.GetCurrentItemTradeGlobalPrice(templateName);
		if (infoByTemplateName.BuyPrice > -9.9999999747524271E-07 ||
		    infoByTemplateName.BuyPriceFactor > 9.9999999747524271E-07) {
			if (infoByTemplateName.BuyPrice < -9.9999999747524271E-07)
				num2 = VMGlobalMarketManager.Instance.GetCurrentItemTradeGlobalPrice(templateName, true);
		} else
			num2 = 0.0f;

		return new PriceInfo(templateName) {
			Price = num1 * infoByTemplateName.PriceFactor,
			BuyPrice = num2 * infoByTemplateName.BuyPriceFactor
		};
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.OnFillPrices -= FillPrices;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.OnFillPrices += FillPrices;
	}
}