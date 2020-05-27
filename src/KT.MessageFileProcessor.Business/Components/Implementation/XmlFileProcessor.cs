using KT.MessageFileProcessor.Business.Components.Definition;
using KT.MessageFileProcessor.Business.Models.Payloads;
using KT.MessageFileProcessor.Business.Models.XmlModel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace KT.MessageFileProcessor.Business.Components.Implementation
{
    public class XmlFileProcessor : IXmlFileProcessor
    {

        private readonly ILogger<XmlFileProcessor> _logger;

        public XmlFileProcessor(ILogger<XmlFileProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<List<StudentEvent>> GetStudentEventsFromXmlStream(string fileName, Stream blob)
        {
            try
            {
                StreamReader reader = new StreamReader(blob);
                var text = await reader.ReadToEndAsync();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                string json = JsonConvert.SerializeXmlNode(doc);

                var biztalkMessages = JsonConvert.DeserializeObject<BiztalkMessage>(json);

                List<StudentEvent> result = MapStudentEvents(biztalkMessages);

                result.ForEach(e => e.FileName = fileName);

                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStudentEventsFromXmlStream");
                throw;
            }

        }

        private List<StudentEvent> MapStudentEvents(BiztalkMessage biztalkMessages)
        {
            var result = new List<StudentEvent>();

            if (biztalkMessages != null && biztalkMessages.MsgLog != null)
            {

                if (biztalkMessages.MsgLog.LogEntries != null && biztalkMessages.MsgLog.LogEntries.Any())
                {
                    var messages = biztalkMessages.MsgLog.LogEntries.Select(e => MapStudentEvent(e)).ToList();
                    result.AddRange(messages);
                }
                else if (biztalkMessages.MsgLog.LogEntry != null)
                {
                    result.Add(MapStudentEvent(biztalkMessages.MsgLog.LogEntry));
                }
            }

            return result;
        }

        private StudentEvent MapStudentEvent(LogEntry logEntry)
        {
            var message = new StudentEvent
            {
                UserId = int.Parse(logEntry.User),
                MessageTypeId = int.Parse(logEntry.MsgType),
                Content = new List<ContentItemMessage>()
            };

            if (DateTime.TryParse(logEntry.CreatedDate, out DateTime dtCreatedDate))
                message.Published = dtCreatedDate;

            if (logEntry.MsgData?.Item != null)
            {
                var itemJson = JsonConvert.SerializeObject(logEntry.MsgData.Item);
                var token = JToken.Parse(itemJson);

                if (token is JArray)
                {
                    List<MsgDataItem> items = JsonConvert.DeserializeObject<List<MsgDataItem>>(itemJson);
                    message.Content = items.Select(e => new ContentItemMessage { Key = e.Key, Value = e.Value }).ToList();
                }
                else if (token is JObject)
                {
                    var singleItem = JsonConvert.DeserializeObject<MsgDataItem>(itemJson);
                    message.Content = new List<ContentItemMessage>
                    {
                        new ContentItemMessage
                        {
                             Key = singleItem.Key,
                             Value = singleItem.Value
                        }
                    };
                }
            }



            return message;
        }



    }

}
