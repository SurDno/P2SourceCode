// Decompiled with JetBrains decompiler
// Type: AmplifyColor.VolumeEffectFieldFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

#nullable disable
namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffectFieldFlags
  {
    public string fieldName;
    public string fieldType;
    public bool blendFlag = false;

    public VolumeEffectFieldFlags(FieldInfo pi)
    {
      this.fieldName = pi.Name;
      this.fieldType = pi.FieldType.FullName;
    }

    public VolumeEffectFieldFlags(VolumeEffectField field)
    {
      this.fieldName = field.fieldName;
      this.fieldType = field.fieldType;
      this.blendFlag = true;
    }
  }
}
