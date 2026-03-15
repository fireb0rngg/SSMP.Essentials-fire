using SSMP.Api.Command.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSMP_Utils.Client.Commands
{
    internal interface IDescriptiveClientCommand : IClientCommand
    {
        public string Description { get; }

    }
}
