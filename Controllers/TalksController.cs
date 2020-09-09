﻿using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/camps/{moniker}/talks")] //kontroler aswsocjacyjny - taqki bedzie url

    public class TalksController :ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker) // ten moniker bierze sie z route
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker,true);
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,"Failed to get talks");
            }
        }

        [HttpGet("{id:int}")] //id musi byc intem
        public async Task<ActionResult<TalkModel>> Get(string moniker,int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound();
                return _mapper.Map<TalkModel>(talk);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talks");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Camp does not exist");

                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;

                if (model.Speaker == null) return BadRequest("SpeakerId required");
                var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("Speaker could not be found");

                _repository.Add(talk);

                if (await _repository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction(HttpContext,
                        "Get",
                        values: new { moniker, id = talk.TalkId });

                    return Created(url, _mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("Failed to save talks");
                }
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talks");
            }
        }

        [HttpGet("{id:int}")] //id musi byc intem
        public async Task<ActionResult<TalkModel>> Put(string moniker,int id, TalkModel model)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound();

                if(model.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if(speaker != null)
                    {
                        talk.Speaker = speaker;
                    }
                }

                _mapper.Map(model, talk);
                if(await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(talk);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to put talks");

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to put talks");
            }
        }
        [HttpDelete("id:int}")]
        public async Task<IActionResult> Delete(string moniker,int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound();

                _repository.Delete(talk);

                if(await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to delete talk");
                }
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete talks");
            }
        }
    }
}
