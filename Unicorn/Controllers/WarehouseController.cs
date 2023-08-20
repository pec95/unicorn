using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unicorn.Entities;
using Unicorn.Models;

namespace Unicorn.Controllers
{
    //[Authorize(Roles = "Warehouse")]
    [EnableCors("AllowWarehouse")]
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly DataContext _context;
        private Error _errorObject;

        public WarehouseController(DataContext context)
        {
            this._context = context;
        }

        [HttpGet("getCars")]
        public List<CarWeb> GetExsitingCars()
        {
            var cars = getCars();

            return cars;
        }

        [HttpGet("allParts")]
        public List<PartWeb> GetParts()
        {
            var parts = _context.Parts.ToList();

            var partsAll = getPartToWeb(parts);
            
            return partsAll;

        }

        [HttpPost("addPart")]
        public  async Task<IActionResult> AddPart([FromBody] PartAdd newPartValue)
        {
            if (ModelState.IsValid)
            {
                var partFound = _context.Parts.FirstOrDefault(p => p.SerialNumber == newPartValue.SerialNumber);
                if(partFound != null)
                {
                    return BadRequest(false);
                }

                var partDate = DateTime.Parse(newPartValue.DateManufactured);
                Part newPart = new Part() { SerialNumber = newPartValue.SerialNumber, ManufacterDate = partDate };

                var car = _context.Cars.FirstOrDefault(c => c.Id == newPartValue.CarId);
                
                if(car == null)
                {
                    return BadRequest(false);
                }

                _context.Parts.Add(newPart);
                _context.SaveChanges();

                var partDb = _context.Parts.Include(p => p.Cars).First(p => p.SerialNumber == newPart.SerialNumber);
    
                partDb.Cars.Add(car);

                bool updated = await TryUpdateModelAsync(partDb);

                if (updated)
                {
                    _context.SaveChanges();
                    var partWeb = new PartWeb() { Id = partDb.Id, DateManufactured = partDate.ToShortDateString(), SerialNumber = newPartValue.SerialNumber };

                    return Ok(partWeb);
                }
                else 
                {
                    return BadRequest(false);
                }
            }
            else
            {
                return BadRequest(false);
            }

        }

        [HttpDelete("deletePart/{id}")]
        public IActionResult DeletePart(int id)
        {
            var allParts = _context.Parts.ToList();

            var part = allParts.FirstOrDefault(p => p.Id == id);
            
            if (part == null)
            {
                return NotFound(false);
            }
            else
            {
                var articleToRemove = _context.Articles.FirstOrDefault(a => a.PartId == id);

                if(articleToRemove != null)
                {
                    _context.Articles.Remove(articleToRemove);
                    _context.SaveChanges();
                }

                _context.Parts.Remove(part);
                
                _context.SaveChanges();

                return Ok(id);
            }
        }

        [HttpGet("searchSerial/{serial}")]
        public bool SearchSerial(int serial)
        {
            var part = _context.Parts.FirstOrDefault(p => p.SerialNumber == serial);
            if (part == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [HttpGet("searchDate/{dateString}")]
        public List<PartWeb> SearchDate(string dateString)
        {
            var date = DateTime.Parse(dateString);
            var parts = _context.Parts.Where(p => p.ManufacterDate.Equals(date)).ToList();

            var partsWeb = getPartToWeb(parts);

            return partsWeb;
        }

        [HttpGet("searchCar/{brandCar}")]
        public List<PartWeb> SearchCar(string brandCar)
        {
            var partsEmpty = new List<PartWeb>();
            int counter = 0;
            int j = 0;
            brandCar = brandCar.Trim().ToLower();
            for (int i = 0; i < brandCar.Length; i++)
            {
                if (brandCar[i].Equals(' ') && j == 0)
                {
                    j = i;
                    counter++;
                }
                else if (brandCar[i].Equals(' '))
                {
                    counter++;
                }
            }

            if (counter != 1)
            {
                return partsEmpty;
            }

            string brandName = brandCar.Substring(0, j + 1);
            string carName = brandCar.Substring(j);
            brandName = brandName.Trim();
            carName = carName.Trim();

            var brand = _context.Brands.Include(b => b.Cars).FirstOrDefault(p => p.Name.ToLower().Equals(brandName));
            if(brand != null)
            {
                var cars = brand.Cars.ToList();
                var car = cars.FirstOrDefault(p => p.Name.ToLower().Equals(carName));

                if (car != null)
                {
                    var carDb = _context.Cars.Include(c => c.Parts).First(c => c.Id == car.Id);
                    var parts = carDb.Parts.ToList();
                    var partsWeb = getPartToWeb(parts);

                    return partsWeb;
                }
            }

            return partsEmpty;

        }

        [HttpPost("createBrands")]
        public void CreateBrands()
        {
            var brand1 = new Brand() { Name = "Audi" };
            var brand2 = new Brand() { Name = "Renault" };
            var brand3 = new Brand() { Name = "Ford" };
            var brand4 = new Brand() { Name = "Dacia" };

            _context.Brands.Add(brand1);
            _context.Brands.Add(brand2);
            _context.Brands.Add(brand3);
            _context.Brands.Add(brand4);
            _context.SaveChanges();
        }

        [HttpPost("createCars")]
        public void CreateCars()
        {
            var car11 = new Car() { Name = "X-Trail", BrandId = 1 };
            var car12 = new Car() { Name = "Qashai", BrandId = 1 };

            var car21 = new Car() { Name = "Q7", BrandId = 2 };
            var car22 = new Car() { Name = "A8", BrandId = 2 };

            var car31 = new Car() { Name = "Clio", BrandId = 3 };
            var car32 = new Car() { Name = "Kadjar", BrandId = 3 };

            var car41 = new Car() { Name = "Focus", BrandId = 4 };
            var car42 = new Car() { Name = "Kuga", BrandId = 4 };

            var car51 = new Car() { Name = "Duster", BrandId = 5 };
            var car52 = new Car() { Name = "Sandero", BrandId = 5 };

            _context.Cars.Add(car11);
            _context.Cars.Add(car12);
            _context.Cars.Add(car21);
            _context.Cars.Add(car22);
            _context.Cars.Add(car31);
            _context.Cars.Add(car32);
            _context.Cars.Add(car41);
            _context.Cars.Add(car42);
            _context.Cars.Add(car51);
            _context.Cars.Add(car52);

            _context.SaveChanges();
        }


        [HttpGet("numberOfParts")]
        public List<CarParts> NumberOfParts()
        {
            var carPartsAll = new List<CarParts>();

            var carsWeb = getCars();
            var cars = _context.Cars.Include(c => c.Parts).ToList();

           foreach(var car in cars)
           {
               foreach(var carWeb in carsWeb)
               {
                   if (carWeb.CarId == car.Id)
                   {
                       var carPart = new CarParts()
                       {
                           BrandCar = carWeb.CarName,
                           PartsNumber = car.Parts.Count
                       };

                       carPartsAll.Add(carPart);

                       break;
                   }
               }
           }

            
            return carPartsAll;
        }

        private List<CarWeb> getCars()
        {
            List<CarWeb> carsWeb = new List<CarWeb>();
            var brands = _context.Brands.Include(b => b.Cars).ToList();

            foreach (var brand in brands)
            {
                var cars = brand.Cars.ToList();
                foreach (var car in cars)
                {
                    CarWeb carWeb = new CarWeb()
                    {
                        CarId = car.Id,
                        CarName = brand.Name + " " + car.Name
                    };

                    carsWeb.Add(carWeb);
                }
            }

            return carsWeb;
        }

        private List<PartWeb> getPartToWeb(List<Part> parts) 
        {
            var partsWeb = new List<PartWeb>();

            foreach (var part in parts)
            {
                var partWeb = new PartWeb()
                {
                    Id = part.Id,
                    SerialNumber = part.SerialNumber,
                    DateManufactured = part.ManufacterDate.ToShortDateString()
                };

                partsWeb.Add(partWeb);
            }

            return partsWeb;
        }
    }
}
