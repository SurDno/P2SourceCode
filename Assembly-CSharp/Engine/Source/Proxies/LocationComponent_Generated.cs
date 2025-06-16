// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.LocationComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Locations;
using Engine.Source.Components;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LocationComponent))]
  public class LocationComponent_Generated : 
    LocationComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      LocationComponent_Generated instance = Activator.CreateInstance<LocationComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((LocationComponent) target2).locationType = this.locationType;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<LocationType>(writer, "LocationType", this.locationType);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.locationType = DefaultDataReadUtility.ReadEnum<LocationType>(reader, "LocationType");
    }
  }
}
