using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vampsdemo.Models
{
    public class DisplayUser
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string phonenumber { get; set; }
        public string email { get; set; }
        public DateTime Birthdate { get; set; }
        public string Role { get; set; }
    }
}