﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerLocationComponent))]
  public class PlayerLocationComponent_Generated : 
    PlayerLocationComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerLocationComponent_Generated instance = Activator.CreateInstance<PlayerLocationComponent_Generated>();
      CopyTo(instance);
      return instance;
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
