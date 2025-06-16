// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.ISpeechReply
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

#nullable disable
namespace PLVirtualMachine.Common
{
  public interface ISpeechReply : IObject, IEditorBaseTemplate, IOrderedChild
  {
    IGameString Text { get; }

    bool OnlyOnce { get; }

    bool OnlyOneReply { get; }

    bool IsDefault { get; }

    ICondition EnableCondition { get; }

    IActionLine ActionLine { get; }
  }
}
