using System.Collections.Generic;
using System.Threading.Tasks;

using YoutubeCutter.Core.Models;

namespace YoutubeCutter.Core.Contracts.Services
{
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetListDetailsDataAsync();
    }
}
