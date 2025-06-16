using System;

namespace SRF.Service;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ServiceConstructorAttribute : Attribute {
	public ServiceConstructorAttribute(Type serviceType) {
		ServiceType = serviceType;
	}

	public Type ServiceType { get; private set; }
}