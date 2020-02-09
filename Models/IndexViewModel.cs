using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class IndexViewModel
    {
        public User SingleUser {get;set;}

        public Association NewAssos {get;set;}
        public LoginUser SingleLogin {get;set;}

        public List<Wedding> AllWeddings {get;set;}

        public List<Association> AllAss {get;set;}

        public List<Wedding> NotAttending {get;set;}

        public List<Wedding> Attending {get;set;}

        public List<Wedding> Creator {get;set;}

        public int UserId {get;set;}
    }
}