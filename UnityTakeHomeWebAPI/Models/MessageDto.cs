using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Net;
using UnityTakeHomeWebAPI.Data;

namespace UnityTakeHomeWebAPI.Models
{
    public class MessageDto
    {
        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public JObject Message { get; set; }

        public string? SentFromIp { get; set; } 

        public int? Priority { get; set; }

        public MessageDto() { }
        public MessageDto(Message entity)
        {
            Timestamp = entity.Timestamp;
            Sender = entity.Sender;
            Message = JsonConvert.DeserializeObject<JObject>(entity.MessageJson);
            SentFromIp = entity.SentFromIp;
            Priority = entity.Priority;
        }

        public Message ConvertToEntity()
        {
            return new Message
            {
                Timestamp = Timestamp,
                Sender = Sender,
                MessageJson = JsonConvert.SerializeObject(Message),
                SentFromIp = SentFromIp,
                Priority = Priority
            };
        }
    }
                                        
}