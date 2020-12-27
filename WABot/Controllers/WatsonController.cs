using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WatsonAssistant.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WABot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatsonController : ControllerBase
    {
        ChatBotViewModel chat;
        public WatsonController()
        {
            if (chat == null)
            {
                chat = new ChatBotViewModel();
                chat.iBMWatsonAssistant.CreateSession();
            }
        }
        // POST api/<WatsonController>
        /// <summary>
        /// Handler of post requests received from chat-api
        /// </summary>
        /// <param name="mensaje">Serialized json object</param>
        /// <returns></returns>
        /// 
        [HttpGet("{mensaje}")]
        public async Task<ActionResult<List<ChatMessage>>> Post(string mensaje)
        {
            try
            {
                chat.OutGoingText = mensaje;
                await chat.SendMessage();
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
            }
            return chat.Messages;
        }

        // PUT api/<WatsonController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WatsonController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
