using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnityTakeHomeWebAPI.Data
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Sender { get; set; }

        public string MessageJson { get; set; }

        [MaxLength(50)]
        public string? SentFromIp { get; set; }

        public int? Priority { get; set; }
    }
}
