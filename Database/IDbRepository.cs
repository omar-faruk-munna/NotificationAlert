using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public interface IDbRepository
    {
        Task CreateRecord(int fncId, string productId, string branchId, string accountNo, string custId, int smsFlag, string toNumber, string toSmsContent, int smsRespFlag, string smsReasonForFail, int emailFlag, string toEmail, string emailSubject, string toEmailContent, int emailRespFlag, string emailReasonForFail, int pushFlag, int alertFlag);

        IEnumerable<dynamic> GetCustomerContact(int isMultiple, string branchId, string accountNo);

        IEnumerable<dynamic> GetCustomerTest();
    }
}
