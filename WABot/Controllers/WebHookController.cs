﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WABot.Api;
using WABot.Helpers.Json;
using WABot.Models;
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
        private static readonly WaApi api = new WaApi("https://eu195.chat-api.com/instance200776/", "wuc3wnug10ec4u87");

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
                            retornar = await api.SendFile(message.ChatId, mens.Text);
                            //retornar = await api.SendMessage(message.ChatId, mens.Text);
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

        /// <summary>
        /// Handler of post requests received from chat-api
        /// </summary>
        /// <param name="desde">Serialized json object</param>
        /// <param name="hasta">Serialized json object</param>
        /// <returns></returns>
        [HttpGet("EnvioMasivo/{desde}/{hasta}")]
        public async Task<ActionResult> EviarMensajesAsync(int desde, int hasta)
        {
            try
            {
                var pacientes = new HttpClient();
                var peti = await pacientes.GetAsync($"https://localhost:5001/api/Pacientes/{desde}/{hasta}");
                var con = await peti.Content.ReadAsStringAsync();
                var tel = JsonConvert.DeserializeObject<List<Paciente>>(con).OrderByDescending(Op => Op.PacId).ToList();

                List<int> tiempos = new List<int>();
                tiempos.Add(720000);
                tiempos.Add(600000);
                tiempos.Add(1800000);
                tiempos.Add(3000000);
                tiempos.Add(1320000);
                tiempos.Add(1080000);
                tiempos.Add(300000);
                tiempos.Add(480000);
                tiempos.Add(1980000);
                tiempos.Add(2220000);
                tiempos.Add(3120000);
                tiempos.Add(660000);
                tiempos.Add(2700000);
                tiempos.Add(1140000);
                tiempos.Add(2400000);
                tiempos.Add(1740000);

                var x = 0;
                if (tel != null)
                {
                    Console.WriteLine($"Desde {desde} - Hasta {hasta} Total de evio {tel.Count}");
                    foreach (var item in tel)
                    {
                        x++;
                        var mensaje = await ChatWhatson("mensaje");
                        var mens = mensaje.FirstOrDefault(Option => Option.IsIncoming == true);
                        retornar = await api.SendMessage(message.ChatId, mens.Text);

                        string cel = $"573184156945@c.us";
                        var MENSAJE = new List<Message> { new Message { Body = mens.Text, ChatId = cel } };
                        var asower = new Answer();
                        asower.Messages = MENSAJE;
                        Console.WriteLine($"[{x}] - {cel} Enviando");


                        var respuesta = await EnviarArchivo(asower);
                        Console.WriteLine(respuesta);//item.PacTelefono  {item.PacTelefono})
                        Random r1 = new Random();
                        int indexBtn = r1.Next(tiempos.Count);
                        int esperar = tiempos[indexBtn];
                        await Task.Delay(esperar);
                    }
                }

                return Ok("Tarea Finalizada");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
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
