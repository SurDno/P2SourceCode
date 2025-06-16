using System;
using System.Collections.Generic;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Source.Connections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Tools.Serializations.Converters;

public static class UnityDataWriteUtility {
	public static void Write(IDataWriter writer, string name, Vector2 value) {
		writer.Begin(name, null, false);
		writer.Write(UnityConverter.ToString(value));
		writer.End(name, false);
	}

	public static void Write(IDataWriter writer, string name, Vector3 value) {
		writer.Begin(name, null, false);
		writer.Write(UnityConverter.ToString(value));
		writer.End(name, false);
	}

	public static void Write(IDataWriter writer, string name, Vector4 value) {
		writer.Begin(name, null, false);
		writer.Write(UnityConverter.ToString(value));
		writer.End(name, false);
	}

	public static void Write(IDataWriter writer, string name, Quaternion value) {
		writer.Begin(name, null, false);
		writer.Write(UnityConverter.ToString(value));
		writer.End(name, false);
	}

	public static void Write(IDataWriter writer, string name, Color value) {
		writer.Begin(name, null, false);
		writer.Write(UnityConverter.ToString(value));
		writer.End(name, false);
	}

	public static void Write(IDataWriter writer, string name, LayerMask value) {
		writer.Begin(name, null, false);
		writer.Write(DefaultConverter.ToString(value.value));
		writer.End(name, false);
	}

	public static void Write(IDataWriter writer, string name, GradientAlphaKey value) {
		writer.Begin(name, null, true);
		DefaultDataWriteUtility.Write(writer, "Alpha", value.alpha);
		DefaultDataWriteUtility.Write(writer, "Time", value.time);
		writer.End(name, true);
	}

	public static void Write(IDataWriter writer, string name, GradientColorKey value) {
		writer.Begin(name, null, true);
		DefaultDataWriteUtility.Write(writer, "R", value.color.r);
		DefaultDataWriteUtility.Write(writer, "G", value.color.g);
		DefaultDataWriteUtility.Write(writer, "B", value.color.b);
		DefaultDataWriteUtility.Write(writer, "A", value.color.a);
		DefaultDataWriteUtility.Write(writer, "Time", value.time);
		writer.End(name, true);
	}

	public static void Write<T>(IDataWriter writer, string name, UnitySubAsset<T> value) where T : Object {
		writer.Begin(name, null, true);
		DefaultDataWriteUtility.Write(writer, "Id", value.Id);
		DefaultDataWriteUtility.Write(writer, "Name", value.Name);
		writer.End(name, true);
	}

	public static void Write<T>(IDataWriter writer, string name, UnityAsset<T> value) where T : Object {
		writer.Begin(name, null, true);
		DefaultDataWriteUtility.Write(writer, "Id", value.Id);
		writer.End(name, true);
	}

	public static void Write<T>(IDataWriter writer, string name, Typed<T> value) where T : class, IObject {
		writer.Begin(name, null, true);
		DefaultDataWriteUtility.Write(writer, "Id", value.Id);
		writer.End(name, true);
	}

	public static void WriteList(IDataWriter writer, string name, List<GradientAlphaKey> value) {
		writer.Begin(name, null, true);
		foreach (var gradientAlphaKey in value)
			Write(writer, "Item", gradientAlphaKey);
		writer.End(name, true);
	}

	public static void WriteList(IDataWriter writer, string name, List<GradientColorKey> value) {
		writer.Begin(name, null, true);
		foreach (var gradientColorKey in value)
			Write(writer, "Item", gradientColorKey);
		writer.End(name, true);
	}

	public static void WriteList<T>(IDataWriter writer, string name, List<Typed<T>> value) where T : class, IObject {
		var type = typeof(T);
		writer.Begin(name, null, true);
		foreach (var typed in value)
			Write(writer, "Item", typed);
		writer.End(name, true);
	}
}