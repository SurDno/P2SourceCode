using System;

namespace ParadoxNotion.Design;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate)]
public class SpoofAOTAttribute : Attribute { }