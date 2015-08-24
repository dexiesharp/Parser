using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Models
{
    public class FbUser
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
