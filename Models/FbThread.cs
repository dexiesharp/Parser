using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Models
{
    public class FbThread
    {
        [Key]
        public int FbThreadId { get; set; }

        public string Participants { get; set; }

        public virtual List<FbMessage> Messages { get; set; }

        public int MessageCount
        {
            get
            {
                return Messages.Count();
            }
        }
    }
}
