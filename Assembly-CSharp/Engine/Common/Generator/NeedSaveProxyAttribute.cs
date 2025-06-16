using System;

namespace Engine.Common.Generator;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class NeedSaveProxyAttribute : Attribute { }