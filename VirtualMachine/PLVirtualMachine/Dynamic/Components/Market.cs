using System.Collections.Generic;
using System.Xml;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMMarket))]
  public class Market : VMMarket, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Market);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.SaveDynamicSerializableList(writer, "MarketPriceInfoList", marketPricesTable.Values);
      SaveManagerUtility.SaveDynamicSerializable(writer, "DefaultPriceInfo", defaultPriceInfo);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i1];
        if (childNode1.Name == "MarketPriceInfoList")
        {
          marketPricesTable = new Dictionary<string, PriceInfo>(childNode1.ChildNodes.Count);
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            XmlElement childNode2 = (XmlElement) childNode1.ChildNodes[i2];
            PriceInfo priceInfo = new PriceInfo();
            priceInfo.LoadFromXML(childNode2);
            marketPricesTable.Add(priceInfo.TemplateName, priceInfo);
          }
        }
        else if (childNode1.Name == "DefaultPriceInfo")
        {
          defaultPriceInfo = new PriceInfo();
          defaultPriceInfo.LoadFromXML(childNode1);
        }
      }
    }
  }
}
