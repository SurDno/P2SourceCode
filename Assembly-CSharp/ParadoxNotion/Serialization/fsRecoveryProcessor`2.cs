// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.fsRecoveryProcessor`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace ParadoxNotion.Serialization
{
  public class fsRecoveryProcessor<TCanProcess, TMissing> : fsObjectProcessor where TMissing : TCanProcess, IMissingRecoverable
  {
    public override bool CanProcess(Type type) => typeof (TCanProcess).RTIsAssignableFrom(type);

    public override void OnBeforeDeserialize(Type storageType, ref fsData data)
    {
      if (!data.IsDictionary)
        return;
      Dictionary<string, fsData> json = data.AsDictionary;
      fsData fsData;
      if (!json.TryGetValue("$type", out fsData))
        return;
      Type type1 = fsTypeCache.GetType(fsData.AsString, storageType);
      if (type1 == (Type) null)
      {
        json["missingType"] = new fsData(fsData.AsString);
        json["recoveryState"] = new fsData(data.ToString());
        json["$type"] = new fsData(typeof (TMissing).FullName);
      }
      if (type1 == typeof (TMissing))
      {
        Type type2 = fsTypeCache.GetType(json["missingType"].AsString, storageType);
        if (type2 != (Type) null)
        {
          Dictionary<string, fsData> asDictionary = fsJsonParser.Parse(json["recoveryState"].AsString).AsDictionary;
          json = json.Concat<KeyValuePair<string, fsData>>(asDictionary.Where<KeyValuePair<string, fsData>>((Func<KeyValuePair<string, fsData>, bool>) (kvp => !json.ContainsKey(kvp.Key)))).ToDictionary<KeyValuePair<string, fsData>, string, fsData>((Func<KeyValuePair<string, fsData>, string>) (c => c.Key), (Func<KeyValuePair<string, fsData>, fsData>) (c => c.Value));
          json["$type"] = new fsData(type2.FullName);
          data = new fsData(json);
        }
      }
    }
  }
}
