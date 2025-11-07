using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Arcube.Utility;

namespace Arcube.Api
{
    public class  ApiManager
    {
        public static ApiManager New => new();
        
        public static async Task<string> GetHeader(string url, string header)
        {
            var www = UnityWebRequest.Head(EnvironmentController.Env.urls.GetDataUrl(url));
            await www.SendWebRequest();
            return www.result != UnityWebRequest.Result.Success ? null : www.GetResponseHeader(header);
        }
        
        public async Task<byte[]> DownloadBytes(string url)
        {
            using var www = UnityWebRequest.Get(EnvironmentController.Env.urls.GetDataUrl(url));
            await SendWebRequest(www);
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Log.AddError(()=> $"Failed to download MIDI file: {www.uri} {www.error}");
                return null;
            }

            return www.downloadHandler.data;
        }

        public async Task<byte[]> DownloadBytesExternal(string url)
        {
            using var www = UnityWebRequest.Get(url);
            await SendWebRequest(www);
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Log.AddError(()=> $"Failed to download MIDI file: {www.uri} {www.error}");
                return null;
            }

            return www.downloadHandler.data;
        }
        
        public async Task<T> PostData<T>(UrlKey urlKey, string parameters) where T : class
        {
            var url = EnvironmentController.Env.urls.GetApiURL(urlKey, parameters);
            using var www = UnityWebRequest.PostWwwForm(url, UnityWebRequest.kHttpVerbPOST);
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            await SendWebRequest(www);
            if (www.responseCode != 201 && www.responseCode != 200)
            {
                Log.AddWarning(()=> $"{url}:{www.result}");
                return null;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Log.Add(()=> www.result);
                return null;
            }
            
            var data = JsonConvert.DeserializeObject<T>(www.downloadHandler.text);
            return data;
        }

        public async Task<string> GetText(UrlKey urlKey, string parameter = null)
        {
            var url = EnvironmentController.Env.urls.GetApiURL(urlKey, parameter);
            using var www = UnityWebRequest.Get(url);

            www.timeout = 10;
            await www.SendWebRequest();

            if (www.responseCode != 404 && www.result == UnityWebRequest.Result.Success) return www.downloadHandler.text;
            var error = www.error;
            Log.AddWarning(()=> $"Request failed: {error} url: {url}");
            return string.Empty;
        }
        
        public async Task<T> GetData<T>(UrlKey urlKey, string parameter = null) where T : class
        {
            var url = EnvironmentController.Env.urls.GetApiURL(urlKey, parameter);
            using var www = UnityWebRequest.Get(url);
            www.timeout = 10;
            await www.SendWebRequest();

            if (www.responseCode != 200 && www.responseCode != 201 && www.responseCode != 400)
            {
                var result = www.result;
                var responseCode = www.responseCode;
                Log.AddWarning(()=> $"{url}:{result}:{responseCode}");
                return null;
            }

            try
            {
                var responseText = ReplaceBraces(www.downloadHandler.text);
                var data = JsonConvert.DeserializeObject<T>(responseText);
                return data;
            }
            catch (JsonException ex)
            {
                Log.AddError(()=> "JSON Deserialization Error: " + ex.Message);
                return null;
            }
        }
        
        private static string ReplaceBraces(string input)
        {
            var pattern = "\"{";
            var replacement = "{";

            var result = Regex.Replace(input, pattern, replacement);

            pattern = "}\"";
            replacement = "}";

            result = Regex.Replace(result, pattern, replacement);

            result = result.Replace("\\", "");
            return result;
        }

        public async Task<Sprite> GetImageAsync(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return null;
                using var www = UnityWebRequestTexture.GetTexture(url);

                www.timeout = 10;
                await www.SendWebRequest();

                Texture2D texture = null;
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Log.AddWarning(()=> www.error + ":" + www.url);
                    return null;
                }

                texture = DownloadHandlerTexture.GetContent(www);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                return sprite;
            }
            catch(Exception e)
            {
                Log.AddException(e);
                return null;
            }
        }
        
        private static async Task SendWebRequest(UnityWebRequest www, float time = 5, int retries = 1)
        {
            await www.SendWebRequest();
            var timer = time;
            while (!www.isDone && timer > 0)
            {
                timer -= Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }

            if (!www.isDone)
            {
                Log.Add(()=> "Piano: Request timed out: " + www.url);
                www.Abort();
            } 
        }
    }
}