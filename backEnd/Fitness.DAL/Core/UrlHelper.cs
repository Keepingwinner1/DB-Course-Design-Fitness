using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fitness.DAL.Core
{
    public class UrlHelper
    {
        public static bool IsUrl(string input)
        {
            string urlPattern = @"^(https?:\/\/)?([\p{L}\p{N}\-\.]+)\.([\p{L}]{2,})(:\d+)?(\/[-\p{L}\p{N}@:%_\+.~#?&//=]*)?$";
            return Regex.IsMatch(input, urlPattern, RegexOptions.IgnoreCase);
        }

    }
}
