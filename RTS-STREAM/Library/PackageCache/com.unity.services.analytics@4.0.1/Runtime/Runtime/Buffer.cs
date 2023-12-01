using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Unity.Services.Analytics.Internal
{
    interface IBuffer
    {
        string UserID { get; set; }
        string InstallID { get; set; }
        string PlayerID { get; set; }
        string SessionID { get; set; }
        string Serialize(List<Buffer.Token> tokens);
        void InsertTokens(List<Buffer.Token> tokens);
        void PushStartEvent(string name, DateTime datetime, Int64? eventVersion, bool addPlayerIdsToEventBody = false);
        void PushEndEvent();
        void PushObjectStart(string name = null);
        void PushObjectEnd();
        void PushArrayStart(string name = null);
        void PushArrayEnd();
        void PushDouble(double val, string name = null);
        void PushFloat(float val, string name = null);
        void PushString(string val, string name = null);
        void PushInt64(Int64 val, string name = null);
        void PushInt(int val, string name = null);
        void PushBool(bool val, string name = null);
        void PushTimestamp(DateTime val, string name = null);
        void FlushToDisk();
        void ClearDiskCache();
        void ClearBuffer();
        void LoadFromDisk();
        void PushEvent(Event evt);
        List<Buffer.Token> CloneTokens();
    }

    /// <summary>
    /// Captures the data as tokens so that can be used to build up the JSON
    /// Later. We do this so as not to serialize inside other peoples functions
    /// but also because we might not have all the info we need to serialize at
    /// that point in time.
    /// This is _NOT_ a thread safe buffer, its the job of the calling code to
    /// handle that.
    /// </summary>
    class Buffer : IBuffer
    {
        public string UserID { get; set; }
        public string SessionID { get; set; }
        public string PlayerID { get; set; }
        public string InstallID { get; set; }

        // With the exception of EventStart, EventEnd, these tokens map to the
        // DDNA JSON Schema. The full schema at the time of writing is. OBJECT,
        // ARRAY, STRING, INTEGER, BOOLEAN, TIMESTAMP, EVENT_TIMESTAMP, FLOAT.
        internal enum TokenType
        {
            EventStart,
            EventEnd,
            EventParamsStart,
            EventParamsEnd,
            EventObjectStart, // Maps to OBJECT
            EventObjectEnd, // Maps to OBJECT
            EventArrayStart, // Maps to ARRAY
            EventArrayEnd, // Maps to ARRAY
            Boolean, // Maps to BOOLEAN
            Float64, // Maps to FLOAT
            String, // Maps to STRING
            Int64, // Maps to INTEGER
            Timestamp, // Maps to TIMESTAMP
            EventTimestamp, // Maps to EVENT_TIMESTAMP
            StandardEventIds,
        }

        // The event information is broken into name, type, and data. The name
        // usually ends up being the key in the JSON and the type and data are
        // for serialization.
        internal struct Token
        {
            public string Name;
            public TokenType Type;

            // NOTE: A union was tried and did show some minor speed increase
            // but it seemed to trigger a bug in the mono runtime, the volume of
            // data running through this should be low enough not to notice
            // anything. using 'object' doesn't trigger the issue.
            public object Data;
        }

        readonly List<Token> m_Tokens = new List<Token>();
        readonly string m_CacheFilePath = CanUseDiskPersistence() ? $"{Application.persistentDataPath}/eventcache" : "";
        readonly long m_CacheFileMaximumSize = 1024 * 1024 * 5; // 5MB

        const string k_NoBufferSupportMessage = "Unity Analytics cache is not supported on this platform, data will not be locally persisted.";

        int m_DiskCacheLastFlushedToken;
        long m_DiskCacheSize;

        public Buffer()
        {
            LoadFromDisk();
            ClearDiskCache();
        }

        public List<Token> CloneTokens()
        {
            var tokens = new List<Token>(m_Tokens);
            m_Tokens.Clear();
            return tokens;
        }

        public void InsertTokens(List<Token> tokens)
        {
            m_Tokens.AddRange(tokens);
        }

        // LOSDK-165 need to be mindful of 5MB limit in future.
        // LOSDK-166 need to honor the enabled list.
        // LOSDK-174 generate the data better.
        /// <summary>
        /// If the DataBuffer knows about the UserID and the Session ID calling this function
        /// will return a JSON blob of all the data in the buffer, it will then clear the
        /// internal data.
        /// </summary>
        /// <returns>String of JSON or Null</returns>
        public string Serialize(List<Token> tokens)
        {
#if UNITY_ANALYTICS_DEVELOPMENT
            Debug.Assert(!string.IsNullOrEmpty(UserID));
            Debug.Assert(!string.IsNullOrEmpty(SessionID));
#endif

            if (tokens.Count == 0)
            {
                return null;
            }

            var data = new StringBuilder();

            // The JSON output has not been tested with DDNA yet, we the ability
            // to actually send the info to the backend next.
            data.Append("{\"eventList\":[");

            foreach (var t in tokens)
            {
                switch (t.Type)
                {
                    case TokenType.EventStart:
                    {
                        data.Append("{");
                        data.Append("\"eventName\":\"");
                        data.Append(t.Name);
                        data.Append("\",");
                        data.Append("\"userID\":\"");
                        data.Append(UserID);
                        data.Append("\",");
                        data.Append("\"sessionID\":\"");
                        data.Append(SessionID);
                        data.Append("\",");
                        data.Append("\"eventUUID\":\"");
                        data.Append(Guid.NewGuid().ToString());
                        data.Append("\",");

#if UNITY_ANALYTICS_EVENT_LOGS
                        Debug.LogFormat("Serializing event {0} for dispatch...", t.Name);
#endif

                        // Session ID and UserID are also needed here.
                        break;
                    }
                    case TokenType.EventEnd:
                    {
                        // object
                        data.Append("},");
                        break;
                    }
                    case TokenType.EventObjectEnd:
                    {
                        if (data[data.Length - 1] == ',')
                        {
                            data.Remove(data.Length - 1, 1);
                        }
                        data.Append("},");
                        break;
                    }
                    case TokenType.EventArrayEnd:
                    {
                        if (data[data.Length - 1] == ',')
                        {
                            data.Remove(data.Length - 1, 1);
                        }
                        data.Append("],");
                        break;
                    }
                    case TokenType.EventParamsStart:
                    {
                        data.Append("\"eventParams\":{");
                        break;
                    }
                    case TokenType.EventParamsEnd:
                    {
                        if (data[data.Length - 1] == ',')
                        {
                            data.Remove(data.Length - 1, 1);
                        }

                        // event params
                        data.Append("}");
                        break;
                    }
                    case TokenType.Float64:
                    {
                        if (null != t.Name)
                        {
                            data.Append("\"");
                            data.Append(t.Name);
                            data.Append("\":");
                        }
                        data.AppendFormat(CultureInfo.InvariantCulture, "{0}", (double)t.Data);
                        data.Append(",");
                        break;
                    }
                    case TokenType.Boolean:
                    {
                        if (null != t.Name)
                        {
                            data.Append("\"");
                            data.Append(t.Name);
                            data.Append("\":");
                        }
                        data.Append((bool)t.Data ? "true" : "false");
                        data.Append(",");
                        break;
                    }
                    case TokenType.Int64:
                    {
                        if (null != t.Name)
                        {
                            data.Append("\"");
                            data.Append(t.Name);
                            data.Append("\":");
                        }
                        data.Append((Int64)t.Data);
                        data.Append(",");
                        break;
                    }
                    case TokenType.String:
                    {
                        if (null != t.Name)
                        {
                            data.Append("\"");
                            data.Append(t.Name);
                            data.Append("\":");
                        }
                        data.Append(JsonConvert.ToString((string)t.Data));
                        data.Append(",");
                        break;
                    }
                    case TokenType.Timestamp:
                    {
                        data.Append("\"");
                        data.Append(t.Name);
                        data.Append("\":\"");
                        data.Append(SaveDateTime((DateTime)t.Data));
                        data.Append("\",");
                        break;
                    }
                    case TokenType.EventTimestamp:
                    {
                        data.Append("\"eventTimestamp\":\"");
                        data.Append(SaveDateTime((DateTime)t.Data));
                        data.Append("\",");
                        break;
                    }
                    case TokenType.EventObjectStart:
                    {
                        if (null != t.Name)
                        {
                            data.Append("\"");
                            data.Append(t.Name);
                            data.Append("\":");
                        }
                        data.Append("{");
                        break;
                    }
                    case TokenType.EventArrayStart:
                    {
                        data.Append("\"");
                        data.Append(t.Name);
                        data.Append("\":");
                        data.Append("[");
                        break;
                    }
                    case TokenType.StandardEventIds:
                    {
                        data.Append("\"unityInstallationID\":\"");
                        data.Append(InstallID);
                        data.Append("\",");

                        if (!string.IsNullOrEmpty(PlayerID))
                        {
                            data.Append("\"unityPlayerID\":\"");
                            data.Append(PlayerID);
                            data.Append("\",");
                        }

                        break;
                    }
                }

                if (t.Type == TokenType.EventEnd && IsRequestOverSizeLimit(data.ToString()))
                {
                    break;
                }
            }

            // JSON doesn't like trailing ',' so we remove the last one.
            if (data[data.Length - 1] == ',')
            {
                data.Remove(data.Length - 1, 1);
            }

            data.Append("]}");

            return data.ToString();
        }

        public static string SaveDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
        }

        static DateTime ParseDateTime(string dateTime)
        {
            return DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
        }

        bool IsRequestOverSizeLimit(string data)
        {
            var byteCount = Encoding.Unicode.GetByteCount(data);
            var byteLimit = 4194304;

            return byteCount >= byteLimit;
        }

        public void PushStartEvent(string name, DateTime datetime, Int64? eventVersion, bool addPlayerIdsToEventBody = false)
        {
#if UNITY_ANALYTICS_EVENT_LOGS
            Debug.LogFormat("Recorded event {0} at {1} (UTC)", name, SaveDateTime(datetime));
#endif

            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.EventStart,
                Data = null
            });

            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.EventTimestamp,
                Data = datetime
            });

            if (eventVersion != null)
            {
                m_Tokens.Add(new Token
                {
                    Name = "eventVersion",
                    Type = TokenType.Int64,
                    Data = eventVersion
                });
            }

            if (addPlayerIdsToEventBody)
            {
                m_Tokens.Add(new Token
                {
                    Name = null,
                    Type = TokenType.StandardEventIds,
                    Data = null
                });
            }

            m_Tokens.Add(new Token
            {
                Name = null,
                Type = TokenType.EventParamsStart,
                Data = null
            });
        }

        public void PushEndEvent()
        {
            m_Tokens.Add(new Token
            {
                Name = null,
                Type = TokenType.EventParamsEnd,
                Data = null
            });

            m_Tokens.Add(new Token
            {
                Name = null,
                Type = TokenType.EventEnd,
                Data = null
            });
        }

        public void PushObjectStart(string name = null)
        {
            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.EventObjectStart,
                Data = null
            });
        }

        public void PushObjectEnd()
        {
            m_Tokens.Add(new Token
            {
                Name = null,
                Type = TokenType.EventObjectEnd,
                Data = null
            });
        }

        public void PushArrayStart(string name = null)
        {
            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.EventArrayStart,
                Data = null
            });
        }

        public void PushArrayEnd()
        {
            m_Tokens.Add(new Token
            {
                Name = null,
                Type = TokenType.EventArrayEnd,
                Data = null
            });
        }

        public void PushDouble(double val, string name = null)
        {
            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.Float64,
                Data = val
            });
        }

        public void PushFloat(float val, string name = null)
        {
            PushDouble(val, name);
        }

        public void PushString(string val, string name = null)
        {
#if UNITY_ANALYTICS_DEVELOPMENT
            Debug.AssertFormat(!string.IsNullOrEmpty(val), "Required to have a value");
#endif

            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.String,
                Data = val
            });
        }

        public void PushInt64(Int64 val, string name = null)
        {
            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.Int64,
                Data = val
            });
        }

        public void PushInt(int val, string name = null)
        {
            PushInt64(val, name);
        }

        public void PushBool(bool val, string name = null)
        {
            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.Boolean,
                Data = val
            });
        }

        public void PushTimestamp(DateTime val, string name)
        {
            m_Tokens.Add(new Token
            {
                Name = name,
                Type = TokenType.Timestamp,
                Data = val
            });
        }

        public void FlushToDisk()
        {
            if (!CanUseDiskPersistence())
            {
                Debug.Log(k_NoBufferSupportMessage);
                return;
            }

            if (m_DiskCacheSize > m_CacheFileMaximumSize)
            {
                // Cache is full, do not keep spaffing into it.
                return;
            }

            using (var stream = File.Open(m_CacheFilePath, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8))
                {
                    writer.Seek(0, SeekOrigin.End);

                    for (var i = m_DiskCacheLastFlushedToken; i < m_Tokens.Count; i++)
                    {
                        WriteToken(writer, m_Tokens[i]);
                        m_DiskCacheLastFlushedToken++;
                    }

                    m_DiskCacheSize = stream.Length;
                    Debug.Log($"Flushed up to token index {m_DiskCacheLastFlushedToken}, cache file is {m_DiskCacheSize}B");
                }
            }
        }

        public void ClearDiskCache()
        {
            if (!CanUseDiskPersistence())
            {
                Debug.Log(k_NoBufferSupportMessage);
                return;
            }
            m_DiskCacheLastFlushedToken = 0;
            if (File.Exists(m_CacheFilePath))
            {
                File.Delete(m_CacheFilePath);
            }
        }

        public void ClearBuffer()
        {
            m_Tokens.Clear();
            ClearDiskCache();
        }

        public void LoadFromDisk()
        {
            if (!CanUseDiskPersistence())
            {
                Debug.Log(k_NoBufferSupportMessage);
                return;
            }

            m_Tokens.Clear();
            if (File.Exists(m_CacheFilePath))
            {
                try
                {
                    var incomingEvents = new List<Token>();
                    using (var stream = File.Open(m_CacheFilePath, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream, Encoding.UTF8))
                        {
                            var length = stream.Length;
                            while (stream.Position != length)
                            {
                                incomingEvents.Add(ReadToken(reader));
                            }

                            m_Tokens.AddRange(incomingEvents);
                            m_DiskCacheSize = length;
                            m_DiskCacheLastFlushedToken = m_Tokens.Count - 1;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error loading cached events: file was corrupt (probably due to improper app/system shutdown). Cached events have been discarded and other operations will continue as normal. Error was {e.Message}");
                    m_DiskCacheSize = 0;
                    m_DiskCacheLastFlushedToken = 0;
                }
            }
        }

        void WriteToken(BinaryWriter writer, Token token)
        {
            writer.Write((int)token.Type);

            var hasName = null != token.Name;
            writer.Write(hasName);
            if (hasName)
            {
                writer.Write(token.Name);
            }

            switch (token.Type)
            {
                case TokenType.Boolean:
                    writer.Write((bool)token.Data);
                    break;
                case TokenType.Int64:
                    writer.Write((long)token.Data);
                    break;
                case TokenType.Float64:
                    writer.Write((double)token.Data);
                    break;
                case TokenType.String:
                    writer.Write((string)token.Data);
                    break;
                case TokenType.Timestamp:
                case TokenType.EventTimestamp:
                    writer.Write(SaveDateTime((DateTime)token.Data));
                    break;
            }
        }

        Token ReadToken(BinaryReader reader)
        {
            var token = new Token
            {
                Type = (TokenType)reader.ReadInt32()
            };

            var hasName = reader.ReadBoolean();
            if (hasName)
            {
                token.Name = reader.ReadString();
            }

            switch (token.Type)
            {
                case TokenType.Boolean:
                    token.Data = reader.ReadBoolean();
                    break;
                case TokenType.Int64:
                    token.Data = reader.ReadInt64();
                    break;
                case TokenType.Float64:
                    token.Data = reader.ReadDouble();
                    break;
                case TokenType.String:
                    token.Data = reader.ReadString();
                    break;
                case TokenType.Timestamp:
                case TokenType.EventTimestamp:
                    token.Data = ParseDateTime(reader.ReadString());
                    break;
            }

            return token;
        }

        public void PushEvent(Event evt)
        {
            // Serialize event

            var dateTime = DateTime.Now;
            PushStartEvent(evt.Name, dateTime, evt.Version);

            // Serialize event params

            var eData = evt.Parameters;

            foreach (var data in eData.Data)
            {
                if (data.Value is float f32Val)
                {
                    PushFloat(f32Val, data.Key);
                }
                else if (data.Value is double f64Val)
                {
                    PushDouble(f64Val, data.Key);
                }
                else if (data.Value is string strVal)
                {
                    PushString(strVal, data.Key);
                }
                else if (data.Value is int intVal)
                {
                    PushInt(intVal, data.Key);
                }
                else if (data.Value is Int64 int64Val)
                {
                    PushInt64(int64Val, data.Key);
                }
                else if (data.Value is bool boolVal)
                {
                    PushBool(boolVal, data.Key);
                }
            }

            PushEndEvent();
        }

        static bool CanUseDiskPersistence()
        {
            // Switch requires a specific setup to have write access to the disc so it won't be handled here.
            return
                Application.platform != RuntimePlatform.Switch &&
#if !UNITY_2021_1_OR_NEWER
                Application.platform != RuntimePlatform.XboxOne &&
#endif
                Application.platform != RuntimePlatform.GameCoreXboxOne &&
                Application.platform != RuntimePlatform.GameCoreXboxSeries
                && !string.IsNullOrEmpty(Application.persistentDataPath);
        }
    }

    class BufferRevoked : IBuffer
    {
        public string UserID { get; set; }
        public string InstallID { get; set; }
        public string PlayerID { get; set; }
        public string SessionID { get; set; }

        public void ClearBuffer()
        {
        }

        public void ClearDiskCache()
        {
        }

        public List<Buffer.Token> CloneTokens()
        {
            return new List<Buffer.Token>();
        }

        public void InsertTokens(List<Buffer.Token> tokens)
        {
        }

        public void FlushToDisk()
        {
        }

        public void LoadFromDisk()
        {
        }

        public void PushArrayEnd()
        {
        }

        public void PushArrayStart(string name = null)
        {
        }

        public void PushBool(bool val, string name = null)
        {
        }

        public void PushDouble(double val, string name = null)
        {
        }

        public void PushEndEvent()
        {
        }

        public void PushEvent(Event evt)
        {
        }

        public void PushFloat(float val, string name = null)
        {
        }

        public void PushInt(int val, string name = null)
        {
        }

        public void PushInt64(long val, string name = null)
        {
        }

        public void PushObjectEnd()
        {
        }

        public void PushObjectStart(string name = null)
        {
        }

        public void PushStartEvent(string name, DateTime datetime, long? eventVersion, bool addPlayerIdsToEventBody = false)
        {
        }

        public void PushString(string val, string name = null)
        {
        }

        public void PushTimestamp(DateTime val, string name = null)
        {
        }

        public string Serialize(List<Buffer.Token> tokens)
        {
            return String.Empty;
        }
    }
}
