using System;
namespace EventApp.Model
{
    public class CardModel
    {
        public int CardId { get; set; }

        public string EventType { get; set; }

        public string EventDate { get; set; }

        public string ClientName { get; set; }

        public int Persons { get; set; }

        public string Location { get; set; }
    }
}
