using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WABot.Api;
using WABot.Helpers.Json;
using WABot.ModeloTurno;
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
        private static readonly WaApi api = new WaApi("https://eu182.chat-api.com/instance210152/", "w520uh3nu7nbidnc");

        /// <summary>
        /// Handler of post requests received from chat-api
        /// </summary>
        /// <param name="cel">Serialized json object</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Prueba/{cel}")]
        public async Task<string> GenerarTurno(string cel)
        {
            //var urlBase = "https://localhost:5001/api/Personas";
            var http = new HttpClient();
            var per = await http.GetAsync($"https://wabot20201202085345.azurewebsites.net/api/Personas/celular/{cel}");
            //var per = await http.GetFromJsonAsync<Persona>($"{urlBase}/celular/{cel}");
            Persona persona = new Persona();
            int? idPersona = null;
            if (per.IsSuccessStatusCode)
            {
                var cont = await per.Content.ReadAsStringAsync();
                var perso = JsonConvert.DeserializeObject<Persona>(cont);
                if (perso == null)
                {
                    //return Ok("La persona ya existe");
                    persona = new Persona { PerTelefono = cel, Turnos = null };
                    idPersona = null;
                }
                else
                {
                    idPersona = perso.PerId;
                    persona = null;
                }
            }
            else
            {
                persona = new Persona { PerTelefono = cel, Turnos = null };
            }

            var turno = new Turno
            {
                TurnIdPacienteNavigation = persona,
                TurnIdPaciente = idPersona,
                TurnCodigo = Guid.NewGuid().ToString(),
                TurnFecha = DateTime.Now.ToString(),
                TurnEstado = 1,
            };

            var body = JsonConvert.SerializeObject(turno);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await http.PostAsync($"https://wabot20201202085345.azurewebsites.net/api/Turnoes", content);
            //var response = await http.PostAsJsonAsync<Persona> ($"{urlBase}/Personas",persona);
            //var p = await http.PostAsJsonAsync($"{urlBase}/Personas", content);
            if (response.IsSuccessStatusCode)
            {
                var con = await response.Content.ReadAsStringAsync();
                var perso = JsonConvert.DeserializeObject<Turno>(con);
                return ($"Su turno se ha generado correctamente \n\n " +
                    $"Codigo: {perso.TurnId} \n " +
                    $"Fecha y hora: {perso.TurnFecha} \n " +
                    $"Estado: {perso.TurnEstadoNavigation.EstDescripcion}");
            }
            else
            {
                return (response.IsSuccessStatusCode.ToString());
            }

            return "";
        }


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
        [Route("MensajeTurno")]
        public async Task<string> EnviarMensajeTurno(Answer data, string token)
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
                            string mensajeEnviar = mens.Text;
                            //if (mens.Intencion == "SolicitarTurno")
                            //{
                            //    var generarTurno = await GenerarTurno(message.ChatId.Replace("57", "").Replace("@c.us", ""));
                            //    mensajeEnviar = generarTurno;
                            //}

                            if (mens.Intencion == "Confirmar")
                            {
                                var ultimoMensajeHttp = await ObtenerUltimoMensaje(message.ChatId);
                                var mensajeUltimo = await ChatWhatson(ultimoMensajeHttp);
                                var mensUltimo = mensajeUltimo.Where(Option => Option.IsIncoming == true).ToList();
                                if (mensUltimo[mensUltimo.Count - 1].Intencion == "SolicitarTurno")
                                {
                                    var generarTurno = await GenerarTurno(message.ChatId.Replace("57", "").Replace("@c.us", ""));
                                    mensajeEnviar = generarTurno;
                                }
                                else
                                {
                                    mensajeEnviar = "Opcion no valida";
                                }

                            }

                            if (mens.Intencion == "SolicitarAvance")
                            {
                                var avance = await SolicitarAvance();
                                mensajeEnviar = (avance == null) ? "Lo sentimos ubo un error al consultar" : avance;
                            }

                            retornar = await api.SendMessage(message.ChatId, mensajeEnviar);
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
                            //var mensaje = await ChatWhatson(message.Body);
                            //var mens = mensaje.FirstOrDefault(Option => Option.IsIncoming == true);
                            retornar = await api.SendFile(message.ChatId, "");
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
                            retornar = await api.SendFile(message.ChatId, "");
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
            var mensajes = await api.GetRequestGet("messages", chatid);
            var msn = mensajes.Messages.Where(s => s.FromMe == false).ToList();
            msn = msn.OrderByDescending(s => s.MessageNumber).ToList();
            var res = new Answer { InstanceId = mensajes.InstanceId, Messages = msn };
            return res;
        }

        [HttpGet]
        [Route("MensajeAnterior")]
        public async Task<string> GetMensajeAnterior(string chatid)
        {
            var mensaje = await GetMensajes(chatid);
            var ultimo = (mensaje.Messages != null && mensaje.Messages.Count > 0) ? mensaje.Messages[1].Body : null;
            return ultimo;
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
                        //var mensaje = await ChatWhatson("mensaje");
                        var http = new HttpClient();
                        var get = await http.GetAsync($"https://localhost:5001/api/Watson/mensaje");
                        var contenido = await get.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<ObservableCollection<ChatMessage>>(contenido);
                        var mens = res.FirstOrDefault(Option => Option.IsIncoming == true);

                        string cel = $"573184156945@c.us";
                        var MENSAJE = new List<Message> { new Message { Body = mens.Text, ChatId = cel } };
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
        private async Task<List<ChatMessage>> ChatWhatson(string mensaje)
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
        private async Task<string> SolicitarAvance()
        {
            var http = new HttpClient();
            var per = await http.GetAsync($"https://wabot20201202085345.azurewebsites.net/api/Turnoes");
            var turno_ = new List<Turno>();
            string respuesta = "";
            if (per.IsSuccessStatusCode)
            {
                var cont = await per.Content.ReadAsStringAsync();
                var turnoSerializado = JsonConvert.DeserializeObject<List<Turno>>(cont);
                if (turnoSerializado.Count > 0)
                {
                    respuesta = $"Va en el turno: {turnoSerializado[0].TurnId} \n " +
                        $"espero que seas el siguiente \n " +
                        $"Bye";
                    //turnoSerializado = turnoSerializado.OrderByDescending(s => s.TurnId).ToList();
                    //return Ok("La persona ya existe");
                }
            }
            return respuesta;
        }

        private async Task<string> ObtenerUltimoMensaje(string cel)
        {
            //cel = $"57{cel}@c.us";
            var http = new HttpClient();
            var per = await http.GetAsync($"https://wabot20201202085345.azurewebsites.net/MensajeAnterior?chatid={cel}");
            string respuesta = "";
            if (per.IsSuccessStatusCode)
            {
                var cont = await per.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(cont))
                {
                    respuesta = cont;
                }
            }
            return respuesta;
        }

    }
}
