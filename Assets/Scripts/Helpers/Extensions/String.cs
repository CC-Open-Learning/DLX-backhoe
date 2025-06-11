namespace RemoteEducation.Extensions
{
    public static class StringExtensions
    {
		/// <summary>Capitalizes the first character of the string.</summary>
		public static string Capitalize(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str;
        }

		public static bool GetInt(this string str, out int result)
		{
			result = -1;
			bool canParse = false;
			int i = 0;
			int firstDigit = -1;
			int length = 1;

			for (; i < str.Length; ++i)
			{
				if (char.IsDigit(str[i]))
				{
					canParse = true;
					firstDigit = i++;
					break;
				}
			}

			if (canParse)
			{
				for (; i < str.Length; ++i)
				{
					if (char.IsDigit(str[i]) == false)
					{
						length = i - firstDigit;
						break;
					}
				}
				result = int.Parse(str.Substring(firstDigit, length));
			}

			return canParse;
		}
	}
}
