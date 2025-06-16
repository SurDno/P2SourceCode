using Cofe.Meta;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inspectors;

[Initialisable]
public static class UnityInspectedDrawers {
	private static InspectedDrawerService.DrawerHandle defaultObjectDrawer;

	[Initialise]
	private static void Initialise() {
		InspectedDrawerService.Add<Bounds>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var bounds1 = (Bounds)value;
			var bounds2 = drawer.BoundsField(name, bounds1);
			if (!mutable || !(bounds1 != bounds2))
				return;
			value = bounds2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<Vector2>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var vector2_1 = (Vector2)value;
			var vector2_2 = drawer.Vector2Field(name, vector2_1);
			if (!mutable || !(vector2_1 != vector2_2))
				return;
			value = vector2_2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<Vector3>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var vector3_1 = (Vector3)value;
			var vector3_2 = drawer.Vector3Field(name, vector3_1);
			if (!mutable || !(vector3_1 != vector3_2))
				return;
			value = vector3_2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<Vector4>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var vector4_1 = (Vector4)value;
			var vector4_2 = drawer.Vector4Field(name, vector4_1);
			if (!mutable || !(vector4_1 != vector4_2))
				return;
			value = vector4_2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<Quaternion>(
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var eulerAngles = ((Quaternion)value).eulerAngles;
				var euler = drawer.Vector3Field(name, eulerAngles);
				if (!mutable || !(eulerAngles != euler))
					return;
				value = Quaternion.Euler(euler);
				if (setter != null)
					setter(value);
			});
		InspectedDrawerService.Add<Color>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var color1 = (Color)value;
			var color2 = drawer.ColorField(name, color1);
			if (!mutable || !(color1 != color2))
				return;
			value = color2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<Rect>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var rect1 = (Rect)value;
			var rect2 = drawer.RectField(name, rect1);
			if (!mutable || !(rect1 != rect2))
				return;
			value = rect2;
			if (setter != null)
				setter(value);
		});
		InspectedDrawerService.Add<AnimationCurve>(
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var animationCurve1 = (AnimationCurve)value;
				var animationCurve2 = drawer.CurveField(name, animationCurve1);
				if (!mutable || animationCurve1 == animationCurve2)
					return;
				value = animationCurve2;
				if (setter != null)
					setter(value);
			});
		InspectedDrawerService.Add<GradientAlphaKey>(
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var displayName = name;
				var gradientAlphaKey = (GradientAlphaKey)value;
				var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer);
				if (fold.Expand)
					gradientAlphaKey = new GradientAlphaKey(drawer.FloatField("Alpha", gradientAlphaKey.alpha),
						drawer.FloatField("Time", gradientAlphaKey.time));
				InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
			});
		InspectedDrawerService.Add<GradientColorKey>(
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var displayName = name;
				var gradientColorKey = (GradientColorKey)value;
				var fold = InspectedDrawerUtility.BeginComplex(name, displayName, context, drawer);
				if (fold.Expand)
					gradientColorKey = new GradientColorKey(drawer.ColorField("Color", gradientColorKey.color),
						drawer.FloatField("Time", gradientColorKey.time));
				InspectedDrawerUtility.EndComplex(fold, name, context, drawer);
			});
		InspectedDrawerService.Add<Scene>((name, type, value, mutable, context, drawer, target, member, setter) => {
			var path = ((Scene)value).path;
			drawer.TextField(name, path);
		});
		InspectedDrawerService.AddConditional(type => typeof(Object).IsAssignableFrom(type),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var object1 = (Object)value;
				var object2 = drawer.ObjectField(name, object1, type);
				if (!mutable || !(object1 != object2))
					return;
				value = object2;
				if (setter != null)
					setter(value);
			});
	}
}