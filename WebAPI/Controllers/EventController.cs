using Business.Abstract;
using Entities.Concrete;
using Entities.DTOs.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class EventController : ControllerBase
    {
        IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{id}")]
        public IActionResult ApproveEvent(int eventId)
        {
            var eventToApprove = _eventService.Get(e=>e.Id==eventId);
            if (eventToApprove != null)
            {
                eventToApprove.IsApproved = true;
                _eventService.Update(eventToApprove);
                return Ok();
            }
            else
            {
                return NoContent();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("reject/{id}")]
        public IActionResult RejectEvent(int eventId)
        {
            var eventToReject = _eventService.Get(e => e.Id == eventId);
            if (eventToReject != null)
            {
                eventToReject.IsApproved = false;
                _eventService.Update(eventToReject);
                return Ok();
            }
            else
            {
                return NoContent();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public IActionResult RemoveEvent(int eventId)
        {
            var eventToRemove = _eventService.Get(e => e.Id == eventId);
            if (eventToRemove != null)
            {
                _eventService.Delete(eventToRemove);
                return Ok();
            }
            else
            {
                return NoContent();
            }
        }
        [HttpPost("addevent")]
        public IActionResult AddEvent(EventDto eventDto)
        {
            Event eventEntity = new Event();
            eventEntity.Address = eventDto.Address;
            eventEntity.EventDate = eventDto.EventDate;
            eventEntity.LastApplicationEventDate = eventDto.LastApplicationEventDate;
            eventEntity.CategoryId = eventDto.CategoryId;
            eventEntity.CityId = eventDto.CityId;
            eventEntity.Description = eventDto.Description;
            eventEntity.Quota = eventDto.Quota;
            eventEntity.Name = eventDto.Name;
            eventEntity.IsTicket = eventDto.IsTicket;
            eventEntity.IsApproved = false; //Admin onayı için onay bekleniyor.

            if (eventDto.IsTicket)
            {
                eventEntity.Price = eventDto.Price;
            }

            _eventService.Add(eventEntity);
            return Ok(eventEntity);
        }
    }
}
