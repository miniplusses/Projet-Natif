using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tp2Application.Model; 

public static class ApiRequest {
    
    public static async Task<T> SendRequest<T>(string _uri, string _jsonData, string method) {
        string apiUrl = "http://localhost:5284/Tp2/" + _uri;
        HttpResponseMessage response;
        
        using (HttpClient client = new HttpClient()) {
            if (method == "POST") {
                response = await client.PostAsync(apiUrl,
                    new StringContent(_jsonData, Encoding.UTF8, "application/json"));
            }
            else if (method == "PUT") {
                response = await client.PutAsync(apiUrl,
                    new StringContent(_jsonData, Encoding.UTF8, "application/json"));
            }
            else {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
                request.Content = new StringContent(_jsonData, Encoding.UTF8, "application/json");
                
                response = await client.SendAsync(request);
            }

            if (response.IsSuccessStatusCode) {
                if (method == "PUT") {
                    if (_uri != "" && _uri[0] == '{') {
                        T resultat = (T)Convert.ChangeType(response, typeof(T));
                        return resultat;
                    }
                    else {
                       string contenu = await response.Content.ReadAsStringAsync();
                        T resultat = (T)Convert.ChangeType(contenu, typeof(T));
                        return resultat; 
                    }
                } else if (method == "POST" && response.StatusCode != HttpStatusCode.NoContent) {
                    return await response.Content.ReadFromJsonAsync<T>();
                } else if (method == "DELETE") {
                    T resultat = (T)Convert.ChangeType(response, typeof(T));
                    return resultat;
                } else {
                    return default(T);
                }

            }
        }
        
        throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
    }
}