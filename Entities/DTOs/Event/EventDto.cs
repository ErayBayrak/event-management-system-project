using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Event
{
    public class EventDto
    {
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime LastApplicationEventDate { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int Quota { get; set; }
        public bool IsTicket { get; set; }
        public int? Price { get; set; }
        public bool IsApproved { get; set; }
        public int CityId { get; set; }
        public int CategoryId { get; set; }
        public int? UserId { get; set; }
        public int? CompanyId { get; set; }
    }
}
