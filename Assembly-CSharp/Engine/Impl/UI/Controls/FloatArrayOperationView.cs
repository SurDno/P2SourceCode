namespace Engine.Impl.UI.Controls
{
  public class FloatArrayOperationView : FloatArrayView
  {
    [SerializeField]
    private float a;
    [SerializeField]
    private OperationEnum operation;
    [SerializeField]
    private float b;
    [SerializeField]
    private FloatView resultView;

    public override void GetValue(int index, out float value)
    {
      switch (index)
      {
        case 0:
          value = a;
          break;
        case 1:
          value = b;
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
          SetValue(ref a, value);
          break;
        case 1:
          SetValue(ref b, value);
          break;
      }
    }

    private void OnValidate() => ApplyValues();

    private void SetValue(ref float field, float value)
    {
      if (field == (double) value)
        return;
      field = value;
      ApplyValues();
    }

    private void ApplyValues()
    {
      if (!((Object) resultView != (Object) null))
        return;
      switch (operation)
      {
        case OperationEnum.Add:
          resultView.FloatValue = a + b;
          break;
        case OperationEnum.Subtract:
          resultView.FloatValue = a - b;
          break;
        case OperationEnum.Multiply:
          resultView.FloatValue = a * b;
          break;
        case OperationEnum.Divide:
          if (b != 0.0)
          {
            resultView.FloatValue = a / b;
          }
          break;
      }
    }

    public override void SkipAnimation()
    {
      if (!((Object) resultView != (Object) null))
        return;
      resultView.SkipAnimation();
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
