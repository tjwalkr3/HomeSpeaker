using Grpc.Core;
using Grpc.Net.Client;
using HomeSpeaker.Server;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeSpeaker.Client
{
    public class HomeSpeakerClient : HomeSpeaker.Server.Greeter2.Greeter2Client
    {
        private readonly GrpcChannel channel;
        private readonly Greeter2.Greeter2Client client;

        public HomeSpeakerClient(IConfiguration config)
        {
            channel = GrpcChannel.ForAddress(config["HomeSpeaker.Server"]);
            client = new HomeSpeaker.Server.Greeter2.Greeter2Client(channel);
        }

        public string DoGreeting(string yourValue)
        {
            return client.SayHello(new HelloRequest2 { Name = yourValue }).Message;
        }
    }
}
