using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;

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
      if (!json.TryGetValue("$type", out fsData fsData))
        return;
      Type type1 = fsTypeCache.GetType(fsData.AsString, storageType);
      if (type1 == null)
      {
        json["missingType"] = new fsData(fsData.AsString);
        json["recoveryState"] = new fsData(data.ToString());
        json["$type"] = new fsData(typeof (TMissing).FullName);
      }
      if (type1 == typeof (TMissing))
      {
        Type type2 = fsTypeCache.GetType(json["missingType"].AsString, storageType);
        if (type2 != null)
        {
          Dictionary<string, fsData> asDictionary = fsJsonParser.Parse(json["recoveryState"].AsString).AsDictionary;
          json = json.Concat(asDictionary.Where(kvp => !json.ContainsKey(kvp.Key))).ToDictionary(c => c.Key, c => c.Value);
          json["$type"] = new fsData(type2.FullName);
          data = new fsData(json);
        }
      }
    }
  }
}
