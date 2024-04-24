using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorApp2.Client
{
    public interface IAuthService
    {
        Task<bool> IsAuthenticated();
    }
    public class AuthService : IAuthService
    {
        private readonly IJSRuntime _jsRuntime;


        public AuthService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
            return !string.IsNullOrEmpty(token);
        }
        
    }
}
