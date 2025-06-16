namespace Engine.Source.Commons
{
  public abstract class InstanceByRequest<T> where T : class, new()
  {
    private static T instance;

    public static T Instance
    {
      get
      {
        if ((object) InstanceByRequest<T>.instance == null)
          InstanceByRequest<T>.instance = new T();
        return InstanceByRequest<T>.instance;
      }
    }
  }
}
