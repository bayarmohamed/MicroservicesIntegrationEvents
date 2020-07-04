using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceA.Api.Application.Commands
{
    public class CreateMessageCommand:Command
    {
        public string  Message { get; set; }
    }
}
