using System.Collections.Generic;
using System.Text;

namespace ChenPipi.ProjectPinBoard.Editor
{

    public static class ListExtension
    {

        public static string Join<T>(this IList<T> list, string separator = "", bool ignoreEmptyOrNull = true)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < list.Count; ++i)
            {
                if (ignoreEmptyOrNull && string.IsNullOrEmpty(list[i].ToString()))
                {
                    continue;
                }
                builder.Append(list[i]);
                if (i < list.Count - 1)
                {
                    builder.Append(separator);
                }
            }
            return builder.ToString();
        }

    }

}
