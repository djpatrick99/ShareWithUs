using System;
namespace ShareWithUs.Models
{
    public class JoinRequest
    {
        public int ServerId
        {
            get;
            set;
        }

        public int UserId
        {
            get;
            set;
        }

        public JoinRequest()
        {
        }
    }
}
