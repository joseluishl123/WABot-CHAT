using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WABot.Helpers;
using WABot.Helpers.Json;


namespace WABot.Api
{
    /// <summary>
    /// Class representing the implementation of the API of the site chat-api.com
    /// </summary>
    public class WaApi
    {
        /// <summary>
        /// Link. You can get it in your personal office.
        /// </summary>
        private string APIUrl = "https://eu195.chat-api.com/instance182066/";
        /// <summary>
        /// Token. You can get it in your personal office.
        /// </summary>
        private string token = "kr5thavzs4zv0awc";

        /// <summary>
        /// The designer accepts token and link as parameters
        /// </summary>
        /// <param name="aPIUrl">Url</param>
        /// <param name="token">Token</param>
        public WaApi(string aPIUrl, string token)
        {
            APIUrl = aPIUrl;
            this.token = token;
        }

        /// <summary>
        /// The method makes a request to the server chat-api.com.
        /// </summary>
        /// <param name="method">API method as per documentation.</param>
        /// <param name="data">Json data</param>
        /// <returns></returns>
        public async Task<string> SendRequest(string method, string data)
        {
            string url = $"{APIUrl}{method}?token={token}";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("", content);
                return await result.Content.ReadAsStringAsync();
            }
        }

        public async Task<Answer> GetRequest(string method, string chatid)
        {
            string url = $"{APIUrl}{method}?token={token}";
            if (!string.IsNullOrEmpty(chatid))
                url = $"{APIUrl}{method}?token={token}&chatId={chatid}";

            var messages = new Answer();
            using (var client = new HttpClient())
            {

                var result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode)
                {
                    var respuesta = await result.Content.ReadAsStringAsync();
                    messages = JsonConvert.DeserializeObject<Answer>(respuesta);
                }

                return messages;
            }
        }
         public async Task<Answer> GetRequestGet(string method, string chatid)
        {
            DateTime foo2 = DateTime.UtcNow.Date.Date;
            long unixTime2 = ((DateTimeOffset)foo2).ToUnixTimeSeconds();

            string url = $"{APIUrl}{method}?token={token}&limit=100000&min_time={unixTime2}";
            if (!string.IsNullOrEmpty(chatid))
                url = $"{APIUrl}{method}?token={token}&chatId={chatid}&limit=500";

            var messages = new Answer();
            using (var client = new HttpClient())
            {

                var result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode)
                {
                    var respuesta = await result.Content.ReadAsStringAsync();
                    messages = JsonConvert.DeserializeObject<Answer>(respuesta);
                }

                return messages;
            }
        }
        public async Task<Answer> GetRequestmessagesHistory(string method)
        {
        //https://api.chat-api.com/instance180795/messagesHistory?token=nvpohfhzexbp53sl&page=1&count=20
            string url = $"{APIUrl}{method}?token={token}";
                url = $"{APIUrl}{method}?token={token}&count=20";

            var messages = new Answer();
            using (var client = new HttpClient())
            {

                var result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode)
                {
                    var respuesta = await result.Content.ReadAsStringAsync();
                    messages = JsonConvert.DeserializeObject<Answer>(respuesta);
                }

                return messages;
            }
        }

        /// <summary>
        /// Sends a message to this ID
        /// </summary>
        /// <param name="chatID">chat ID</param>
        /// <param name="text">Message Text</param>
        /// <returns></returns>
        public async Task<string> SendMessage(string chatID, string text)
        {
            var data = new Dictionary<string, string>()
            {
                {"chatId",chatID },
                { "body", text }
            };
            return await SendRequest("sendMessage", JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Sends a voice message
        /// </summary>
        /// <param name="chatID">chat ID</param>
        /// <returns></returns>
        public async Task<string> SendOgg(string chatID)
        {
            string ogg = "https://firebasestorage.googleapis.com/v0/b/chat-api-com.appspot.com/o/audio_2019-02-02_00-50-42.ogg?alt=media&token=a563a0f7-116b-4606-9d7d-172426ede6d1";
            var data = new Dictionary<string, string>
            {
                {"audio", ogg },
                {"chatId", chatID }
            };

            return await SendRequest("sendAudio", JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Sends out geolocation
        /// </summary>
        /// <param name="chatID">chat ID</param>
        /// <returns></returns>
        public async Task<string> SendGeo(string chatID,string lat, string lng)
        {
            var data = new Dictionary<string, string>()
            {
                { "lat", lat },
                { "lng", lng },
                { "address", "Ubicación" },
                { "chatId", chatID} 
            };
            return await SendRequest("sendLocation", JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Creates a group between user and bot.
        /// </summary>
        /// <param name="author">The author parameter from the obtained JSON body.</param>
        /// <returns></returns>
        public async Task<string> CreateGroup(string author)
        {
            var phone = author.Replace("@c.us", "");
            var data = new Dictionary<string, string>()
            {
                { "groupName", "Group C#"},
                { "phones", phone },
                { "messageText", "This is your group." }
            };
            return await SendRequest("group", JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Sends files to the user.
        /// </summary>
        /// <param name="chatID">chat ID</param>
        /// <param name="base64">The format of the desired file.</param>
        /// <returns></returns>
        public async Task<string> SendFile(string chatID, string base64)
        {
            string mensajeTexto = "Hoy 14 de Noviembre, la solidaridad no es un acto de caridad, sino una ayuda mutua entre fuerzas que luchan por el mismo objetivo. \n\n" +
      "Comparto este mensaje de manera masiva para invitarnos a apoyarnos. \n\n";


            var data = new Dictionary<string, string>(){
                    { "chatId", chatID },
                    { "body", base64 },
                    { "filename", "img_WhatsApp"},
                    { "caption", mensajeTexto}
                };

            return await SendRequest("sendFile", JsonConvert.SerializeObject(data));

            //var availableFormat = new Dictionary<string, string>()
            //{
            //    {"doc", Base64String.Doc },
            //    {"gif",Base64String.Gif },

            //    { "jpg",Base64String.Jpg },
            //    { "png", Base64String.Png },
            //    { "pdf", Base64String.Pdf },
            //    { "mp4",Base64String.Mp4 },
            //    { "mp3", Base64String.Mp3}
            //};

            //if (availableFormat.ContainsKey(format))
            //{
            //    var data = new Dictionary<string, string>(){
            //        { "chatId", chatID },
            //        { "body", availableFormat[format] },
            //        { "filename", "yourfile" },
            //        { "caption", $"Your file" }
            //    };

            //    return await SendRequest("sendFile", JsonConvert.SerializeObject(data));
            //}
            //return await SendMessage(chatID, "No file with this format");

        }
    }
}
