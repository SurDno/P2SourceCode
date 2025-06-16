// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.Market
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System.Collections.Generic;
using System.Xml;

#nullable disable
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
      SaveManagerUtility.SaveDynamicSerializableList<PriceInfo>(writer, "MarketPriceInfoList", (IEnumerable<PriceInfo>) this.marketPricesTable.Values);
      SaveManagerUtility.SaveDynamicSerializable(writer, "DefaultPriceInfo", (ISerializeStateSave) this.defaultPriceInfo);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i1];
        if (childNode1.Name == "MarketPriceInfoList")
        {
          this.marketPricesTable = new Dictionary<string, PriceInfo>(childNode1.ChildNodes.Count);
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            XmlElement childNode2 = (XmlElement) childNode1.ChildNodes[i2];
            PriceInfo priceInfo = new PriceInfo();
            priceInfo.LoadFromXML(childNode2);
            this.marketPricesTable.Add(priceInfo.TemplateName, priceInfo);
          }
        }
        else if (childNode1.Name == "DefaultPriceInfo")
        {
          this.defaultPriceInfo = new PriceInfo();
          this.defaultPriceInfo.LoadFromXML(childNode1);
        }
      }
    }
  }
}
