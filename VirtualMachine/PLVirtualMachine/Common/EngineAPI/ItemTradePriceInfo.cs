using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Common.EngineAPI;

public class ItemTradePriceInfo : ISerializeStateSave, IDynamicLoadSerializable {
	private IWorldBlueprint itemTemplate;
	private float priceValue = 1f;
	private float buyPriceValue = 1f;
	private float priceFactor = 1f;
	private float buyPriceFactor = 1f;

	public ItemTradePriceInfo() { }

	public ItemTradePriceInfo(IWorldBlueprint item) {
		itemTemplate = item;
	}

	public IWorldBlueprint ItemTemplate {
		get => itemTemplate;
		set => itemTemplate = value;
	}

	public float Price {
		get => priceValue;
		set => priceValue = value;
	}

	public float BuyPrice {
		get => buyPriceValue;
		set => buyPriceValue = value;
	}

	public float PriceFactor {
		get => priceFactor;
		set => priceFactor = value;
	}

	public float BuyPriceFactor {
		get => buyPriceFactor;
		set => buyPriceFactor = value;
	}

	public float ResultPrice => priceValue * priceFactor;

	public float ResultBuyPrice => buyPriceValue * buyPriceFactor;

	public void StateSave(IDataWriter writer) {
		if (itemTemplate != null)
			SaveManagerUtility.Save(writer, "TemplateGuid", itemTemplate.BaseGuid);
		SaveManagerUtility.Save(writer, "Price", priceValue);
		SaveManagerUtility.Save(writer, "BuyPrice", buyPriceValue);
		SaveManagerUtility.Save(writer, "PriceFactor", priceFactor);
		SaveManagerUtility.Save(writer, "BuyPriceFactor", buyPriceFactor);
	}

	public void LoadFromXML(XmlElement xmlNode) {
		foreach (XmlElement childNode in xmlNode.ChildNodes)
			if (childNode.Name == "TemplateGuid") {
				var uint64 = StringUtility.ToUInt64(childNode.InnerText);
				var blueprintByGuid = IStaticDataContainer.StaticDataContainer.GameRoot.GetBlueprintByGuid(uint64);
				if (blueprintByGuid != null) {
					if (typeof(IWorldBlueprint).IsAssignableFrom(blueprintByGuid.GetType()))
						itemTemplate = (IWorldBlueprint)blueprintByGuid;
					else
						Logger.AddError(string.Format(
							"SaveLoad error: cannot load item trade price info, item template id {0} is invalid",
							uint64));
				} else
					Logger.AddError(string.Format(
						"SaveLoad error: cannot load item trade price info, item template id {0} not found", uint64));
			} else if (childNode.Name == "Price")
				priceValue = StringUtility.ToSingle(childNode.InnerText);
			else if (childNode.Name == "BuyPrice")
				buyPriceValue = StringUtility.ToSingle(childNode.InnerText);
			else if (childNode.Name == "PriceFactor")
				priceFactor = StringUtility.ToSingle(childNode.InnerText);
			else if (childNode.Name == "BuyPriceFactor")
				buyPriceFactor = StringUtility.ToSingle(childNode.InnerText);
	}

	public bool NeedToSave() {
		return true;
	}
}