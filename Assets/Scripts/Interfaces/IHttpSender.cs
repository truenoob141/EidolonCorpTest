using System.Threading;
using Cysharp.Threading.Tasks;

namespace EidolonCorpTest.Interfaces
{
    public interface IHttpSender
    {
        public UniTask<(bool, long)> Post(string address, string data = null, CancellationToken token = default);
    }
}