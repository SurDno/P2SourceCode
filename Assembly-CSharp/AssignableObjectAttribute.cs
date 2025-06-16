using System;
using UnityEngine;

public class AssignableObjectAttribute : PropertyAttribute {
	public Type BaseType { get; }

	public AssignableObjectAttribute(Type baseType) {
		BaseType = baseType;
	}
}