using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace Arcube.AssetManagement
{
    public class Meta
    {
        public string key;
        public int views;
        public bool liked;
        public bool saved;
        public int shares;

        [JsonIgnore] public DateTime createdAt;
        [JsonIgnore] public DateTime modifiedAt;

        private string FilePath { get; set; }
        public Meta(string key)
        {
            this.key = key;
            FilePath = GetPath(key);
        }

        public Meta AddViews()
        {
            views++;
            Save();
            return this;
        }

        public Meta AddShares()
        {
            shares++;
            Save();
            return this;
        }

        public Meta SetLiked(bool liked)
        {
            this.liked = liked;
            Save();
            return this;
        }

        public Meta SetSaved(bool saved)
        {
            this.saved = saved;
            Save();
            return this;
        }

        public void Save()
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var data = JsonConvert.SerializeObject(this);
            File.WriteAllTextAsync(FilePath, data);
        }

        public static string GetPath(string key) => Path.Combine(Application.persistentDataPath, "meta", $"{key}.meta");

        public static Meta LoadOrCreate(string key)
        {
            var path = GetPath(key);
            if (!File.Exists(path) || string.IsNullOrWhiteSpace(File.ReadAllText(path)))
            {
                return new Meta(key);
            }
            
            var data = File.ReadAllText(path);
            if (string.IsNullOrEmpty(data))
            {
                return new Meta(key);
            }

            var meta = JsonConvert.DeserializeObject<Meta>(data);
            if (meta == null)
            {
                return new Meta(key);
            }

            meta.key = key;
            meta.createdAt = File.GetCreationTime(path);
            meta.modifiedAt = File.GetLastWriteTime(path);

            return meta;
        }
    }
}