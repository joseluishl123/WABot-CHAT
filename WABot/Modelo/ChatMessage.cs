﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WatsonAssistant.Models
{
    public class ChatMessage
    {

        public string Text { get; set; }
        public string Intencion { get; set; }
        public DateTime MessageDateTime { get; set; }
        public bool IsIncoming { get; set; }
        public string Image { get; set; }
    }
}
