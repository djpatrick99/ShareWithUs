using System;
namespace ShareWithUs.Models
{
    public class Member
    {
        public User User
        {
            get;
            set;
        }

        public int Role
        {
            get;
            set;
        }

        public Member()
        {
        }
    }
}