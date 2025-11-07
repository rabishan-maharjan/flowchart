using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using Arcube.Utility;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace Arcube.Api
{
    public class ServerState
    {
        public DateTime maintainanceStartDate { get; set; }
        public DateTime maintainanceEndDate { get; set; }
        public bool isBeingMaintained;
    }
    public class InternetConnectionManager : MonoBehaviour
    {
        public static InternetConnectionManager Instance { get; private set; }
        private void Awake() => Instance = this;
        public bool Initialized { get; private set; }
        private readonly ServerState _serverState = new();
        public async Task<bool> Initialize()
        {
            // _serverState.isBeingMaintained = await CheckServerMaintenance();
            // if (_serverState.isBeingMaintained) return false;

            var isOnline = await IsOnline();
            if (isOnline) Initialized = await FetchTime();

            return Initialized;
        }

        public event Action<bool> OnOnlineStatusChanged;
        private bool _isOnline = true;
        public async Task<bool> IsOnline()
        {
            //  return true;
            // if (_serverState.isBeingMaintained) return false;

            //var apiCheck = await PingUrlWithErrorAsync(EnvironmentController.Env.urls.domain);
            var apiCheck = await PingUrlWithErrorAsync("https://internal.arcube.com.np/api/internet_time.php");
            bool wasOnline = _isOnline;
            _isOnline = apiCheck.Item1;

            // Only invoke event if coming back online from offline
            if (_isOnline && !wasOnline)
            {
                OnOnlineStatusChanged?.Invoke(true);
            }

            return apiCheck.Item1;
        }

        private async Task<(bool, string)> PingUrlWithErrorAsync(string url)
        {
            using var request = UnityWebRequest.Get(url);
            request.timeout = 5;

            try
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                    return (true, "Success");

                return request.result switch
                {
                    UnityWebRequest.Result.ConnectionError => (false, "Unable to establish a connection."),
                    UnityWebRequest.Result.DataProcessingError => (false, "Data processing issue occurred."),
                    UnityWebRequest.Result.ProtocolError => (false, $"Protocol issue (HTTP {request.responseCode})."),
                    _ => (false, "An unknown issue occurred.")
                };
            }
            catch (Exception ex)
            {
                return (false, $"Exception occurred: {ex.Message}");
            }
        }

        private async Task<bool> CheckServerMaintenance()
        {
            try
            {
                if (!await IsOnline())
                {
                    return false;
                }

                var result = await ApiManager.New.GetText(UrlKey.MaintenanceMode);
                var underMaintenance = JSON.Parse(result)["on_maintenance_mode"].AsBool;
                OnOnlineStatusChanged?.Invoke(true);
                return underMaintenance;
            }
            catch (Exception ex)
            {
                Log.AddWarning(() => ex.Message);
                Log.AddWarning(() => $"Server under maintenance: true");
                return true;
            }
        }

        private string _timeData;
        public DateTime CurrentDateTime { get; private set; }
        [ContextMenu("time")]
        public async Task<bool> FetchTime()
        {
            try
            {
                var result = await ApiManager.New.GetText(UrlKey.Time);
                _timeData = JSON.Parse(result)["current_time"].Value;

                if (_timeData == null)
                {
                    Log.AddWarning(() => "Unable to get time");
                    OnOnlineStatusChanged?.Invoke(false);
                }
                else
                {
                    StartCoroutine(StartSyncCount());
                }
            }
            catch (Exception ex)
            {
                Log.AddWarning(() => ex.Message);
            }

            Initialized = true;
            return Initialized;
        }

        //TODO move to relevant place
        private IEnumerator StartSyncCount()
        {
            var wait = new WaitForSeconds(1);
            const string format = "yyyy-MM-ddTHH:mm:ss.ffffffZ";
            CurrentDateTime = DateTime.ParseExact(_timeData, format, CultureInfo.InvariantCulture);
            while (true)
            {
                CurrentDateTime = CurrentDateTime.AddSeconds(1);
                yield return wait;
            }
        }
    }
}