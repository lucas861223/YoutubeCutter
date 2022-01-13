using System.Threading.Tasks;

namespace YoutubeCutter.Contracts.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle();

        Task HandleAsync();
    }
}
