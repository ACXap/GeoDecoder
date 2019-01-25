namespace GeoCoding
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Первый символ строки в верхнем регистре
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUpperFistChar(this string str)
        {
            if(str.Length>0)
            {
                var chars = str.ToCharArray();
                chars[0] = char.ToUpper(chars[0]);
                return new string(chars);
            }
            return null;
        }
    }
}