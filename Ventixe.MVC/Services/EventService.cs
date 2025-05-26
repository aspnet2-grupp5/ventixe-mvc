using Google.Protobuf.WellKnownTypes;
using Ventixe.MVC.Protos;
using Ventixe.MVC.Factories;

namespace Ventixe.MVC.Services
{
    public class EventService (EventProto.EventProtoClient eventClient)
    {
        private readonly EventProto.EventProtoClient _eventClient = eventClient;

        public async Task<List<Event>> GetAllEventsAsync()
        {
            var reply = await _eventClient.GetAllEventsAsync(new Empty());
            return reply.Events.ToList();
        }

        public async Task<Event?> GetEventByIdAsync(string eventId)
        {
            var reply = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest 
            {
                EventId = eventId 
            });
            if (reply.Event == null)
                return null;

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
            return await _eventClient.DeleteEventAsync(new GetEventByIdRequest 
            {
                EventId = eventId 
            });
        }
    }
}
