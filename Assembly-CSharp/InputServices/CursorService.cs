namespace InputServices
{
  public static class CursorService
  {
    private static ICursorController instance;

    public static ICursorController Instance
    {
      get
      {
        if (CursorService.instance == null)
          CursorService.instance = (ICursorController) new WindowsCursorController();
        return CursorService.instance;
      }
    }
  }
}
