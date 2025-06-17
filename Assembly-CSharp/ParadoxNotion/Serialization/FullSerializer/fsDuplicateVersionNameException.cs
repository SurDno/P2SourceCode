using System;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public sealed class fsDuplicateVersionNameException(Type typeA, Type typeB, string version) : Exception(
    typeA + " and " + typeB + " have the same version string (" + version + "); please change one of them.");
}
