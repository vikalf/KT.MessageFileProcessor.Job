using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KT.MessageFileProcessor.Business.Models.XmlModel
{
    [Serializable]
    public class BiztalkMessage
    {
        [JsonProperty("ns0:MsgLog")]
        public MsgLog MsgLog { get; set; }
    }

    [Serializable]
    public class MsgLog
    {
        [JsonProperty("ns1:LogEntry")]
        public LogEntry LogEntry { get; set; }

        [JsonProperty("LogEntry")]
        public List<LogEntry> LogEntries { get; set; }
    }

    [Serializable]
    public class LogEntry
    {
        [JsonProperty("MsgType")]
        public string MsgType { get; set; }

        [JsonProperty("User")]
        public string User { get; set; }

        [JsonProperty("DataElement")]
        public MsgData MsgData { get; set; }

        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }

    }

    [Serializable]
    public class MsgData
    {
        [JsonProperty("Item")]
        public Object Item { get; set; }
    }

    [Serializable]
    public class MsgDataItem
    {
        [JsonProperty("@Key")]
        public string Key { get; set; }

        [JsonProperty("#text")]
        public string Value { get; set; }
    }
}
