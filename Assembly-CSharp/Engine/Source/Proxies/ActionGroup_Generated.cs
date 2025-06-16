// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ActionGroup_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ActionGroup))]
  public class ActionGroup_Generated : 
    ActionGroup,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ActionGroup_Generated instance = Activator.CreateInstance<ActionGroup_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ActionGroup_Generated actionGroupGenerated = (ActionGroup_Generated) target2;
      actionGroupGenerated.Name = this.Name;
      actionGroupGenerated.Key = this.Key;
      actionGroupGenerated.Joystick = this.Joystick;
      actionGroupGenerated.JoystickHold = this.JoystickHold;
      actionGroupGenerated.IsChangeble = this.IsChangeble;
      actionGroupGenerated.Hide = this.Hide;
      actionGroupGenerated.Interact = this.Interact;
      actionGroupGenerated.Context = this.Context;
      CloneableObjectUtility.FillListTo<GameActionType>(actionGroupGenerated.Actions, this.Actions);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteEnum<KeyCode>(writer, "Key", this.Key);
      DefaultDataWriteUtility.Write(writer, "Joystick", this.Joystick);
      DefaultDataWriteUtility.Write(writer, "JoystickHold", this.JoystickHold);
      DefaultDataWriteUtility.Write(writer, "IsChangeble", this.IsChangeble);
      DefaultDataWriteUtility.Write(writer, "Hide", this.Hide);
      DefaultDataWriteUtility.Write(writer, "Interact", this.Interact);
      DefaultDataWriteUtility.WriteEnum<ActionGroupContext>(writer, "Context", this.Context);
      DefaultDataWriteUtility.WriteListEnum<GameActionType>(writer, "Actions", this.Actions);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Key = DefaultDataReadUtility.ReadEnum<KeyCode>(reader, "Key");
      this.Joystick = DefaultDataReadUtility.Read(reader, "Joystick", this.Joystick);
      this.JoystickHold = DefaultDataReadUtility.Read(reader, "JoystickHold", this.JoystickHold);
      this.IsChangeble = DefaultDataReadUtility.Read(reader, "IsChangeble", this.IsChangeble);
      this.Hide = DefaultDataReadUtility.Read(reader, "Hide", this.Hide);
      this.Interact = DefaultDataReadUtility.Read(reader, "Interact", this.Interact);
      this.Context = DefaultDataReadUtility.ReadEnum<ActionGroupContext>(reader, "Context");
      this.Actions = DefaultDataReadUtility.ReadListEnum<GameActionType>(reader, "Actions", this.Actions);
    }
  }
}
