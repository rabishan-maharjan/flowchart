using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Arcube.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Arcube
{
    public class Log : MonoBehaviour
    {
        [FormerlySerializedAs("_settings")] [SerializeField]
        private LogColorSettings colorSettings;

        [SerializeField] private TMP_Text t_text;
        [SerializeField] private ScrollRect _scroll;
        public static Log instance;
        [field: SerializeField] public LogSettings LOGSettings { get; set; }

        private void Awake() => instance = this;

        public static void Add(Func<object> messageProvider, string tag = "Piano", Object context = null)
        {
#if UNITY_EDITOR
            if (!instance)
            {
                Debug.Log(messageProvider(), context);
                return;
            }
#endif
            if (!instance.LOGSettings.showNormal) return;
                
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR
            Debug.Log($"{tag}: {messageProvider()}", context);
            instance.CreateText(messageProvider(), LogType.Default);
#else
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.WriteLine($"{tag}: {messageProvider()}");
            System.Console.ResetColor();
#endif
        }

        public static void AddTest(Func<object> messageProvider, string tag = "Piano", Object context = null)
        {
#if UNITY_EDITOR
            if (!instance)
            {
                Debug.Log(messageProvider(), context);
                return;
            }
#endif
            if (!instance.LOGSettings.showTest) return;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR
            Debug.Log($"{tag}: {messageProvider()}", context);
            instance.CreateText(messageProvider(), LogType.Default);
#else
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.WriteLine($"{tag}: {messageProvider()}");
            System.Console.ResetColor();
#endif
        }

        public static void AddPriority(Func<object> messageProvider, string tag = "Piano", Object context = null)
        {
#if UNITY_EDITOR
            if (!instance)
            {
                Debug.Log(messageProvider(), context);
                return;
            }
#endif
            if (!instance.LOGSettings.showPriority) return;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR
            Debug.Log($"{tag}: {messageProvider()}", context);
            instance.CreateText(messageProvider(), LogType.Priority);
#else
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.WriteLine($"{tag}: {messageProvider()}");
            System.Console.ResetColor();
#endif
        }

        public static void AddHighlight(Func<object> messageProvider, string tag = "Piano", Object context = null)
        {
#if UNITY_EDITOR
            if (!instance)
            {
                Debug.Log($"<color=green>{messageProvider()}</color>", context);
                return;
            }
#endif

            if (!instance.LOGSettings.showHighlights) return;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR
            Debug.Log($"{tag}: <color=green>{messageProvider()}</color>", context);
            instance.CreateText(messageProvider(), LogType.Highlight);
#else
            System.Console.ForegroundColor = System.ConsoleColor.Green;
            System.Console.WriteLine($"{tag}: {messageProvider()}");
            System.Console.ResetColor();

#endif
        }

        public static void AddWarning(Func<object> messageProvider, string tag = "Piano", Object context = null)
        {
#if UNITY_EDITOR
            if (!instance)
            {
                Debug.LogWarning(messageProvider(), context);
                return;
            }
#endif
            if (!instance.LOGSettings.showWarning) return;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR
            Debug.LogWarning($"{tag}: {messageProvider()}", context);
            instance.CreateText(messageProvider(), LogType.Warning);
#else
            System.Console.ForegroundColor = System.ConsoleColor.Yellow;
            System.Console.WriteLine($"{tag}: {messageProvider()}");
            System.Console.ResetColor();
#endif
        }

        public static void AddError(Func<object> messageProvider, string tag = "Piano", Object context = null)
        {
#if UNITY_EDITOR
            if (!instance)
            {
                Debug.LogError(messageProvider(), context);
                return;
            }
#endif
            if (!instance.LOGSettings.showError) return;

#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR
            Debug.LogError($"{tag}: {messageProvider()}", context);
            instance.CreateText(messageProvider(), LogType.Error);
#else
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine($"{tag}: {messageProvider()}");
            System.Console.ResetColor();
#endif
        }

        public static void AddException(Exception ex, string tag = "Piano", Object context = null)
        {
#if UNITY_EDITOR
            Debug.LogException(ex, context);
            return;
#endif
            if (!instance.LOGSettings.showException) return;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX
            Debug.LogError(ex.Message, context);
            instance.CreateText(ex.StackTrace, LogType.Error);
#else
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine($"{tag}: {ex.Message}");
            System.Console.ResetColor();
#endif
        }

        private void CreateText(object obj, LogType logType)
        {
            var color = colorSettings.GetColor(logType);
            CreateText(obj, color);
        }

        private readonly List<GameObject> _textObjects = new();
        private void CreateText(object obj, Color color)
        {
#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
            if (!t_text) return;

            var text = Instantiate(t_text, t_text.transform.parent);
            text.text = obj.ToString();
            text.color = color;
            text.gameObject.SetActive(true);
            text.transform.SetAsFirstSibling();
            
            _textObjects.Add(text.gameObject);
#endif
        }

        private static readonly object Lock = new();
        private static readonly List<string> LOGBuffer = new();
        private static bool _writingLogs;
        private static string _filePath;
        private const int BufferSize = 10; // Number of logs before writing to file
        private void Start()
        {
            LOGSettings = EnvironmentController.Env.logSettings;
            if (!LOGSettings.saveLogs) return;

            _filePath = Path.Combine(Application.persistentDataPath, "logs.txt");

            Application.logMessageReceived += HandleLog;
        }

        public void Clear()
        {
            for (var index = _textObjects.Count - 1; index >= 0; index--)
            {
                var t = _textObjects[index];
                Destroy(t.gameObject);
            }
            
            _textObjects.Clear();
        }

        private static void HandleLog(string condition, string stackTrace, UnityEngine.LogType type)
        {
            if (!instance.LOGSettings.saveLogs) return;

            if (type is not (UnityEngine.LogType.Error or UnityEngine.LogType.Exception)) return;
            lock (Lock)
            {
                LOGBuffer.Add($"{condition}\n{stackTrace}");

                // If buffer is full, write to file
                if (LOGBuffer.Count >= BufferSize && !_writingLogs)
                {
                    _writingLogs = true;
                    WriteLogsToFile();
                }
            }
        }

        private static void WriteLogsToFile()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (Lock)
                {
                    try
                    {
                        File.AppendAllLines(_filePath, LOGBuffer);
                        LOGBuffer.Clear(); // Clear the buffer after writing
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to write logs to file: {ex}");
                    }
                    finally
                    {
                        _writingLogs = false;
                    }
                }
            });
        }

        private void OnApplicationQuit()
        {
            // Ensure any remaining logs are written to file
            if (LOGBuffer.Count > 0)
            {
                WriteLogsToFile();
            }
        }
    }
}