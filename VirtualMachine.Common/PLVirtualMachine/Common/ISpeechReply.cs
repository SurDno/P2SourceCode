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
