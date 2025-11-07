using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Arcube.EditorTools
{
    public class LeaderboardInitializerEditor : EditorWindow
    {
        [Serializable]
        public class MusicInfo
        {
            public string id;
            public string title;
            public string[] tags;
            public bool status;
        }

        [Serializable]
        public struct LeaderboardId
        {
            public string id;
            public string title;
        }

        [Serializable]
        public class LeaderboardRequest
        {
            public string id;
            public string name;
            public string sortOrder;
            public string updateType;
        }

        private string projectId = "9d4c0fc9-56fc-4b5c-94e5-69f713b58f6c";
        private string productionId = "cbae139e-052d-4a8c-a211-540ebf613b06";
        private string developmentId = "2d237f87-8af7-4c2d-8ca6-9126e09f389e";
        private string environmentId;
        private string authorizationHeader = "ZjQ3MGE1M2ItYTk3Zi00NTg0LTgzMGUtMWRiYzQ5ODE2NmFhOlFrbkNRZHhHb1RiaUVzV0k0ZGEyeFEtZWk5cWktNjN0";
        private string baseUrl = "https://services.api.unity.com/leaderboards/v1/projects";
        private bool isProduction = false;
        private string songListUrl = "https://pianokids.arcube.com.np/api/songs?pagination=none";
        private LeaderboardId[] leaderboardIds;
        private Vector2 scrollPosition;

        [MenuItem("Tools/Leaderboard/Initialize Leaderboards")]
        public static void ShowWindow()
        {
            var window = GetWindow<LeaderboardInitializerEditor>("Leaderboard Initializer");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Leaderboard Configuration", EditorStyles.boldLabel);

            projectId = EditorGUILayout.TextField("Project ID", projectId);
            productionId = EditorGUILayout.TextField("Production Environment ID", productionId);
            developmentId = EditorGUILayout.TextField("Development Environment ID", developmentId);
            authorizationHeader = EditorGUILayout.TextField("Authorization Header", authorizationHeader);
            baseUrl = EditorGUILayout.TextField("Base URL", baseUrl);
            isProduction = EditorGUILayout.Toggle("Use Production?", isProduction);
            songListUrl = EditorGUILayout.TextField("Song List URL", songListUrl);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Load Songs"))
            {
                _ = LoadSongs();
            }

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Create Leaderboards"))
            {
                if (leaderboardIds.Length == 0)
                {
                    Debug.LogWarning("⚠️ No songs loaded. Please load songs first.");
                    return;
                }
                _ = CreateLeaderboards();
            }
        }

        private async Task LoadSongs()
        {
            try
            {
                var songs = await GetData<List<MusicInfo>>(songListUrl);

                leaderboardIds = songs
                    .Where(x => !x.tags.Contains("Test") && x.status)
                    .OrderByDescending(x => int.TryParse(x.id, out var num) ? num : int.MinValue)
                    .Select(x => new LeaderboardId { id = x.id, title = x.title })
                    .ToArray();

                foreach (var lb in leaderboardIds)
                {
                    Debug.Log($"Loaded song: {lb.id} - {lb.title}");
                }
                Debug.Log($"✅ Loaded {leaderboardIds.Length} songs.");
                Repaint();
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to load songs: {e.Message}");
            }
        }
        public async Task<T> GetData<T>(string url) where T : class
        {
            using var www = UnityWebRequest.Get(url);

            www.timeout = 10;
            await www.SendWebRequest();

            if (www.responseCode != 200 && www.responseCode != 201 && www.responseCode != 400)
            {
                var result = www.result;
                var responseCode = www.responseCode;
                Log.AddWarning(() => $"{url}:{result}:{responseCode}");
                return null;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<T>(www.downloadHandler.text);
                return data;
            }
            catch (JsonException ex)
            {
                Log.AddError(() => "JSON Deserialization Error: " + ex.Message);
                return null;
            }
        }

        private async Task CreateLeaderboards()
        {
            environmentId = isProduction ? productionId : developmentId;

            if (leaderboardIds == null || leaderboardIds.Length == 0)
            {
                Debug.LogWarning("⚠️ No songs loaded. Please load songs first.");
                return;
            }

            foreach (var leaderboard in leaderboardIds)
            {
                var exists = await CheckLeaderboardExists(leaderboard.id);
                if (!exists)
                {
                    Debug.Log($"Creating leaderboard: {leaderboard.id}");
                    await CreateLeaderboard(leaderboard.id, leaderboard.title);
                }
                else
                {
                    Debug.Log($"Leaderboard exists: {leaderboard.id}");
                    break;
                }
            }

            Debug.Log("🏁 Leaderboard creation process completed.");
        }

        private async Task<bool> CheckLeaderboardExists(string leaderboardId)
        {
            var url = $"{baseUrl}/{projectId}/environments/{environmentId}/leaderboards/{leaderboardId}";
            using var request = UnityWebRequest.Get(url);

            request.SetRequestHeader("Authorization", $"Basic {authorizationHeader}");
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone) await Task.Yield();

            return request.result == UnityWebRequest.Result.Success;
        }

        private async Task CreateLeaderboard(string leaderboardId, string title)
        {
            title = Regex.Replace(title, "[^a-zA-Z0-9_ ]", " ");
            title = title.Length > 50 ? title.Substring(0, 50) : title;

            var url = $"{baseUrl}/{projectId}/environments/{environmentId}/leaderboards";
            var jsonBody = JsonUtility.ToJson(new LeaderboardRequest
            {
                id = leaderboardId,
                name = title,
                sortOrder = "desc",
                updateType = "keepBest"
            });

            using var request = new UnityWebRequest(url, "POST");
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Basic {authorizationHeader}");
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success || request.responseCode == 201)
            {
                Debug.Log($"✅ Created leaderboard: {leaderboardId}");
            }
            else
            {
                Debug.LogError($"❌ Failed to create leaderboard: {leaderboardId} - {request.responseCode}\n{request.downloadHandler.text}");
            }
        }
    }
}
