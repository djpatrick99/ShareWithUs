using System;
using System.Collections.Generic;

namespace ShareWithUs.Models
{
    public class Server
    {
        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public List<User> members
        {
            get;
            set;
        }

        public Server()
        {
            
        }
    }
}
