using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceA.Api.Application.Commands
{
    public abstract class Command:IRequest<bool>
    {
    }
}
