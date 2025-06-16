// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.fsMetaProperty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsMetaProperty
  {
    private MemberInfo _memberInfo;

    internal fsMetaProperty(fsConfig config, FieldInfo field)
    {
      this._memberInfo = (MemberInfo) field;
      this.StorageType = field.FieldType;
      this.MemberName = field.Name;
      this.IsPublic = field.IsPublic;
      this.CanRead = true;
      this.CanWrite = true;
      this.CommonInitialize(config);
    }

    internal fsMetaProperty(fsConfig config, PropertyInfo property)
    {
      this._memberInfo = (MemberInfo) property;
      this.StorageType = property.PropertyType;
      this.MemberName = property.Name;
      this.IsPublic = property.GetGetMethod() != (MethodInfo) null && property.GetGetMethod().IsPublic && property.GetSetMethod() != (MethodInfo) null && property.GetSetMethod().IsPublic;
      this.CanRead = property.CanRead;
      this.CanWrite = property.CanWrite;
      this.CommonInitialize(config);
    }

    private void CommonInitialize(fsConfig config)
    {
      fsPropertyAttribute attribute = fsPortableReflection.GetAttribute<fsPropertyAttribute>(this._memberInfo);
      if (attribute != null)
      {
        this.JsonName = attribute.Name;
        this.OverrideConverterType = attribute.Converter;
      }
      if (!string.IsNullOrEmpty(this.JsonName))
        return;
      this.JsonName = config.GetJsonNameFromMemberName(this.MemberName, this._memberInfo);
    }

    public Type StorageType { get; private set; }

    public Type OverrideConverterType { get; private set; }

    public bool CanRead { get; private set; }

    public bool CanWrite { get; private set; }

    public string JsonName { get; private set; }

    public string MemberName { get; private set; }

    public bool IsPublic { get; private set; }

    public void Write(object context, object value)
    {
      FieldInfo memberInfo1 = this._memberInfo as FieldInfo;
      PropertyInfo memberInfo2 = this._memberInfo as PropertyInfo;
      if (memberInfo1 != (FieldInfo) null)
      {
        memberInfo1.SetValue(context, value);
      }
      else
      {
        if (!(memberInfo2 != (PropertyInfo) null))
          return;
        MethodInfo setMethod = memberInfo2.GetSetMethod(true);
        if (setMethod != (MethodInfo) null)
          setMethod.Invoke(context, new object[1]{ value });
      }
    }

    public object Read(object context)
    {
      return this._memberInfo is PropertyInfo ? ((PropertyInfo) this._memberInfo).GetValue(context, new object[0]) : ((FieldInfo) this._memberInfo).GetValue(context);
    }
  }
}
