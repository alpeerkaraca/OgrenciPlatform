using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public class AiRequestDto
    {
        public List<Message> messages { get; set; }
        public string model { get; set; }
        public bool stream { get; set; }
    }

    public class AiResponseDto
    {
        public List<Choice> choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}