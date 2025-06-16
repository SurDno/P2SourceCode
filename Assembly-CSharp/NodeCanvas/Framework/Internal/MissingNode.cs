using System;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;

namespace NodeCanvas.Framework.Internal
{
  [DoNotList]
  [Description("Please resolve the MissingNode issue by either replacing the node or importing the missing node type in the project.")]
  public sealed class MissingNode : Node, IMissingRecoverable
  {
    [fsProperty]
    public string missingType { get; set; }

    [fsProperty]
    public string recoveryState { get; set; }

    public override string name => "<color=#ff6457>* Missing Node *</color>";

    public override Type outConnectionType => null;

    public override int maxInConnections => 0;

    public override int maxOutConnections => 0;

    public override bool showCommentsBottom => false;
  }
}
