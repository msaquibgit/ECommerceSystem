using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<(string Subject, string Content)> ResolveAsync(int typeId, int channelId, int? version, Dictionary<string, object>? data);
    }
}
