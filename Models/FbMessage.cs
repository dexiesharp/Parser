using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Models
{
    public class FbMessage
    {
        [Key]
        public int FbMessageId { get; set; }

        public int FbThreadId { get; set; }
        public virtual FbThread FbThread { get; set; }
        public DateTime TimeSent { get; set; }
        public string Text { get; set; }
        public string Author { get; set; } //todo: Replace with user class
    }
}
