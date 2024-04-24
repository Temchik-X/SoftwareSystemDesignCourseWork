using BlazorApp2.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorApp2.Client
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public AuthenticationService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<string> LoginAsync(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Authentication/login", model);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

            await SaveTokenAsync(data.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
            return data.Token;
        }

        public async Task LogoutAsync()
        {
            await RemoveTokenAsync();
        }
        public async Task<string> GetUsernameJson()
        {
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var usernameResponse = await _httpClient.GetStringAsync($"api/authentication/get-username");
            return usernameResponse;
        }

        public async Task<string> GetRoleJson()
        {
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var roleResponse = await _httpClient.GetStringAsync("api/authentication/get-role");
            return roleResponse;
        }
        public async Task<string> GetIdJson()
        {
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var idResponse = await _httpClient.GetStringAsync("api/authentication/get-id");
            return idResponse;
        }
        public async Task<string> GetRole()
        {
            string roleJson = await GetRoleJson();
            // Десериализуем строку JSON в объект
            var roleObject = JsonSerializer.Deserialize<Dictionary<string, string>>(roleJson);

            // Получаем значение поля "role"
            if (roleObject.TryGetValue("role", out var roleName))
            {
                return roleName;
            }
            else
            {
                // Если поле "role" не найдено, возвращаем пустую строку или другое значение по умолчанию
                return string.Empty;
            }
        }
        public async Task<string> GetUsername()
        {
            string usernameJson = await GetUsernameJson();
            // Десериализуем строку JSON в объект
            var usernameObject = JsonSerializer.Deserialize<Dictionary<string, string>>(usernameJson);

            // Получаем значение поля "role"
            if (usernameObject.TryGetValue("username", out var userName))
            {
                return userName;
            }
            else
            {
                // Если поле "role" не найдено, возвращаем пустую строку или другое значение по умолчанию
                return string.Empty;
            }
        }
        public async Task<int> GetId()
        {
            string idJson = await GetIdJson();
            // Десериализуем строку JSON в объект
            var idObject = JsonSerializer.Deserialize<Dictionary<string, string>>(idJson);

            // Получаем значение поля "id"
            if (idObject.TryGetValue("id", out var idName))
            {
                return int.Parse(idName);
            }
            else
            {
                // Если поле "role" не найдено, возвращаем пустую строку или другое значение по умолчанию
                return 0;
            }
        }
        private async Task SaveTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "authToken", token);
        }

        private async Task RemoveTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        public async Task<bool> IsAuthenticated()
        {
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
            return !string.IsNullOrEmpty(token);
        }
        public async Task<string> GetToken()
        {
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
            return token;
        }
    }
}
