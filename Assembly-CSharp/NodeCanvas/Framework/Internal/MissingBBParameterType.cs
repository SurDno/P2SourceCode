// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.Internal.MissingBBParameterType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;

#nullable disable
namespace NodeCanvas.Framework.Internal
{
  public class MissingBBParameterType : BBParameter<object>, IMissingRecoverable
  {
    [fsProperty]
    public string missingType { get; set; }

    [fsProperty]
    public string recoveryState { get; set; }
  }
}
