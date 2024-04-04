namespace RDMSharpTests
{
    public static class Helper
    {
        public static void ThrowInnerException<T>(Action action) where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T t)
            {
                throw t.InnerException ?? t;
            }
        }
    }
}