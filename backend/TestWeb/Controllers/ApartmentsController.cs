using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWeb.Data;
using TestWeb.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentsController : ControllerBase
    {
        private readonly ApartmentDb _context;
        public ApartmentsController(ApartmentDb context)
        {
            _context = context;
        }

        // GET: api/<ApartmentsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.AbpApartments.ToListAsync());
        }

        // GET api/<ApartmentsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var a = await _context.AbpApartments.FindAsync(id);
            if(a == null)   
            {
                return NotFound();
            }
            return Ok(a);
        }

        // POST api/<ApartmentsController>
        [HttpPost]
        public async Task<IActionResult> Post(Apartment apartment)
        {
            var Apartment = new Apartment()
            {
                ApartmentId = apartment.ApartmentId,
                OwnerId = apartment.OwnerId,
                OwnerName = apartment.OwnerName,
                AmountOfPeople = apartment.AmountOfPeople,
                AmountOfRooms = apartment.AmountOfRooms,
                BuildingId = apartment.BuildingId,
                Floor = apartment.Floor,
                Area = apartment.Area,
                Price = apartment.Price,
                Status = apartment.Status
            };
            await _context.AbpApartments.AddAsync(Apartment);
            await _context.SaveChangesAsync();

            return Ok(Apartment);
        }

        // PUT api/<ApartmentsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, Apartment apartment)
        {
            var a = await _context.AbpApartments.FindAsync(id);
            if (a != null)
            {
                //a.ApartmentId = apartment.ApartmentId;
                a.OwnerId = apartment.OwnerId;
                a.OwnerName = apartment.OwnerName;
                a.AmountOfPeople = apartment.AmountOfPeople;
                a.AmountOfRooms= apartment.AmountOfRooms;
                a.BuildingId = apartment.BuildingId;
                a.Floor = apartment.Floor;
                a.Area = apartment.Area;
                a.Price = apartment.Price;
                a.Status = apartment.Status;

                await _context.SaveChangesAsync();

                return Ok(a);
            }
            return NotFound();
        }

        // DELETE api/<ApartmentsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var a = await _context.AbpApartments.FindAsync(id);
            if (a != null)
            {
                _context.Remove(a);
                await _context.SaveChangesAsync();

                return Ok(a);
            }
            return NotFound();
        }
    }
}
