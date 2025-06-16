// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.Internal.MissingNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using System;

#nullable disable
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

    public override Type outConnectionType => (Type) null;

    public override int maxInConnections => 0;

    public override int maxOutConnections => 0;

    public override bool showCommentsBottom => false;
  }
}
