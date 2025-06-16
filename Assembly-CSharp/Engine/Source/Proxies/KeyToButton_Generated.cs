// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.KeyToButton_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using InputServices;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (KeyToButton))]
  public class KeyToButton_Generated : 
    KeyToButton,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      KeyToButton_Generated instance = Activator.CreateInstance<KeyToButton_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      KeyToButton_Generated toButtonGenerated = (KeyToButton_Generated) target2;
      toButtonGenerated.Name = this.Name;
      toButtonGenerated.KeyCode = this.KeyCode;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteEnum<JoystickKeyCode>(writer, "KeyCode", this.KeyCode);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.KeyCode = DefaultDataReadUtility.ReadEnum<JoystickKeyCode>(reader, "KeyCode");
    }
  }
}
