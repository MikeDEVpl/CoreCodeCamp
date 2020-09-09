using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps")]   // (tutaj bedzie api/Camps)
    [ApiVersion("2.0")] 
     [ApiController] // powoduje szukanie w body requestu przy post
    public class CampsController2 : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController2(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        //gdy nie ma api version przy akcji to dziala ona dla kazdej wersji
        public async Task<IActionResult> Get(bool includeTalks = false) //Gdy typ zwrotu Action Result sie zgadza zwroci 200 OK
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks);
                var result = new
                {
                    Count = results.Count(),
                    Results = _mapper.Map<CampModel[]>(results)
                };
                return Ok(result);

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }

        [HttpGet("{moniker}")] //Rozszerza sciezke /api[controller]/

        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);
                if (result == null) return NotFound();

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }


        [HttpGet("{moniker}")] //Rozszerza sciezke /api[controller]/

        public async Task<ActionResult<CampModel>> Get11(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker,true);
                if (result == null) return NotFound();

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }
        [HttpGet("search")] // search bedzie czescia adresu a theDate to query string
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!results.Any()) return NotFound();

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }

        public async Task<ActionResult<CampModel[]>> Post(CampModel model)
        {
            try
            {
                var existingCamp = await _repository.GetCampAsync(model.Moniker);
                if (existingCamp != null)
                {
                    return BadRequest("Moniker in use");
                }

                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker }); // generuje link dla dodanego campa

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use parent moniker");
                }

                var camp = _mapper.Map<Camp>(model);
                _repository.Add(camp);
                if (await _repository.SaveChangesAsync())
                {
                    return Created("", _mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
            return BadRequest();

        }
        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(model.Moniker);
                if (oldCamp == null)
                    return NotFound($"Could not find camp with moniker: {moniker}");

                _mapper.Map(model, oldCamp);

                if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

            return BadRequest();
        }

        public async Task<IActionResult> Delete(string moniker) { // Iaction result bo nie zwracamy nic oprocz kodu ok/ko

            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound();

                _repository.Delete(oldCamp);

                if(await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

            return BadRequest("Failed to delete camp");

        } 
    
    }
}
