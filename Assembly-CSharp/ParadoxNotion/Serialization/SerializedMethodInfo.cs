// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.SerializedMethodInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Serialization.FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace ParadoxNotion.Serialization
{
  [Serializable]
  public class SerializedMethodInfo : ISerializationCallbackReceiver
  {
    [SerializeField]
    private string _returnInfo;
    [SerializeField]
    private string _baseInfo;
    [SerializeField]
    private string _paramsInfo;
    [NonSerialized]
    private MethodInfo _method;
    [NonSerialized]
    private bool _hasChanged;

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
      this._hasChanged = false;
      if (!(this._method != (MethodInfo) null))
        return;
      this._returnInfo = this._method.ReturnType.FullName;
      this._baseInfo = string.Format("{0}|{1}", (object) this._method.RTReflectedType().FullName, (object) this._method.Name);
      this._paramsInfo = string.Join("|", ((IEnumerable<ParameterInfo>) this._method.GetParameters()).Select<ParameterInfo, string>((Func<ParameterInfo, string>) (p => p.ParameterType.FullName)).ToArray<string>());
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      this._hasChanged = false;
      string[] strArray1 = this._baseInfo.Split('|');
      System.Type type1 = fsTypeCache.GetType(strArray1[0], (System.Type) null);
      if (type1 == (System.Type) null)
      {
        this._method = (MethodInfo) null;
      }
      else
      {
        string name = strArray1[1];
        string[] strArray2;
        if (!string.IsNullOrEmpty(this._paramsInfo))
          strArray2 = this._paramsInfo.Split('|');
        else
          strArray2 = (string[]) null;
        string[] source = strArray2;
        System.Type[] typeArray = source == null ? new System.Type[0] : ((IEnumerable<string>) source).Select<string, System.Type>((Func<string, System.Type>) (n => fsTypeCache.GetType(n, (System.Type) null))).ToArray<System.Type>();
        if (((IEnumerable<System.Type>) typeArray).All<System.Type>((Func<System.Type, bool>) (t => t != (System.Type) null)))
        {
          this._method = type1.RTGetMethod(name, typeArray);
          if (!string.IsNullOrEmpty(this._returnInfo))
          {
            System.Type type2 = fsTypeCache.GetType(this._returnInfo, (System.Type) null);
            if (this._method != (MethodInfo) null && type2 != this._method.ReturnType)
              this._method = (MethodInfo) null;
          }
        }
        if (!(this._method == (MethodInfo) null))
          return;
        this._hasChanged = true;
        this._method = ((IEnumerable<MethodInfo>) type1.RTGetMethods()).FirstOrDefault<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == name));
      }
    }

    public SerializedMethodInfo()
    {
    }

    public SerializedMethodInfo(MethodInfo method)
    {
      this._hasChanged = false;
      this._method = method;
    }

    public MethodInfo Get() => this._method;

    public bool HasChanged() => this._hasChanged;

    public string GetMethodString()
    {
      return string.Format("{0} ({1})", (object) this._baseInfo.Replace("|", "."), (object) this._paramsInfo.Replace("|", ", "));
    }
  }
}
