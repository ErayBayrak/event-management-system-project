using Business.Abstract;
using Entities.Concrete;
using Entities.DTOs.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EventController : ControllerBase
    {
        IEventService _eventService;
        IUserService _userService;
        public EventController(IEventService eventService,IUserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{eventId}")]
        public IActionResult ApproveEvent(int eventId)
        {
            var eventToApprove = _eventService.Get(e => e.Id == eventId);
            if (eventToApprove != null)
            {
                eventToApprove.IsApproved = true;
                _eventService.Update(eventToApprove);
                return Ok("Etkinlik kabul edildi.");
            }
            else
            {
                return BadRequest();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("reject/{eventId}")]
        public IActionResult RejectEvent(int eventId)
        {
            var eventToReject = _eventService.Get(e => e.Id == eventId);
            if (eventToReject != null)
            {
                eventToReject.IsApproved = false;
                _eventService.Update(eventToReject);
                return Ok("Etkinlik reddedildi.");
            }
            else
            {
                return BadRequest();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteevent/{eventId}")]
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
            eventEntity.UserId = eventDto.UserId;
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
        private User GetCurrentUser()
        {
            var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return null;
            }

            var userEmail = userEmailClaim.Value;
            var user = _userService.GetByMail(userEmail);
            return user;
        }

        [Authorize]
        [HttpPut("updateownevent/{eventId}")]
        public IActionResult UpdateOwnEvent(int eventId, UpdateOwnEventDto updateOwnEventDto)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return BadRequest("Kullanıcı bilgisi alınamadı.");
            }

            var userEvent = _eventService.Get(e => e.Id == eventId && e.UserId == user.Id);

            if (userEvent == null)
            {
                return NotFound("Etkinlik bulunamadı veya yetkisiz erişim.");
            }
            TimeSpan remainingDays = userEvent.LastApplicationEventDate - DateTime.Today;
            if (remainingDays.TotalDays > 5)
            {
                userEvent.Address = updateOwnEventDto.Address;
                userEvent.Quota = updateOwnEventDto.Quota;
                _eventService.Update(userEvent);
                return Ok("Etkinlik bilgileri başarıyla güncellendi.");
            }
            
            return Ok("Etkinlik zamanında kısa süre kaldığı için güncellenemedi.");
        }
        [Authorize]
        [HttpDelete("deleteownevent/{eventId}")]
        public IActionResult DeleteOwnEvent(int eventId)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return BadRequest("Kullanıcı bilgisi alınamadı.");
            }

            var userEvent = _eventService.Get(e => e.Id == eventId && e.UserId == user.Id);

            if (userEvent == null)
            {
                return NotFound("Etkinlik bulunamadı veya yetkisiz erişim.");
            }
            TimeSpan remainingDays = userEvent.LastApplicationEventDate - DateTime.Today;
            if (remainingDays.TotalDays > 5)
            {
                _eventService.Delete(userEvent);
                return Ok("Etkinlik başarıyla iptal edildi.");
            }
            return Ok("Etkinliği silemezsiniz.");
        }
        [Authorize] 
        [HttpGet("myevents")]
        public IActionResult GetUserEvents()
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return BadRequest("Kullanıcı bilgisi alınamadı.");
            }

            var userEvents = _eventService.GetAll(e => e.UserId == currentUser.Id);

            return Ok(userEvents);
        }
    }
}
