using Business.Abstract;
using Entities.Concrete;
using Entities.DTOs.City;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CityController : ControllerBase
    {
        ICityService _cityService;
        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpPost("addcity")]
        public IActionResult AddCity(CityDto cityDto)
        {
            City city = new City();
            city.Name = cityDto.Name;
            _cityService.Add(city);

            return Ok(city);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateCity(int id, [FromBody] CityDto cityDto)
        {
            var city = _cityService.Get(c => c.Id == id);
            city.Name = cityDto.Name;
            _cityService.Update(city);
            return Ok(city);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteCity(int id)
        {
            try
            {
                var city = _cityService.Get(c => c.Id == id);
                if (city == null)
                {
                    return NotFound();
                }
                _cityService.Delete(city);
                return Ok("Başarıyla silindi.");
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
