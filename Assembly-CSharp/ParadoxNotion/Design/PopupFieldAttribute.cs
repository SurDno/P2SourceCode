// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Design.PopupFieldAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class PopupFieldAttribute : Attribute
  {
    public object[] values;
    public string staticPath;

    public PopupFieldAttribute(params object[] values) => this.values = values;

    public PopupFieldAttribute(string staticPath) => this.staticPath = staticPath;
  }
}
