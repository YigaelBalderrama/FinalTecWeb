﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpsonApp.Services;
using SimpsonApp.Models;
using SimpsonApp.Data.Repository;
using SimpsonApp.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace SimpsonApp.Controllers
{
    [Route("api/character/{characterID:int}/[controller]")]
    public class PhraseController : ControllerBase
    {
        private IPhraseService _phraseService;
        public PhraseController(IPhraseService phraseService)
        {
            _phraseService = phraseService;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Phrase>>> getPhrases(int characterID)
        {
            try
            {
                return Ok(await _phraseService.getPhrases(characterID));
            }
            catch (NotFoundOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Something happend: {ex.Message}");
            }

        }

        [HttpGet("{PhraseId:int}", Name = "GetPhrase")]
        public async Task<ActionResult<Phrase>> GetPhraseAsync(int characterID, int PhraseId)
        {
            try
            {
                return Ok(await _phraseService.GetphraseAsync(characterID, PhraseId));
            }
            catch (NotFoundOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Something happend: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Phrase>> CreatePhraseAsync(int characterID, [FromBody] Phrase phrase)
        {
            if (validateModelFields().Count() == 0)
            {
                try
                {
                    var phraseCreated = await _phraseService.CreatePhraseAsync(characterID, phrase);
                    return CreatedAtRoute("GetPhrase", new { characterID = characterID, PhraseId = phraseCreated.ID }, phraseCreated);
                }
                catch (NotFoundOperationException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Something happend: {ex.Message}");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [Authorize]
        [HttpPut("{phraseID:int}")]
        public async Task<ActionResult<Phrase>> UpdatePhraseAsync(int characterID, int phraseID, [FromBody] Phrase Frase)
        {
            if (validateModelFields(true).Count() == 0)
            {
                try
                {
                    return Ok(await _phraseService.UpdatePhraseAsync(characterID, phraseID, Frase));
                }
                catch (NotFoundOperationException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Something happend: {ex.Message}");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [Authorize]
        [HttpDelete("{phraseID:int}")]
        
        public async Task<ActionResult<bool>> DeletePhraseAsync(int characterID, int phraseID)
        {
            try
            {
                return Ok(await _phraseService.DeletePhraseAsync(characterID, phraseID));
            }
            catch (NotFoundOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Something happend: {ex.Message}");
            }
        }
        //api/character/2/phrase/like
        [HttpPut("like")]
        public async Task<ActionResult<Phrase>> LikePhraseAsync(int characterID, [FromBody] List<int> listaPhrasesId)
        {
            try
            {
                return Ok(await _phraseService.addLikes(characterID, listaPhrasesId));
            }
            catch (NotFoundOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Something happend: {ex.Message}");
            }
        }

        public List<string> validateModelFields( bool updateMode = false)
        {
            var ret = new List<string>();
            if (!ModelState.IsValid)
            {
                foreach (var par in ModelState)
                {
                    if (par.Value.Errors.Count != 0)
                    {
                        string clar = "";
                        foreach (var err in par.Value.Errors)
                        {
                            if (!(err.ErrorMessage == "Required" && updateMode))
                            {
                                clar += err.ErrorMessage + ", ";
                            }
                        }
                        if (clar != "")
                            ret.Add($"{par.Key} -> ({clar})");
                    }
                }
            }
            return ret;
        }
    }
    
}
