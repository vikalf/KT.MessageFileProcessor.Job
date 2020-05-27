using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KT.MessageFileProcessor.Business.Models.Payloads
{

    [Serializable]
    public class StudentEvent
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public int MessageTypeId { get; set; }
        [DataMember]
        public List<ContentItemMessage> Content { get; set; }
        [DataMember]
        public DateTime? Published { get; set; }


        public string ToJsonString() => Newtonsoft.Json.JsonConvert.SerializeObject(this);


    }

    [Serializable]
    public class ContentItemMessage
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }

}
