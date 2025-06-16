using System;

namespace SRF.Service;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ServiceSelectorAttribute : Attribute {
	public ServiceSelectorAttribute(Type serviceType) {
		ServiceType = serviceType;
	}

	public Type ServiceType { get; private set; }
}