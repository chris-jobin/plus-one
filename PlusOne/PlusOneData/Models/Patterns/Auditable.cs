using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlusOneData.Models.Patterns
{
    public abstract class Auditable
    {
        public DateTime Created { get; set; }
    }
}
