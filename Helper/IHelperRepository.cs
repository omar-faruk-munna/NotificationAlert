using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public interface IHelperRepository
    {
        string GetValidNumber(string toNumber, string numberPrefixList);
        string ConvertBanglatoUnicode(string banglaText);
    }
}
