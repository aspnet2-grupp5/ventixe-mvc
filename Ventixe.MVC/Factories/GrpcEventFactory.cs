using Ventixe.MVC.Models;
using Ventixe.MVC.Models.Events;
using Ventixe.MVC.Protos;

namespace Ventixe.MVC.Services
{
    public interface IGrpcEventFactory
    {
        Event ToProto(CreateEventViewModel model);
        CreateEventViewModel ToViewModel(Event protoEvent);
        EventViewModel ToEventViewModel(Event protoEvent);
    }
    public class GrpcEventFactory : IGrpcEventFactory
    {
        public Event ToProto(CreateEventViewModel model)
        {
            return new Event
            {
                EventId = model.EventId ?? Guid.NewGuid().ToString(),
                EventTitle = model.EventTitle,
                Description = model.Description,
                EventImage = model.Image,
                Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(model.Date.ToUniversalTime()),
                Price = model.Price,
                Quantity = model.Quantity,
                SoldQuantity = model.SoldQuantity,
                Category = string.IsNullOrWhiteSpace(model.CategoryId) ? null : new Category { CategoryId = model.CategoryId },
                Location = string.IsNullOrWhiteSpace(model.LocationId) ? null : new Location { LocationId = model.LocationId },
                Status = string.IsNullOrWhiteSpace(model.StatusId) ? null : new Status { StatusId = model.StatusId }
            };
        }

        public CreateEventViewModel ToViewModel(Event protoEvent)
        {
            return new CreateEventViewModel
            {
                EventId = protoEvent.EventId,
                EventTitle = protoEvent.EventTitle,
                Description = protoEvent.Description,
                Image = protoEvent.EventImage,
                Date = protoEvent.Date.ToDateTime().ToLocalTime(),
                Price = protoEvent.Price,
                Quantity = protoEvent.Quantity,
                SoldQuantity = protoEvent.SoldQuantity,
                CategoryId = protoEvent.Category?.CategoryId,
                LocationId = protoEvent.Location?.LocationId,
                StatusId = protoEvent.Status?.StatusId
            };
        }
        public EventViewModel ToEventViewModel(Event protoEvent)
        {
            return new EventViewModel
            {
                EventId = protoEvent.EventId,
                EventTitle = protoEvent.EventTitle,
                EventImage = protoEvent.EventImage,
                Description = protoEvent.Description,
                Date = protoEvent.Date?.ToDateTime(),
                Price = (decimal)protoEvent.Price,
                Quantity = protoEvent.Quantity,
                SoldQuantity = protoEvent.SoldQuantity,
                Location = protoEvent.Location != null ? $"{protoEvent.Location.City}, {protoEvent.Location.Address}" : null,
                Category = protoEvent.Category?.CategoryName,
                Status = protoEvent.Status?.StatusName
            };
        }
    }
}
