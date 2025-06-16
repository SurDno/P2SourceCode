namespace Engine.Common.Services
{
  public interface IJerboaService
  {
    float Quality { get; set; }

    float Amount { get; set; }

    JerboaColorEnum Color { get; set; }

    void Syncronize();
  }
}
