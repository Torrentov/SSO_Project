using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;



[InterfaceType(ComInterfaceType.InterfaceIsDual)]
[Guid("E5EADB1C-2D03-428C-A724-511938752888")]
public interface _Visible_Methods
{
    string GetCode(string client_id, string response_type, string email, string password);
    ApiToken GetToken(string client_id, string client_secret, string code, string grant_type);
    string ApiRequest(string path, ApiToken token);
}

[ClassInterface(ClassInterfaceType.None)]
[Guid("CDE63844-9FBF-44EF-9A32-E0A2CA847994")]
[ProgId("SSOApi")]
public class SSOApi : _Visible_Methods
{

    public class Code
    {
        public string code { get; set; }
    }

    private string _server = "https://localhost:7128";

    private async Task<string> GetCodeAsync(string client_id, string response_type, string email, string password)
    {
        HttpClient _httpClient = new HttpClient();
        var values = new Dictionary<string, string>
        {
            { "client_id", client_id },
            { "response_type", response_type },
            { "email", email },
            { "password", password }
        };
        var content = new FormUrlEncodedContent(values);
        var response = await _httpClient.PostAsync(_server + "/api/Authenticate/get-code/", content);
        var code = await response.Content.ReadAsStringAsync();
        return code;
    }

    public string GetCode(string client_id, string response_type, string email, string password)
    {
        Code code = JsonConvert.DeserializeObject<Code>(GetCodeAsync(client_id, response_type, email, password).Result);
        return code.code;
    }

    private async Task<string> GetTokenAsync(string client_id, string client_secret, string code, string grant_type)
    {
        HttpClient _httpClient = new HttpClient();
        var values = new Dictionary<string, string>
            {
                { "client_id", client_id },
                { "code", code },
                { "grant_type", grant_type },
                { "client_secret", client_secret }
            };
        var content = new FormUrlEncodedContent(values);
        var response = await _httpClient.PostAsync(_server + "/api/Authenticate/get-token", content);
        //var token = await response.Content.ReadAsAsync<ApiToken>();
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    public ApiToken GetToken(string client_id, string client_secret, string code, string grant_type)
    {
        var token = JsonConvert.DeserializeObject<ApiToken>(GetTokenAsync(client_id, client_secret, code, grant_type).Result);
        return token;
    }

    private async Task<string> ApiRequestAsync(string path, ApiToken token)
    {
        HttpClient _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
        if (path[0] != '/')
        {
            path = "/" + path;
        }
        var response = await _httpClient.GetAsync(_server + path);
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    public string ApiRequest(string path, ApiToken token)
    {
        return ApiRequestAsync(path, token).Result;
    }
}
