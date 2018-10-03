namespace GeekLearning.Email.Unit.Test
{
    public static class Utils
    {
        public static T[] Yield<T>(this T item)
        {
            return new T[1] { item };
        }
    }
}
