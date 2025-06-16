// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsDuplicateVersionNameException
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer
{
  public sealed class fsDuplicateVersionNameException : Exception
  {
    public fsDuplicateVersionNameException(Type typeA, Type typeB, string version)
      : base(typeA.ToString() + " and " + (object) typeB + " have the same version string (" + version + "); please change one of them.")
    {
    }
  }
}
