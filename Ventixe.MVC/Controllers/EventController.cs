using Microsoft.AspNetCore.Mvc;
using Ventixe.MVC.Protos;
using Ventixe.MVC.Services;

namespace Ventixe.MVC.Controllers
{
    public class EventController : Controller
    {
        private readonly EventService _eventService;

        public EventController()
        {
            _eventService = new EventService();
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllEventsAsync();
            return View(events);
        }

        public async Task<IActionResult> Details(string id)
        {
            var ev = await _eventService.GetEventByIdAsync(id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event newEvent)
        {
            var result = await _eventService.CreateEventAsync(newEvent);
            if (result.StatusCode == 200)
                return RedirectToAction("Index");

            ViewBag.Error = result.Message;
            return View(newEvent);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var ev = await _eventService.GetEventByIdAsync(id);
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Event updatedEvent)
        {
            var result = await _eventService.UpdateEventAsync(updatedEvent);
            if (result.StatusCode == 200)
                return RedirectToAction("Index");

            ViewBag.Error = result.Message;
            return View(updatedEvent);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var ev = await _eventService.GetEventByIdAsync(id);
            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _eventService.DeleteEventAsync(id);
            return RedirectToAction("Index");
        }
    }
}
