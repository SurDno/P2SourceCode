// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.ValidateResult
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace Engine.Source.Components.Interactable
{
  public struct ValidateResult
  {
    public ValidateResult(bool result, string reason = "")
    {
      this.Result = result;
      this.Reason = reason;
    }

    public bool Result { get; private set; }

    public string Reason { get; private set; }
  }
}
