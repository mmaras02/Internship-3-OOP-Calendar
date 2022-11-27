using System;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Text;
using Calendar;

namespace Calendar.Classes
{
    public class Event
    {
        public Guid Id;
        public string EventName { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        private List<string> AttendeeEmail;
        public Event(Guid id,List<string> attendeeEmail)
        {
            Id = id;
            AttendeeEmail = attendeeEmail;
        }
        /*public Event(Guid id,string eventName,string location,DateTime startDate,DateTime endDate)
        {
            Id=id;
            EventName=eventName;
            Location=location;
            StartDate=startDate;
            EndDate=endDate;
        } */
        public Event(){}
        public List<string> GetEmail()
        {
            return AttendeeEmail;
        }
        public void AddPerson(Person newPerson)
        {
            AttendeeEmail.Add(newPerson.Email);
        }
        public void RemoveEmail(string email)
        {
            if(AttendeeEmail.Contains(email))
                AttendeeEmail.Remove(email);
        }
        public bool DoesAnEventExist(List<Event> events)
        {
            foreach (var item in events)
            {
                if (EventName == item.EventName)
                {
                    Console.WriteLine("Event already exists! ");
                    return true;
                }
            }
            return false;
        }

        public bool IsActive()
        {
            if (StartDate < DateTime.Now && EndDate > DateTime.Now)
                return true;
            else return false;
        }
        public bool IsUpcoming()
        {
            if (StartDate > DateTime.Now)
                return true;
            else return false;
        }
        public bool IsOver()
        {
            if (EndDate<DateTime.Now)
                return true;
            else return false;
        } 
        public void PrintActiveEvent()
        {
            double endsIn=Math.Round((EndDate-DateTime.Now).TotalHours,1);
            Console.WriteLine($"Event Id: {Id}\n"+
                              $"Event name: {EventName} - Location: {Location} - Ends in: {endsIn} hours");
        }
        public void PrintUpcomingEvent()
        {
            var startsIn=Math.Round((StartDate-DateTime.Now).TotalDays,1);
            double lasts=Math.Round((EndDate-StartDate).TotalHours,1);
            Console.WriteLine($"\n\nEvent Id: {Id}\n"+
                              $"Event name: {EventName} - Location: {Location} - Starts in: {startsIn} days - Lasts: {lasts} hours");
        }
        public void PrintEndedEvents()
        {
            var endedBefore=Math.Round((DateTime.Now-EndDate).TotalDays,1);
            double lasts=Math.Round((EndDate-StartDate).TotalHours,1);
            Console.WriteLine($"Event name: {EventName} - Location: {Location} - Ended before: {endedBefore} days - Lasted: {lasts} hours");
        }
        

    }
}