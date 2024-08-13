using PlusOneData.Models.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlusOneData.Models
{
    public class PlusOneEntry : Keyable
    {
        public string ChannelId { get; set; }
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }
        
        public int GetValue() => int.TryParse(Value, out var value) ? value : 0;
    }
}
