using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WindowTargetAbilityProjectile))]
  public class WindowTargetAbilityProjectile_Generated : 
    WindowTargetAbilityProjectile,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      WindowTargetAbilityProjectile_Generated instance = Activator.CreateInstance<WindowTargetAbilityProjectile_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
    }

    public void DataWrite(IDataWriter writer)
    {
    }

    public void DataRead(IDataReader reader, Type type)
    {
    }
  }
}
