// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.RepairableComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RepairableComponent))]
  public class RepairableComponent_Generated : 
    RepairableComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RepairableComponent_Generated instance = Activator.CreateInstance<RepairableComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((RepairableComponent) target2).settings = this.settings;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<RepairableSettings>(writer, "Settings", this.settings);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.settings = UnityDataReadUtility.Read<RepairableSettings>(reader, "Settings", this.settings);
    }
  }
}
