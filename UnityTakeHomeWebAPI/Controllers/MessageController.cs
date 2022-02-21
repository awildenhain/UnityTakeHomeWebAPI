using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using UnityTakeHomeWebAPI.Data;
using UnityTakeHomeWebAPI.Models;

namespace UnityTakeHomeWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {        
        private readonly List<string> _acceptableFields = new List<string>()
        {
            "ts",
            "sender",
            "message",
            "sent-from-ip",
            "priority"
        };

        private readonly ILogger<MessageController> _logger;
        private readonly UnityTakeHomeContext _context;
        private Queue<MessageDto> _messages;

        public MessageController(ILogger<MessageController> logger, UnityTakeHomeContext context)
        {
            _logger = logger;
            _context = context;
            
            _messages = new Queue<MessageDto>();
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<IActionResult> GetListAsync()
        {
            try
            {
                var messages = await _context.Messages.Select(x => new MessageDto(x)).ToListAsync();
                return Ok(JsonConvert.SerializeObject(messages));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(string payload)
        {
            try
            {
                var deserializedPayload = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(payload);
                //checking for something that didn't deserialize, or has fewer than the 3 required fields
                if (deserializedPayload == null || deserializedPayload.Count() < 3)
                {
                    return BadRequest($"Payload not valid (ts, sender, and message required).");
                }
                //checking for invalid fields
                foreach (var field in deserializedPayload.Keys)
                {
                    if (!_acceptableFields.Contains(field))
                    {
                        return BadRequest($"{field} is not a valid field.");
                    }
                }

                var newMessage = new MessageDto();

                #region field-by-field validation
                //ASSUMPTION: Unix time stamp expected in seconds, not milliseconds
                DateTimeOffset? timestamp = DateTimeOffset.FromUnixTimeSeconds(Int32.Parse(deserializedPayload["ts"])); // try/catch will catch if int or DateTimeOffset conversion fails
                if (!timestamp.HasValue)
                {
                    return BadRequest($"ts not valid.");
                }
                else
                {
                    newMessage.Timestamp = timestamp.Value;
                }

                string sender = deserializedPayload["sender"];
                if (string.IsNullOrWhiteSpace(sender))
                {
                    return BadRequest($"sender not valid.");
                }
                else
                {
                    newMessage.Sender = sender.Trim();
                }

                JObject message = deserializedPayload["message"]; // try/catch will catch if message is not a JObject
                if ( message.GetType() != typeof(JObject) || message.HasValues == false)
                {
                    return BadRequest($"message not valid.");
                }
                else
                {
                    newMessage.Message = message;
                }

                if (deserializedPayload.ContainsKey("sent-from-ip"))
                {
                    IPAddress? sentFromIP = IPAddress.Parse(deserializedPayload["sent-from-ip"]); // try/catch will catch if IPAddress conversion fails
                    if (sentFromIP == null)
                    {
                        return BadRequest($"sent-from-ip not valid.");
                    }
                    else
                    {
                        newMessage.SentFromIp = deserializedPayload["sent-from-ip"]; // continue to use the string version rather than the IPAddress item since IPAddress requires custom serialization
                    }
                }

                // ASSUMPTION: priority will be an int if valid
                if (deserializedPayload.ContainsKey("priority"))
                {
                    int? priority = (int)deserializedPayload["priority"] ; // try/catch will catch if int cast fails
                    if (priority == null)
                    {
                        return BadRequest($"priority not valid.");
                    }
                    else
                    {
                        newMessage.Priority = priority;
                    }
                }
                #endregion

                // Adding to queue
                _messages.Enqueue(newMessage);

                // Adding to db
                _context.Messages.Add(newMessage.ConvertToEntity());
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}