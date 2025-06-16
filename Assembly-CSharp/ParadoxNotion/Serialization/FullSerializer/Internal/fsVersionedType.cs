// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.fsVersionedType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public struct fsVersionedType
  {
    public fsVersionedType[] Ancestors;
    public string VersionString;
    public Type ModelType;

    public object Migrate(object ancestorInstance)
    {
      return Activator.CreateInstance(this.ModelType, ancestorInstance);
    }

    public override string ToString()
    {
      return "fsVersionedType [ModelType=" + (object) this.ModelType + ", VersionString=" + this.VersionString + ", Ancestors.Length=" + (object) this.Ancestors.Length + "]";
    }

    public static bool operator ==(fsVersionedType a, fsVersionedType b)
    {
      return a.ModelType == b.ModelType;
    }

    public static bool operator !=(fsVersionedType a, fsVersionedType b)
    {
      return a.ModelType != b.ModelType;
    }

    public override bool Equals(object obj)
    {
      return obj is fsVersionedType fsVersionedType && this.ModelType == fsVersionedType.ModelType;
    }

    public override int GetHashCode() => this.ModelType.GetHashCode();
  }
}
