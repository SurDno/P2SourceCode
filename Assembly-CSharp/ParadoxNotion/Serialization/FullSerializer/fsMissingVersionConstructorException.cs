using System;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public sealed class fsMissingVersionConstructorException(Type versionedType, Type constructorType)
    : Exception(versionedType + " is missing a constructor for previous model type " + constructorType);
}
