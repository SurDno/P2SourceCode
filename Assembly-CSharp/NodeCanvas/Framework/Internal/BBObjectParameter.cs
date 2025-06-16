using ParadoxNotion;
using System;
using UnityEngine;

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
