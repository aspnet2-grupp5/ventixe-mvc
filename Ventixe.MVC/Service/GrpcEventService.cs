using Google.Protobuf.WellKnownTypes;
using Ventixe.MVC.Protos;
using static Ventixe.MVC.Protos.CategoryProto;
using static Ventixe.MVC.Protos.EventProto;
using static Ventixe.MVC.Protos.LocationProto;
using static Ventixe.MVC.Protos.StatusProto;

namespace Ventixe.MVC.Services
{
    public interface IEventService
    {
        Task<EventReply> CreateEventAsync(Event eventAdd);
        Task<GetAllEventsReply> GetAllEventsAsync();
        Task<GetEventByIdReply> GetEventByIdAsync(string eventId);
        Task<EventReply> UpdateEventAsync(Event updatedEvent);
        Task<EventReply> DeleteEventAsync(string eventId);
        Task<GetAllCategoriesReply> GetAllCategoriesAsync();
        Task<GetAllLocationsReply> GetAllLocationsAsync();
        Task<GetAllStatusesReply> GetAllStatusesAsync();
        Task<GetAllEventsReply> GetEventsByCategoryAsync(string categoryId);
        Task<GetAllEventsReply> GetEventsByLocationAsync(string locationId);
        Task<GetAllEventsReply> GetEventsByStatusAsync(string statusId);

    }
    public class GrpcEventService(EventProtoClient eventClient,
    CategoryProtoClient categoryClient,
    LocationProtoClient locationClient,
    StatusProtoClient statusClient) : IEventService
    {

        private readonly EventProtoClient _eventClient = eventClient;
        private readonly CategoryProtoClient _categoryClient = categoryClient;
        private readonly LocationProtoClient _locationClient = locationClient;
        private readonly StatusProtoClient _statusClient = statusClient;

        public async Task<EventReply> CreateEventAsync(Event eventAdd)
        {
            try
            {
                var response = await _eventClient.CreateEventAsync(eventAdd);
                return response;
            }
            catch (Exception ex)
            {
                return new EventReply { StatusCode = 500, Message = $"Failed to create event: {ex.Message}" };
            }
        }
        public async Task<EventReply> DeleteEventAsync(string eventId)
        {
            var reply = _eventClient.DeleteEventAsync(new GetEventByIdRequest { EventId = eventId });
            try
            {
                return await reply;
            }
            catch (Exception ex)
            {
                return new EventReply { StatusCode = 500, Message = $"Failed to delete event: {ex.Message}" };
            }
        }

        public async Task<GetAllCategoriesReply> GetAllCategoriesAsync()
        {
            var reply = _categoryClient.GetAllCategoriesAsync(new Empty());
            return await reply;
        }

        public async Task<GetAllEventsReply> GetAllEventsAsync()
        {
            var reply = _eventClient.GetAllEventsAsync(new Empty());
            return await reply;
        }

        public async Task<GetAllLocationsReply> GetAllLocationsAsync()
        {
            var reply = _locationClient.GetAllLocationsAsync(new Empty());
            return await reply;
        }

        public async Task<GetAllStatusesReply> GetAllStatusesAsync()
        {
            var reply = _statusClient.GetAllStatusesAsync(new Empty());
            return await reply;
        }

        public async Task<GetEventByIdReply> GetEventByIdAsync(string eventId)
        {
            var reply = _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = eventId });
            try
            {
                return await reply;
            }
            catch (Exception)
            {
                return new GetEventByIdReply { StatusCode = 500 };
            }
        }

        public async Task<EventReply> UpdateEventAsync(Event updatedEvent)
        {
            var reply = _eventClient.UpdateEventAsync(updatedEvent);
            try
            {
                return await reply;
            }
            catch (Exception ex)
            {
                return new EventReply { StatusCode = 500, Message = $"Failed to update event: {ex.Message}" };
            }
        }
        public async Task<GetAllEventsReply> GetEventsByCategoryAsync(string categoryId)
        {
            var request = new CategoryIdRequest { CategoryId = categoryId };
            try
            {
                return await _eventClient.GetEventsByCategoryAsync(request);
            }
            catch (Exception)
            {
                return new GetAllEventsReply();
            }
        }
        public async Task<GetAllEventsReply> GetEventsByLocationAsync(string locationId)
        {
            var request = new LocationIdRequest { LocationId = locationId };
            try
            {
                return await _eventClient.GetEventsByLocationAsync(request);
            }
            catch (Exception)
            {
                return new GetAllEventsReply();
            }
        }
        public async Task<GetAllEventsReply> GetEventsByStatusAsync(string statusId)
        {
            var request = new StatusIdRequest { StatusId = statusId };
            try
            {
                return await _eventClient.GetEventsByStatusAsync(request);
            }
            catch (Exception)
            {
                return new GetAllEventsReply();
            }
        }
    }
}


