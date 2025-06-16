namespace InputServices
{
  public static class CursorService
  {
    private static ICursorController instance;

    public static ICursorController Instance
    {
      get
      {
        if (instance == null)
          instance = (ICursorController) new WindowsCursorController();
        return instance;
      }
    }
  }
}
