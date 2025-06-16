using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class FloatArrayOperationView : FloatArrayView
  {
    [SerializeField]
    private float a;
    [SerializeField]
    private FloatArrayOperationView.OperationEnum operation;
    [SerializeField]
    private float b;
    [SerializeField]
    private FloatView resultView;

    public override void GetValue(int index, out float value)
    {
      switch (index)
      {
        case 0:
          value = this.a;
          break;
        case 1:
          value = this.b;
          break;
        default:
          value = 0.0f;
          break;
      }
    }

    public override void SetValue(int index, float value)
    {
      switch (index)
      {
        case 0:
          this.SetValue(ref this.a, value);
          break;
        case 1:
          this.SetValue(ref this.b, value);
          break;
      }
    }

    private void OnValidate() => this.ApplyValues();

    private void SetValue(ref float field, float value)
    {
      if ((double) field == (double) value)
        return;
      field = value;
      this.ApplyValues();
    }

    private void ApplyValues()
    {
      if (!((Object) this.resultView != (Object) null))
        return;
      switch (this.operation)
      {
        case FloatArrayOperationView.OperationEnum.Add:
          this.resultView.FloatValue = this.a + this.b;
          break;
        case FloatArrayOperationView.OperationEnum.Subtract:
          this.resultView.FloatValue = this.a - this.b;
          break;
        case FloatArrayOperationView.OperationEnum.Multiply:
          this.resultView.FloatValue = this.a * this.b;
          break;
        case FloatArrayOperationView.OperationEnum.Divide:
          if ((double) this.b != 0.0)
          {
            this.resultView.FloatValue = this.a / this.b;
            break;
          }
          break;
      }
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.resultView != (Object) null))
        return;
      this.resultView.SkipAnimation();
    }

    private enum OperationEnum
    {
      Add,
      Subtract,
      Multiply,
      Divide,
    }
  }
}
