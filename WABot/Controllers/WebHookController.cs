using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WABot.Api;
using WABot.Helpers.Json;
using WatsonAssistant.Models;

namespace WABot.Controllers
{
    /// <summary>
    /// Controller for processing requests coming from chat-api.com
    /// </summary>
    [ApiController]
    [Route("/")]
    public class WebHookController : ControllerBase
    {
        ChatBotViewModel chat;
        public WebHookController()
        {
            if (chat == null)
            {
                chat = new ChatBotViewModel();
                chat.iBMWatsonAssistant.CreateSession();
            }
        }
        /// <summary>
        /// A static object that represents the API for a given controller.
        /// </summary>
        /// 
        private static readonly WaApi api = new WaApi("https://eu113.chat-api.com/instance196144/", "77zr920hrscx8s14");

        /// <summary>
        /// Handler of post requests received from chat-api
        /// </summary>
        /// <param name="data">Serialized json object</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Post(Answer data)
        {
            foreach (var message in data.Messages)
            {
                if (message.FromMe)
                    continue;

                switch (message.Body.Split()[0].ToLower())
                {
                    case "chatid":
                        return await api.SendMessage(message.ChatId, $"Your ID: {message.ChatId}");
                    case "file":
                        var texts = message.Body.Split();
                        if (texts.Length > 1)
                            return await api.SendFile(message.ChatId, texts[1]);
                        break;
                    case "ogg":
                        return await api.SendOgg(message.ChatId);
                    case "geo":
                        return await api.SendGeo(message.ChatId, "", "");
                    case "group":
                        return await api.CreateGroup(message.Author);
                    default:
                        return await api.SendMessage(message.ChatId, message.Body);
                }
            }
            return "";
        }

        /// <summary>
        /// Handler of post requests received from chat-api
        /// </summary>
        ///// <param name="descripcion">Serialized json object</param>
        /// <returns></returns>
        [HttpPost("enviararchivo")]
        //[Route]
        public async Task<string> EnviarArchivo(Answer data)
        {
            string retornar = "";
            foreach (var message in data.Messages)
            {
                retornar = await api.SendFile(message.ChatId, message.Body);
            }
            return retornar;
        }

        ///// <summary>
        ///// Handler of post requests received from chat-api
        ///// </summary>
        ///// <param name="token">Serialized json object</param>
        ///// <returns></returns>
        [HttpPost]
        [Route("enviarmensaje")]
        public async Task<string> EnviarMensaje(Answer data, string token)
        {
            string retornar = "";
            try
            {
                if (data != null)
                {
                    foreach (var message in data.Messages)
                    {
                        if (!message.FromMe)
                        {
                            message.FromMe = false;
                            message.Id = null;
                            message.MessageNumber = null;
                            var mensaje = await ChatWhatson(message.Body);
                            var mens = mensaje.FirstOrDefault(Option => Option.IsIncoming == true);
                            retornar = await api.SendMessage(message.ChatId, mens.Text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retornar = ex.Message;
            }
            return retornar;
        }

        //[HttpPost]
        //[Route("enviarmensaje2")]
        //public async Task<string> EnviarMensaje2()
        //{
        //    Answer data = new Answer();
        //    var Messages = new Message { Body = "WebHookkkk", ChatId = "573184156945@c.us" };
        //    var messList = new List<Message>();
        //    messList.Add(Messages);
        //    data.Messages = messList;
        //    string retornar = "";
        //    foreach (var message in data.Messages)
        //    {
        //        retornar = await api.SendMessage(message.ChatId, message.Body);
        //    }
        //    return retornar;
        //}

        //[HttpGet]
        //[Route("enviarmensaje3")]
        //public async Task<string> EnviarMensaje3()
        //{
        //    Answer data = new Answer();
        //    var Messages = new Message { Body = "WebHookkkk", ChatId = "573184156945@c.us" };
        //    var messList = new List<Message>();
        //    messList.Add(Messages);
        //    data.Messages = messList;
        //    string retornar = "";
        //    foreach (var message in data.Messages)
        //    {
        //        retornar = await api.SendMessage(message.ChatId, message.Body);
        //    }
        //    return "OK";
        //}

        [HttpPost]
        [Route("enviarubicacion")]
        public async Task<string> Enviarubicacion(Answer data)
        {
            string retornar = "";
            foreach (var message in data.Messages)
            {
                retornar = await api.SendGeo(message.ChatId, message.Author, message.Id);
            }
            return retornar;
        }

        [HttpGet]
        public async Task<Answer> GetMensajes(string chatid)
        {
            return await api.GetRequestGet("messages", chatid);
        }

        [HttpGet]
        [Route("messagesHistory")]
        public async Task<Answer> GetMensajesmessagesHistory()
        {
            return await api.GetRequestmessagesHistory("messagesHistory");
        }

        //[HttpGet("{mensaje}")]
        private async Task<ObservableCollection<ChatMessage>> ChatWhatson(string mensaje)
        {
            try
            {
                chat.OutGoingText = mensaje;
                await chat.SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return chat.Messages;
        }


    }
}
