using System.Threading.Tasks;
using Glb.Common.Entities.Requests;
using Glb.Common.Entities.Responses;
using Glb.Common.Settings;

namespace Glb.Common.Interfaces
{
    public interface ISMS_Service
    {
        Task<SMS_Response>  SendAsync(SMS_Data request,Enums.ServiceProvider? smsServiceProvider=null);
    }
}