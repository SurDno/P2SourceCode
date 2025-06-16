namespace Engine.Source.Achievements.Controllers
{
  public interface IAchievementController
  {
    void Initialise(string id);

    void Terminate();
  }
}
