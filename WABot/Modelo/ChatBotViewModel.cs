using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using WatsonAssistant.Services;
using System.Linq;
using System.Collections.Generic;

namespace WatsonAssistant.Models
{
    public class ChatBotViewModel
    {
        private string _outGoingText;
        public List<ChatMessage> Messages { get; }

        public readonly IBMWatsonAssistant iBMWatsonAssistant;

        public ChatBotViewModel()
        {
            Messages = new List<ChatMessage>();
            OutGoingText = string.Empty;
            iBMWatsonAssistant = new IBMWatsonAssistant();
        }

        ~ChatBotViewModel()
        {
            iBMWatsonAssistant.DeleteSession();
        }

        public string OutGoingText
        {
            get
            {
                return _outGoingText;
            }
            set
            {
                _outGoingText = value;
            }
        }
        //public Task SendCommand => new Task(SendMessage);

        public async Task SendMessage()
        {
            if (!string.IsNullOrEmpty(OutGoingText))
            {
                Messages.Add(new ChatMessage { Text = OutGoingText, IsIncoming = false, MessageDateTime = DateTime.Now });
                string temp = OutGoingText;
                OutGoingText = string.Empty;

                await Task.Run(() =>
                {
                    var res = iBMWatsonAssistant.Message(temp);
                    //Console.WriteLine("Mensaje 1: " + res.Intents);
                    Console.WriteLine("Mensaje 1: " + JsonConvert.SerializeObject(res));
                    OnWatsonMessagerecieved(JsonConvert.SerializeObject(res, Formatting.Indented));
                });
            }
        }


        private void OnWatsonMessagerecieved(string data)
        {
            var message = JsonConvert.DeserializeObject<WatsonMessage>(data);

            if (message.Output.Generic != null)
            {
                foreach (var item in message.Output.Generic)
                {
                    var listItem = new ChatMessage
                    {
                        IsIncoming = true,
                        MessageDateTime = DateTime.Now
                    };
                    if (item.ResponseType.Equals("image"))
                    {
                        listItem.Image = item.Source.ToString();
                    }
                    if (item.ResponseType.Equals("text"))
                    {
                        listItem.Text = item.Text;
                    }
                    if (item.ResponseType.Equals("option"))
                    {
                        listItem.Text += Environment.NewLine + item.Title;
                        //item.Options.First(option => listItem.Text += $"{Environment.NewLine}{option.label}");
                    }
                    if (message.Output.Intents.Length > 0)
                    {
                        listItem.Intencion = message.Output.Intents[0].IntentIntent;
                    }

                    Messages.Add(listItem);
                }
            }
        }
    }
}
