using Google.Protobuf.WellKnownTypes;
using Ventixe.MVC.Protos;
using Ventixe.MVC.Factories;

namespace Ventixe.MVC.Services
{
    public class EventService
    {
        private readonly EventProto.EventProtoClient _eventClient;

        public EventService()
        {
            _eventClient = GrpcEventFactory.CreateEventProtoClient();
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            var reply = await _eventClient.GetAllEventsAsync(new Empty());
            return reply.Events.ToList();
        }

        public async Task<Event?> GetEventByIdAsync(string eventId)
        {
            var reply = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = eventId });
            return reply.Event;
        }

        public async Task<EventReply> CreateEventAsync(Event newEvent)
        {
            return await _eventClient.CreateEventAsync(newEvent);
        }

        public async Task<EventReply> UpdateEventAsync(Event updatedEvent)
        {
            return await _eventClient.UpdateEventAsync(updatedEvent);
        }

        public async Task<EventReply> DeleteEventAsync(string eventId)
        {
            return await _eventClient.DeleteEventAsync(new GetEventByIdRequest { EventId = eventId });
        }
    }
}
