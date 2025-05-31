using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ventixe.MVC.Models;
using Ventixe.MVC.Protos;
using Ventixe.MVC.Services;

namespace Ventixe.MVC.Controllers
{
    public class EventsController(
        IEventService eventService,
        CategoryProto.CategoryProtoClient categoryClient,
        LocationProto.LocationProtoClient locationClient,
        StatusProto.StatusProtoClient statusClient,
        IGrpcEventFactory grpcEventFactory) : Controller
    {
        private readonly IEventService _eventService = eventService;
        private readonly CategoryProto.CategoryProtoClient _categoryClient = categoryClient;
        private readonly LocationProto.LocationProtoClient _locationClient = locationClient;
        private readonly StatusProto.StatusProtoClient _statusClient = statusClient;
        private readonly IGrpcEventFactory _grpcEventFactory = grpcEventFactory;


        [Route("Events")]
        public async Task<IActionResult> Index(string? status)
        {
            ViewData["Title"] = "Events";
            ViewData["CurrentFilter"] = status; // För att markera vald filter i vyn

            var eventsResult = await _eventService.GetAllEventsAsync();

            var model = eventsResult.Events
                .Select(e => _grpcEventFactory.ToEventViewModel(e))
                .ToList();

            if (!string.IsNullOrEmpty(status))
            {
                model = model
                    .Where(e => e.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var response = await _eventService.GetEventByIdAsync(id);

            if (response.StatusCode != 200 || response.Event == null)
                return NotFound();

            var model = _grpcEventFactory.ToEventViewModel(response.Event);

            return View(model);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateEventViewModel();
            await LoadDropdownData(vm);
            return View(vm);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData(model);
                return View(model);
            }

            var grpcEvent = _grpcEventFactory.ToProto(model);

            var result = await _eventService.CreateEventAsync(grpcEvent);

            if (result.StatusCode != 200)
            {
                ModelState.AddModelError("", result.Message ?? "Failed to create event.");
                await LoadDropdownData(model);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet("Events/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Invalid ID");

            var response = await _eventService.GetEventByIdAsync(id);
            if (response.StatusCode != 200 || response.Event is null)
                return NotFound();

            var model = _grpcEventFactory.ToViewModel(response.Event);
            await LoadDropdownData(model);

            return View(model);
        }
        [HttpPost("Events/Edit/{id}")]
        public async Task<IActionResult> Edit(string id, CreateEventViewModel model)
        {
            if (id != model.EventId)
            {
                ModelState.AddModelError("", "ID mismatch.");
                await LoadDropdownData(model);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownData(model);
                return View(model);
            }

            var grpcEvent = _grpcEventFactory.ToProto(model);
            var result = await _eventService.UpdateEventAsync(grpcEvent);

            if (result.StatusCode != 200)
            {
                ModelState.AddModelError("", result.Message ?? "Update failed.");
                await LoadDropdownData(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Events/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Invalid ID");

            var response = await _eventService.GetEventByIdAsync(id);
            if (response.StatusCode != 200 || response.Event is null)
                return NotFound();

            var model = _grpcEventFactory.ToViewModel(response.Event);
            return View(model);
        }

        [HttpPost("Events/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Invalid ID");

            var result = await _eventService.DeleteEventAsync(id);
            if (result.StatusCode != 200)
            {
                ViewData["ErrorMessage"] = result.Message ?? "Delete failed.";
                return View("Error");
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task LoadDropdownData(CreateEventViewModel vm)
        {
            var categoriesReply = await _categoryClient.GetAllCategoriesAsync(new Empty());
            var locationsReply = await _locationClient.GetAllLocationsAsync(new Empty());
            var statusesReply = await _statusClient.GetAllStatusesAsync(new Empty());

            vm.Categories = categoriesReply.Category
                .Select(c => new SelectListItem { Value = c.CategoryId, Text = c.CategoryName })
                .ToList();

            vm.Locations = locationsReply.Location
                .Select(l => new SelectListItem { Value = l.LocationId, Text = $"{l.City}, {l.Address}" })
                .ToList();

            vm.Statuses = statusesReply.Status
                .Select(s => new SelectListItem { Value = s.StatusId, Text = s.StatusName })
                .ToList();
        }

        // Filtrera efter kategori
        public async Task<IActionResult> FilterByCategory(string categoryId)
        {
            var response = await _eventService.GetEventsByCategoryAsync(categoryId);
            return View("Index", response.Events);
        }

        // Filtrera efter plats
        public async Task<IActionResult> FilterByLocation(string locationId)
        {
            var response = await _eventService.GetEventsByLocationAsync(locationId);
            return View("Index", response.Events);
        }

        // Filtrera efter status
        public async Task<IActionResult> FilterByStatus(string statusId)
        {
            var response = await _eventService.GetEventsByStatusAsync(statusId);
            return View("Index", response.Events);
        }
    }
}