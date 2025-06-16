// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.Internal.BBObjectParameter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using System;
using UnityEngine;

#nullable disable
namespace NodeCanvas.Framework.Internal
{
  [Serializable]
  public class BBObjectParameter : BBParameter<object>
  {
    [SerializeField]
    private System.Type _type;

    public BBObjectParameter() => this.SetType(typeof (object));

    public BBObjectParameter(System.Type t) => this.SetType(t);

    public override System.Type varType => this._type;

    public void SetType(System.Type t)
    {
      if (t == (System.Type) null)
        t = typeof (object);
      if (t != this._type)
        this._value = t.RTIsValueType() ? Activator.CreateInstance(t) : (object) null;
      this._type = t;
    }
  }
}
