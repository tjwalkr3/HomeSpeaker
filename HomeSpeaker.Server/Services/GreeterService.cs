using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Server
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }

    public class Greeter2Service : Greeter2.Greeter2Base
    {
        private readonly ILogger<GreeterService> _logger;
        public Greeter2Service(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply2> SayHello(HelloRequest2 request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply2
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
