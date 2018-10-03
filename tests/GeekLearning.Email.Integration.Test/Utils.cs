namespace GeekLearning.Email.Integration.Test
{
    public static class Utils
    {
        public static T[] Yield<T>(this T item)
        {
            return new T[1] { item };
        }
    }
}
