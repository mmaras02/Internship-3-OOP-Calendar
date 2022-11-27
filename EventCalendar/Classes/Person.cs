using System;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Text;
using Calendar;

namespace Calendar.Classes
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get;set;}
        public string Email { get;set; }
        private Dictionary<Guid, bool> Attendance { get; set; }
        public Person(string firstName, string lastName, string email, Dictionary<Guid, bool> attendance)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Attendance = attendance;

        }
        public Person(){
            PersonInput();
        }
        public bool IsAttendingEvent(Guid id)
        {
            if(Attendance.ContainsKey(id))
                return Attendance[id];
            else{
                Console.WriteLine($"{Email} not in event attendee list");
                return false;
            }
        }
        public void IsNotAttendingEvent(Guid id)
        {
            if(Attendance.ContainsKey(id))
                Attendance[id] = false;
            //else Console.WriteLine($"{Email} not in event attendee list");
        }
        public void AttendingEvent(Guid id)
        {
            Attendance[id] = false;
        }
        public void RemoveEvent(Guid id)
        {
            if(Attendance.ContainsKey(id))
                Attendance.Remove(id);
        }
        public void AddEvent(Guid id)
        {
            if(!Attendance.ContainsKey(id))
                Attendance.Add(id, true);
        }
        public bool AreEventsOverlaping(DateTime startDate, DateTime endDate, List<Event> events)
        {
            Event myEvent;
            foreach (var item in Attendance.Keys)
            {
                myEvent = events.Find(x => x.Id == item);

                if (startDate > myEvent.StartDate && endDate < myEvent.EndDate || startDate < myEvent.StartDate && endDate > myEvent.EndDate)
                    return true;
            }
            return false;
        }
        public void PersonDetails()
        {
            Console.WriteLine(FirstName + " " + LastName + " => "+ Email);
        }

        public void PersonInput()
        {
            Console.WriteLine("Please enter the first name of the person: ");
            var name = Console.ReadLine();
            while (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Please enter the first name of the person: ");
                name = Console.ReadLine();
            }
            FirstName = name;

            Console.WriteLine("Please enter the last name of the person: ");
            var surname = Console.ReadLine();
            while (string.IsNullOrEmpty(surname))
            {
                Console.WriteLine("Please enter  the last name of the person: ");
                surname = Console.ReadLine();
            }
            LastName = surname;

            Console.WriteLine("Please enter email of the person: ");
            var email = Console.ReadLine();
            while (string.IsNullOrEmpty(email)&& email.Contains("@")&&email.Contains(".com")||email.Contains(".hr"))
            {
                Console.WriteLine("Please enter email of the person: ");
                email = Console.ReadLine();
            }
            Email = email;
        }
    }
}