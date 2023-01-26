using System.Threading;
using System.Threading.Tasks;
using Glb.Common.Entities;

namespace Glb.Common.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}