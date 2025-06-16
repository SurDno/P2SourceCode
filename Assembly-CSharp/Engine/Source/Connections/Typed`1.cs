using System;
using Engine.Common;
using Inspectors;

namespace Engine.Source.Connections;

public struct Typed<T> where T : class, IObject {
	private Guid id;

	[Inspected] public Guid Id => id;

	public Typed(Guid id) {
		this.id = id;
	}

	public T Value {
		get => TemplateUtility.GetTemplate<T>(Id);
		set => id = value != null ? value.Id : Guid.Empty;
	}

	public override int GetHashCode() {
		return id.GetHashCode();
	}

	public override bool Equals(object a) {
		return a is Typed<T> typed && this == typed;
	}

	public static bool operator ==(Typed<T> a, Typed<T> b) {
		return a.Id == b.Id;
	}

	public static bool operator !=(Typed<T> a, Typed<T> b) {
		return a.Id != b.Id;
	}
}