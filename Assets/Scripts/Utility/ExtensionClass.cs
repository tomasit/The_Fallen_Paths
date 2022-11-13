namespace ExtensionMethods
{
    public static class StringExtension
    {
        public static int CountSubstring(this string text, string value)
        {                  
            int count = 0, minIndex = text.IndexOf(value, 0);
            while (minIndex != -1)
            {
                minIndex = text.IndexOf(value, minIndex + value.Length);
                count++;
            }
            return count;
        }
    }
}