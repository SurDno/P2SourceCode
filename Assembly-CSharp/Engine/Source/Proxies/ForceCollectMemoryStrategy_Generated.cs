// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ForceCollectMemoryStrategy_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ForceCollectMemoryStrategy))]
  public class ForceCollectMemoryStrategy_Generated : 
    ForceCollectMemoryStrategy,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ForceCollectMemoryStrategy_Generated instance = Activator.CreateInstance<ForceCollectMemoryStrategy_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((ForceCollectMemoryStrategy) target2).disableGC = this.disableGC;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "DisableGC", this.disableGC);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.disableGC = DefaultDataReadUtility.Read(reader, "DisableGC", this.disableGC);
    }
  }
}
