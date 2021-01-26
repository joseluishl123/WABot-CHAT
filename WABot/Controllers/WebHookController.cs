using Microsoft.AspNetCore.Mvc;
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
        private static readonly WaApi api = new WaApi("https://eu211.chat-api.com/instance218328/", "3pvpa9ub4demhezh");

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
                //retornar = await api.SendFile(message.ChatId, message.Body);
                retornar = api.MessageDefaul();

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
                            Console.WriteLine(retornar);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retornar = ex.Message;
                Console.WriteLine(retornar);
            }
            return retornar;
        }
        [HttpPost]
        [Route("enviarmensaje2")]
        public async Task<string> EnviarMensaje2(Answer data)
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
                            //var mensaje = await ChatWhatson(message.Body);
                            //var mens = mensaje.FirstOrDefault(Option => Option.IsIncoming == true);
                            //retornar = await api.SendFile(message.ChatId, mens.Text);
                            var msj = api.MessageDefaul();
                            retornar = await api.SendMessage(message.ChatId,msj);
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
                //var pacientes = new HttpClient();
                //var peti = await pacientes.GetAsync($"https://localhost:5001/api/Pacientes/{desde}/{hasta}");
                //var con = await peti.Content.ReadAsStringAsync();
                //var tel = JsonConvert.DeserializeObject<List<Paciente>>(con).OrderByDescending(Op => Op.PacId).ToList();
                var tel = TelNegocios();
                List<int> tiempos = new List<int>();
                tiempos.Add(60000);
                tiempos.Add(70000);
                tiempos.Add(110000);
                tiempos.Add(21000);
                tiempos.Add(50000);
                tiempos.Add(40000);
                tiempos.Add(20000);
                tiempos.Add(38000);
                tiempos.Add(19000);
                tiempos.Add(22000);
                tiempos.Add(21000);
                tiempos.Add(26000);
                tiempos.Add(27000);
                tiempos.Add(50000);
                tiempos.Add(24000);
                tiempos.Add(37000);

                var x = 0;
                if (tel != null)
                {
                    Console.WriteLine($"Desde {desde} - Hasta {hasta} Total de evio {tel.Count}");
                    foreach (var item in tel)
                    {
                        string saludo;
                        x++;
                        try
                        {
                            var http = new HttpClient();
                            var get = await http.GetAsync($"https://localhost:5001/api/Watson/mensaje");
                            var contenido = await get.Content.ReadAsStringAsync();
                            var res = JsonConvert.DeserializeObject<ObservableCollection<ChatMessage>>(contenido);
                            var mens = res.FirstOrDefault(Option => Option.IsIncoming == true);
                            saludo = mens.Text;
                        }
                        catch (Exception)
                        {
                            saludo = "Hola!";
                        }


                        string cel = $"57{item}@c.us";
                        var MENSAJE = new List<Message> { new Message { Body = saludo, ChatId = cel } };
                        var asower = new Answer();
                        asower.Messages = MENSAJE;
                        Console.WriteLine($"[{x}] - {cel} Enviando");
                        //var respuesta = await EnviarArchivo(asower);
                        var respuesta = await EnviarMensaje2(asower);
                        Console.WriteLine(respuesta);//item.PacTelefono  {item.PacTelefono})
                        Random r1 = new Random();
                        int indexBtn = r1.Next(tiempos.Count);
                        int esperar = tiempos[indexBtn];
                        Console.WriteLine(esperar);
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
        private List<string> TelNegocios()
        {
            var tel = new List<string>();
            tel.Add("3104245832");
            tel.Add("3105927852");
            tel.Add("3106275652");
            tel.Add("3108261391");
            tel.Add("3112624380");
            tel.Add("3113296315");
            tel.Add("3116683588");
            tel.Add("3116917368");
            tel.Add("3117262509");
            tel.Add("3117605676");
            tel.Add("3126412332");
            tel.Add("3126463791");
            tel.Add("3128758389");
            tel.Add("3135498438");
            tel.Add("3135701031");
            tel.Add("3136222844");
            tel.Add("3136710620");
            tel.Add("3144532407");
            tel.Add("3145556636");
            tel.Add("3146288790");
            tel.Add("3147078009");
            tel.Add("3147112546");
            tel.Add("3147560090");
            tel.Add("3148841764");
            tel.Add("3174864578");
            tel.Add("3175281302");
            tel.Add("3175831159");
            tel.Add("3184872468");
            tel.Add("3186284587");
            tel.Add("3204832413");
            tel.Add("3206890940");
            tel.Add("3206953528");
            tel.Add("3206991834");
            tel.Add("3207949529");
            tel.Add("3210633505");
            tel.Add("3210635056");
            tel.Add("3215255056");
            tel.Add("3216451023");
            tel.Add("3217092277");
            tel.Add("3217699360");
            tel.Add("3218045312");
            tel.Add("3218384450");
            tel.Add("3218595453");
            tel.Add("3225721787");
            tel.Add("3226222844");
            tel.Add("3226753409");
            tel.Add("3226776099");
            tel.Add("3227058845");
            tel.Add("3227075584");
            tel.Add("3235759993");
            tel.Add("3104299856");
            tel.Add("3147088376");
            tel.Add("3112596920");
            tel.Add("3128685991");
            tel.Add("3045207478");
            tel.Add("3134830071");
            tel.Add("3217648491");
            tel.Add("3104171025");
            tel.Add("3148625011");
            tel.Add("3105388474");
            tel.Add("3206335475");
            tel.Add("3117659881");
            tel.Add("3105030909");
            tel.Add("3136334040");
            tel.Add("3138589345");
            tel.Add("3118747943");
            tel.Add("3218644203");
            tel.Add("3136950853");
            tel.Add("3148367067");
            tel.Add("3117989980");
            tel.Add("3137821206");
            tel.Add("3208971566");
            tel.Add("3207183070");
            tel.Add("3113510130");
            tel.Add("3146374487");
            tel.Add("3128451782");
            tel.Add("3117556149");
            tel.Add("3122421302");
            tel.Add("3207448382");
            tel.Add("3207307762");
            tel.Add("3112283906");
            tel.Add("3138822029");
            tel.Add("3222227661");
            tel.Add("3194121384");
            tel.Add("3142811664");
            tel.Add("3218983011");
            tel.Add("3105217744");
            tel.Add("3137059376");
            tel.Add("3102111077");
            tel.Add("3122774209");
            tel.Add("3217492516");
            tel.Add("3147454396");
            tel.Add("3217873941");
            tel.Add("3136597901");
            tel.Add("3137678706");
            tel.Add("3222714019");
            tel.Add("3228557799");
            tel.Add("3127956640");
            tel.Add("3207077200");
            tel.Add("3122922678");
            tel.Add("3103700980");
            tel.Add("3144084139");
            tel.Add("3208905590");
            tel.Add("3127922394");
            tel.Add("3234707924");
            tel.Add("3218218078");
            tel.Add("3234278151");
            tel.Add("3108227318");
            tel.Add("3212065837");
            tel.Add("3205947349");
            tel.Add("3232915758");
            tel.Add("3208248348");
            return tel;
        }

    }
}
