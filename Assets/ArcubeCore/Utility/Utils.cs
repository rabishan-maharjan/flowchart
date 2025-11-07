using System.Globalization;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Arcube
{
    public struct Vector2Simple
    {
        public int x;
        public int y;

        public Vector2Simple(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Simple(Vector2 v)
        {
            x = (int)v.x;
            y = (int)v.y;
        }

        public Vector2 ToVector2() => new(x, y);
    }
    
    public static class Utils
    {
        // Generates a random string of a given length
        public static string GenerateRandomString(int length)
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new System.Random();

            var stringChars = new char[length];

            for (var i = 0; i < length; i++)
            {
                stringChars[i] = characters[random.Next(characters.Length)];
            }

            return new string(stringChars);
        }

        public static bool RandomBool => Random.Range(0, 2) == 1;

        public static string FormatNumber(int val)
        {
            float count;
            string result;
            if (val < 1000)
            {
                result = val.ToString();
            }
            else if (val < 1000000)
            {
                count = (float)val / 1000;
                result = $"{count:0.0} K";
            }
            else
            {
                count = (float)val / 1000000;
                result = $"{count:0.0} M";
            }

            return result;
        }

        public static string GetTimeString(float val)
        {
            var minutes = Mathf.FloorToInt(val / 60);
            float seconds = Mathf.FloorToInt(val % 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public static string FormatSize(long size)
        {
            string val;
            var s = (size / 1024.0f / 1024);
            val = $"{s:F2} MB";
            if (s > 1024)
            {
                s /= 1024;
                val = $"{s:F2} GB";
            }

            return val;
        }

        public static bool TryParseVector3(string value, out Vector3 result)
        {
            var sArray = value.Split(',');
            result = Vector3.zero;

            if (!float.TryParse(sArray[0], out result.x)) return false;
            if (!float.TryParse(sArray[1], out result.y)) return false;
            if (!float.TryParse(sArray[2], out result.z)) return false;

            return true;
        }

        public static bool TryParseVector2(string value, out Vector2 result)
        {
            string[] sArray = value.Split(',');
            result = Vector3.zero;

            if (!float.TryParse(sArray[0], out result.x)) return false;
            if (!float.TryParse(sArray[1], out result.y)) return false;

            return true;
        }

        public static Vector3 RandomVector(Vector3 range, float maxFactor)
        {
            return new Vector3(Random.Range(range.x, range.x * maxFactor), Random.Range(range.y, range.y * maxFactor),
                Random.Range(range.z, range.z * maxFactor)) * (RandomBool ? 1 : -1);
        }

        public static Vector3 RandomVector(Vector3 min, Vector3 max) => new(Random.Range(min.x, max.x),
            Random.Range(min.y, max.y), Random.Range(min.z, max.z));

        public static T[] RandomizeArray<T>(T[] array)
        {
            var rng = new System.Random();
            var n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            return array;
        }

        public static string ToTitleCase(string text)
        {
            string[] smallWords =
            {
                "a", "an", "and", "as", "at", "but", "by", "for", "in", "nor", "of", "on", "or", "so", "the", "to",
                "up", "yet"
            };
            string[] words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (i == 0 || !Array.Exists(smallWords, w => w.Equals(words[i], StringComparison.OrdinalIgnoreCase)))
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]);
                }
                else
                {
                    words[i] = words[i].ToLower();
                }
            }

            return string.Join(" ", words);
        }

        // Helper function to calculate the screen-space rect of a RectTransform
        public static Rect GetScreenRect(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            Vector3 topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

            return new Rect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x - bottomLeft.x,
                topRight.y - bottomLeft.y
            );
        }
    }
}