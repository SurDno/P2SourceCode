// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer
{
  public class fsConfig
  {
    public System.Type[] SerializeAttributes = new System.Type[2]
    {
      typeof (SerializeField),
      typeof (fsPropertyAttribute)
    };
    public System.Type[] IgnoreSerializeAttributes = new System.Type[2]
    {
      typeof (NonSerializedAttribute),
      typeof (fsIgnoreAttribute)
    };
    public fsMemberSerialization DefaultMemberSerialization = fsMemberSerialization.Default;
    public Func<string, MemberInfo, string> GetJsonNameFromMemberName = (Func<string, MemberInfo, string>) ((name, info) => name);
    public bool SerializeNonAutoProperties = false;
    public bool SerializeNonPublicSetProperties = false;
    public string CustomDateTimeFormatString = (string) null;
    public bool Serialize64BitIntegerAsString = false;
    public bool SerializeEnumsAsInteger = false;
  }
}
