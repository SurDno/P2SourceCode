namespace Engine.Source.Commons.Parameters
{
  public interface IComputeParameter
  {
    void ResetResetable();

    void CorrectValue();

    void ComputeEvent();
  }
}
