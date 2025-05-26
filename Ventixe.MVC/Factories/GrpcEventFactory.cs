using Ventixe.MVC.Protos;

namespace Ventixe.MVC.Factories
{
    public static class GrpcEventFactory
    {
        public static EventProto.EventProtoClient CreateEventProtoClient()
        {
            var channel = Grpc.Net.Client.GrpcChannel.ForAddress("https://localhost:7231");
            return new EventProto.EventProtoClient(channel);
        }
    }
}
