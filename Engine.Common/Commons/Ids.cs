using System;

namespace Engine.Common.Commons
{
  public static class Ids
  {
    public static readonly Guid MainId = new Guid("8eb1fde2-b043-2604-2a9b-3e007cfefb62");
    public static readonly Guid PathologicId = new Guid("f07a62d3-6c61-4e82-b5be-713051f73f3f");
    public static readonly Guid HierarchyId = new Guid("00000000-0000-0000-0000-000000000001");
    public static readonly Guid StorablesId = new Guid("C4165512-B881-46B8-8F29-C5434FED2D77");
    public static readonly Guid ObjectsId = new Guid("0299F8B5-5531-4290-8196-CEC3432212CC");
    public static readonly Guid OthersId = new Guid("{85E1C900-E337-4F77-837B-9B071CAEADEE}");

    public static bool IsRoot(Guid id)
    {
      return id == Ids.HierarchyId || id == Ids.StorablesId || id == Ids.ObjectsId || id == Ids.OthersId;
    }
  }
}
