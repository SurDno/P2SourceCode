// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.SpeakingComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SpeakingComponent))]
  public class SpeakingComponent_Generated : 
    SpeakingComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      SpeakingComponent_Generated instance = Activator.CreateInstance<SpeakingComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((SpeakingComponent) target2).isEnabled = this.isEnabled;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "SpeakAvailable", this.SpeakAvailable);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      CustomStateSaveUtility.SaveListReferences<ILipSyncObject>(writer, "InitialPhrases", this.initialPhrases);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.SpeakAvailable = DefaultDataReadUtility.Read(reader, "SpeakAvailable", this.SpeakAvailable);
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.initialPhrases = CustomStateLoadUtility.LoadListReferences<ILipSyncObject>(reader, "InitialPhrases", this.initialPhrases);
    }
  }
}
