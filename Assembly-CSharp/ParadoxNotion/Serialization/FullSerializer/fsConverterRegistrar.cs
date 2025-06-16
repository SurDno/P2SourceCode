using System;
using System.Collections.Generic;
using System.Reflection;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters;

namespace ParadoxNotion.Serialization.FullSerializer;

public class fsConverterRegistrar {
	public static AnimationCurve_DirectConverter Register_AnimationCurve_DirectConverter;
	public static Bounds_DirectConverter Register_Bounds_DirectConverter;
	public static Gradient_DirectConverter Register_Gradient_DirectConverter;
	public static GUIStyle_DirectConverter Register_GUIStyle_DirectConverter;
	public static GUIStyleState_DirectConverter Register_GUIStyleState_DirectConverter;
	public static Keyframe_DirectConverter Register_Keyframe_DirectConverter;
	public static LayerMask_DirectConverter Register_LayerMask_DirectConverter;
	public static Rect_DirectConverter Register_Rect_DirectConverter;
	public static RectOffset_DirectConverter Register_RectOffset_DirectConverter;
	public static List<Type> Converters = new();

	static fsConverterRegistrar() {
		foreach (var declaredField in typeof(fsConverterRegistrar).GetDeclaredFields())
			if (declaredField.Name.StartsWith("Register_"))
				Converters.Add(declaredField.FieldType);
		foreach (var declaredMethod in typeof(fsConverterRegistrar).GetDeclaredMethods())
			if (declaredMethod.Name.StartsWith("Register_"))
				declaredMethod.Invoke(null, null);
	}
}