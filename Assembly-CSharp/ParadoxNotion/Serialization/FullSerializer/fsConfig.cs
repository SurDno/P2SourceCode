using System;
using System.Reflection;
using UnityEngine;

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
