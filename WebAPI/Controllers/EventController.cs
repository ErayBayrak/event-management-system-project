using Business.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.DTOs.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EventController : ControllerBase
    {
        IEventService _eventService;
        IUserService _userService;
        IAttendanceService _attendanceService;
        Context context;
        public EventController(IEventService eventService, IUserService userService, IAttendanceService attendanceService)
        {
            _eventService = eventService;
            _userService = userService;
            context = new Context();
            _attendanceService = attendanceService;
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
        [Authorize]
        [HttpGet("allevents")]
        public IActionResult GetEvents()
        {

            List<EventCityCategoryDto> events = (
                from e in context.Events
                join ca in context.Categories on e.CategoryId equals ca.Id
                join ci in context.Cities on e.CityId equals ci.Id
                where e.IsApproved
                select new EventCityCategoryDto()
                {
                    CategoryName = ca.Name,
                    CityName = ci.Name,
                    Name = e.Name,
                    Address = e.Address,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    IsTicket = e.IsTicket,
                    LastApplicationEventDate = e.LastApplicationEventDate,
                    Price = e.Price,
                    Quota = e.Quota,
                }).ToList();
            return Ok(events);
        }
        [Authorize]
        [HttpGet("filterevents")]
        public IActionResult FilterEvents(string? categoryName, string? cityName)
        {
            List<EventCityCategoryDto> events = (
                from e in context.Events
                join ca in context.Categories on e.CategoryId equals ca.Id
                join ci in context.Cities on e.CityId equals ci.Id
                where ca.Name == categoryName && ci.Name == cityName && e.IsApproved
                select new EventCityCategoryDto()
                {
                    CategoryName = ca.Name,
                    CityName = ci.Name,
                    Name = e.Name,
                    Address = e.Address,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    IsTicket = e.IsTicket,
                    LastApplicationEventDate = e.LastApplicationEventDate,
                    Price = e.Price,
                    Quota = e.Quota,
                }).ToList();
            return Ok(events);
        }

        [Authorize]
        [HttpPost("join")]
        public IActionResult JoinEvent(int eventId)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return BadRequest("Kullanıcı bilgisi alınamadı.");
            }

            var eventToJoin = _eventService.Get(e => e.Id == eventId && e.IsApproved);
            if (eventToJoin == null)
            {
                return NotFound("Etkinlik bulunamadı veya katılamazsınız.");
            }

            var attendanceCount = _attendanceService.GetAttendanceCountForEvent(eventId);
            if (eventToJoin.Quota <= attendanceCount)
            {
                return BadRequest("Etkinlik kontenjanı dolmuş.");
            }

            var attendance = new Attendance
            {
                UserId = user.Id,
                EventId = eventId
            };

            _attendanceService.Add(attendance);

            return Ok("Etkinliğe katılım işlemi başarılı.");
        }
       
    }
}
