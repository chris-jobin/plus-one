using PlusOneData.Models.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlusOneData.Models
{
    public class GameOverMessage : Keyable
    {
        public string Message { get; set; }
        public string Description { get; set; }
    }
}
