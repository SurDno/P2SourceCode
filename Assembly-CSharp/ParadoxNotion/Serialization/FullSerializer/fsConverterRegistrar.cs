// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsConverterRegistrar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Serialization.FullSerializer.Internal;
using ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer
{
  public class fsConverterRegistrar
  {
    public static AnimationCurve_DirectConverter Register_AnimationCurve_DirectConverter;
    public static Bounds_DirectConverter Register_Bounds_DirectConverter;
    public static Gradient_DirectConverter Register_Gradient_DirectConverter;
    public static GUIStyle_DirectConverter Register_GUIStyle_DirectConverter;
    public static GUIStyleState_DirectConverter Register_GUIStyleState_DirectConverter;
    public static Keyframe_DirectConverter Register_Keyframe_DirectConverter;
    public static LayerMask_DirectConverter Register_LayerMask_DirectConverter;
    public static Rect_DirectConverter Register_Rect_DirectConverter;
    public static RectOffset_DirectConverter Register_RectOffset_DirectConverter;
    public static List<Type> Converters = new List<Type>();

    static fsConverterRegistrar()
    {
      foreach (FieldInfo declaredField in typeof (fsConverterRegistrar).GetDeclaredFields())
      {
        if (declaredField.Name.StartsWith("Register_"))
          fsConverterRegistrar.Converters.Add(declaredField.FieldType);
      }
      foreach (MethodInfo declaredMethod in typeof (fsConverterRegistrar).GetDeclaredMethods())
      {
        if (declaredMethod.Name.StartsWith("Register_"))
          declaredMethod.Invoke((object) null, (object[]) null);
      }
    }
  }
}
