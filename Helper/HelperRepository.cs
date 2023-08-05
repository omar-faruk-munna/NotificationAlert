using LogLog4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class HelperRepository : IHelperRepository
    {
        private readonly ILoggerRepository _error;

        public HelperRepository(ILoggerRepository error)
        {
            this._error = error;
        }

        public string ConvertBanglatoUnicode(string banglaText)
        {
            string unicode = string.Empty;

            try
            {
                StringBuilder sb = new StringBuilder();

                foreach (char c in banglaText)
                {
                    sb.AppendFormat("{1:x4}", c, (int)c);
                }

                unicode = sb.ToString().ToUpper();
            }
            catch (Exception e)
            {
                _error.LogError("Helper", "ConvertBanglatoUnicode", e.Message);
                unicode = string.Empty;
            }

            return unicode;
        }

        public string GetValidNumber(string toNumber, string numberPrefixList)
        {
            string resp = "";

            try
            {
                if (!string.IsNullOrWhiteSpace(toNumber))
                {
                    toNumber = toNumber.Trim();
                    int countToNumber = toNumber.Length;

                    if (countToNumber.Equals(11))
                    {
                        string numberPrefix = "88" + toNumber.Substring(0, 3);

                        if (IsValidNumberPrefix(numberPrefix, numberPrefixList))
                        {
                            resp = "88" + toNumber;
                            return resp;
                        }
                    }
                    else if (countToNumber.Equals(13))
                    {
                        string numberPrefix = toNumber.Substring(0, 5);

                        if (IsValidNumberPrefix(numberPrefix, numberPrefixList))
                        {
                            resp = toNumber;
                            return resp;
                        }
                    }
                    else if (countToNumber > 11)
                    {
                        toNumber = toNumber.Substring(toNumber.Length - 11, 11);
                        string numberPrefix = "88" + toNumber.Substring(0, 3);

                        if (IsValidNumberPrefix(numberPrefix, numberPrefixList))
                        {
                            resp = "88" + toNumber;
                            return resp;
                        }
                    }

                    return resp;
                }

                return resp;
            }
            catch (Exception e)
            {
                _error.LogError("Helper", "GetValidNumber", e.Message);
                return "";
            }
        }

        private bool IsValidNumberPrefix(string numberPrefix, string numberPrefixList)
        {
            try
            {
                List<string> prefixList = numberPrefixList?.Split(',').ToList();

                foreach (var item in prefixList)
                {
                    if (numberPrefix.Equals(item.Trim()))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _error.LogError("Helper", "IsValidNumber", e.Message);
                return false;
            }

            return false;
        }
    }
}
