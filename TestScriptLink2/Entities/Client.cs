using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.Entities
{
    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string GenderCode { get; set; }
        public string GenderValue { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string RaceCode { get; set; }
        public string RaceValue { get; set; }

        public EpisodeInformation EpisodeInformation { get; set; }

        public string Age 
        { 
            get {
                DateTime today = DateTime.Today;
                int age = today.Year - this.DateOfBirth.Year;
                if (this.DateOfBirth > today.AddYears(-age)) 
                    age--;
                return age.ToString();
            }
        }

        public Client()
        {
            this.EpisodeInformation = new EpisodeInformation();
        }
    }
}