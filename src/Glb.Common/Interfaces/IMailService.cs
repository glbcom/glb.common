using System.Threading;
using System.Threading.Tasks;
using Glb.Common.Entities;

namespace Glb.Common.Inerfaces
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}