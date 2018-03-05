using System;
using System.Collections.Generic;

namespace ShareWithUs.Models
{
    public class Message
    {

        public int Id
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public int PosterId
        {
            get;
            set;
        }

        public int ServerId
        {
            get;
            set;
        }

        public int ParentMessageId
        {
            get;
            set;
        }

        public List<Message> childMessages
        {
            get;
            set;
        }

        public Message()
        {
        }
    }
}
