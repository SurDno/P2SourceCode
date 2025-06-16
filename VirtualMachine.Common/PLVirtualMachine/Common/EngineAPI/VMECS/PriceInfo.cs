using System.Xml;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

public class PriceInfo : ISerializeStateSave, IDynamicLoadSerializable {
	private float buyPriceValue = -1f;
	private string templateName = "";

	public PriceInfo() { }

	public PriceInfo(string templateName) {
		this.templateName = templateName;
	}

	public string TemplateName => templateName;

	public float Price { get; set; } = -1f;

	public float BuyPrice {
		get => buyPriceValue;
		set {
			buyPriceValue = value;
			if (buyPriceValue <= 9.9999999747524271E-07 || BuyPriceFactor >= 9.9999999747524271E-07)
				return;
			BuyPriceFactor = 1f;
		}
	}

	public float PriceFactor { get; set; } = 1f;

	public float BuyPriceFactor { get; set; }

	public void StateSave(IDataWriter writer) {
		SaveManagerUtility.Save(writer, "TemplateName", templateName);
		SaveManagerUtility.Save(writer, "Price", Price);
		SaveManagerUtility.Save(writer, "BuyPrice", buyPriceValue);
		SaveManagerUtility.Save(writer, "PriceFactor", PriceFactor);
		SaveManagerUtility.Save(writer, "BuyPriceFactor", BuyPriceFactor);
	}

	public void LoadFromXML(XmlElement xmlNode) {
		foreach (XmlElement childNode in xmlNode.ChildNodes)
			if (childNode.Name == "TemplateName")
				templateName = childNode.InnerText;
			else if (childNode.Name == "Price")
				Price = StringUtility.ToSingle(childNode.InnerText);
			else if (childNode.Name == "BuyPrice")
				buyPriceValue = StringUtility.ToSingle(childNode.InnerText);
			else if (childNode.Name == "PriceFactor")
				PriceFactor = StringUtility.ToSingle(childNode.InnerText);
			else if (childNode.Name == "BuyPriceFactor")
				BuyPriceFactor = StringUtility.ToSingle(childNode.InnerText);
	}
}