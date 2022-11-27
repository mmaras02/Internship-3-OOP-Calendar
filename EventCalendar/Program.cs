using System;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Text;
using Calendar;
using Calendar.Classes;


namespace Calendar
{
    public class Program
    {
        static void Main(string[] args)
        {
            var events=AddEvent();
            var people=AddPeople(events);

            var choice = 0;
            do
            {
                Options();
                var choiceSuccess = int.TryParse(Console.ReadLine(), out choice);

                if (choiceSuccess)
                {
                    switch (choice)
                    {
                        case 1:
                            Console.Clear();
                            Console.WriteLine("Here is a list of currently active events:\n");
                            ActiveEvents(events,people);
                            break;
                        case 2:
                            Console.Clear();
                            Console.WriteLine("Here is a list of your upcoming events:\n ");
                            UpcomingEvents(events,people);
                            break;
                        case 3:
                            Console.Clear();
                            Console.WriteLine("Here is a list of already ended events:");
                            EndedEvents(events,people);
                            break;
                        case 4:
                            Console.Clear();
                            Console.WriteLine("You chose to create new event!\n ");
                            CreateEvent(events,people);
                            break;
                        case 5:
                            Console.WriteLine("Exit from menu!");
                            break;
                        default:
                            Console.WriteLine("Wrong input! Please try again!");
                            break;
                    }
                }
                else
                    Console.WriteLine("Please enter number from 1-5!");
            } while (choice != 5);

        }
        static void Options()
        {
            Console.WriteLine("\n\n================Main Menu================\n");
            Console.WriteLine("Please,choose the action :\n"+
                    "1.Active events\n"+
                    "2.Upcoming events\n" +
                    "3.Already ended events\n" +
                    "4.Create new event\n" +
                    "5.Exit the app\n");
        }
        static bool UserInput()
        {
            Console.WriteLine("If you want to confirm all changes time 'yes' ");
            var answer = Console.ReadLine();
            if (answer.ToLower() == "yes")
                return true;
            else
                return false;
        }
        static void PrintAttendees(Event ev,List<Person>people)
        {
            foreach(var person in people)
            {
                if(ev.GetEmail().Contains(person.Email))
                {
                    person.PersonDetails();
                }
            }
        }
        static bool CheckIfEmailIsValid(string email)
        {
            if (email.Contains("@") && email.Contains(".com")||email.Contains(".hr"))
                return true;
            return false;
        }
        static List<string>CheckEmail(Event theEvent)
    {
        var input=Console.ReadLine().Trim();
        if(input==""){
            Console.WriteLine("Wrong input! Please try again: ");
           input=Console.ReadLine().Trim();
        }
        var email=input.Split(",");

        var foundEmails=new List<string>(){};
        foreach(var item in email)
        {
            if(theEvent.GetEmail().Contains(item))
                foundEmails.Add(item);
        }
        return foundEmails;
    }
    static Event CheckInput(List<Event>events)
    {
        Event myEvent;
        var eventId=Console.ReadLine().ToString();

        myEvent=events.Find(x=>x.Id.ToString()==eventId);

        if(myEvent==null)
        {
            Console.WriteLine("\nThat event doesn't exist! Please try again: ");
            eventId=Console.ReadLine().Trim();
        }

        return myEvent;
    }
        static void ActiveEvents(List<Event>events,List<Person>people)
        {
            foreach(var item in events)
            {
                if(item.IsActive())
                {
                    item.PrintActiveEvent();
                    Console.WriteLine("List of attendees:");
                    PrintAttendees(item,people);
                    //Console.WriteLine($"{string.Join(", ",item.GetEmail())}");
                }
            }
            var option=-1;
            Console.WriteLine("\n\n-------SUBMENU-------");
            Console.WriteLine("1.record absent people\n0.Go back to main menu\n");
            int.TryParse(Console.ReadLine(), out option);
            if(option==1)
            {
                RecordAbsentPeople(events,people);
            }
            else if(option==0){
                Console.Clear();
                return;
            }
            else Console.WriteLine("Wrong input! Please try again!");
    }
    static void RecordAbsentPeople(List<Event>events,List<Person>people)
    {
        Console.WriteLine("Enter the id of the event: ");
        var eventToChange=CheckInput(events);

        if(!eventToChange.IsActive()){
            Console.WriteLine("Chosen event is not currently active! ");
            eventToChange=CheckInput(events);
        }
        Console.WriteLine("Enter emails of the people you want to record as absent: ");
        var emails=CheckEmail(eventToChange);

        if(UserInput())
        {
            foreach(var item in emails)
            {
                people.Find(x => x.Email == item).IsNotAttendingEvent(eventToChange.Id);
            }
            Console.WriteLine("\nYou have successfully marked absence\n");
        }
        else{
            Console.WriteLine("\nAction has been stopped!\n");
            return;
        }
    }
    static void UpcomingEvents(List<Event>events,List<Person>people)
    {
        foreach(var item in events)
        {
            if(item.IsUpcoming())
            {
                item.PrintUpcomingEvent();
                Console.WriteLine("List of attendees:");
                PrintAttendees(item,people);
                //Console.WriteLine($"{string.Join(", ",item.GetEmail())}");
            }
        }
        var option=-1;
        Console.WriteLine("\n\n-------SUBMENU-------");
        Console.WriteLine("1.Delete event\n2.Remove people from event\n0.Go back to main menu");
        int.TryParse(Console.ReadLine(), out option);
        switch(option)
        {
            case 1:
                //Console.Clear();
                DeleteEvent(events,people);
                break;
            case 2:
                //Console.Clear();
                RemovePeople(events,people);
                break;
            case 0:
                Console.Clear();
                break;
        }

    }
    static void DeleteEvent(List<Event>events,List<Person>people)
    {
        Console.WriteLine("Enter id of the event you wish to delete: ");
        var eventToDelete=CheckInput(events);
        if(!eventToDelete.IsUpcoming())
        {
            Console.WriteLine("Choosen event has already ended or is currently active! Please try again: ");
            eventToDelete=CheckInput(events);
        }

        Console.WriteLine($"\nAre you certain you want to delete event:\n{eventToDelete.EventName} - {eventToDelete.Location} - Starts in:{Math.Round((eventToDelete.StartDate-DateTime.Now).TotalDays,1)} days");
        if(UserInput())
        {
            events.Remove(eventToDelete);
            foreach(var person in people)
            {
                person.RemoveEvent(eventToDelete.Id);
            }
            Console.WriteLine("You have successfully deleted an event!");
        }
        else{
            Console.WriteLine("\nAction has been stopped!\n");
            return;
        }

    }
    static void RemovePeople(List<Event>events,List<Person>people)
    {
        Console.WriteLine("Please enter id of the event you wish to remove people from: ");
        var chosenEvent=CheckInput(events);
        if(!chosenEvent.IsUpcoming())
        {
            Console.WriteLine("Choosen event has already ended or is currentl active! Please try again: ");
            chosenEvent=CheckInput(events);
        }
        Console.WriteLine("Enter emails of the people you want to record as absent: ");
        var emails=CheckEmail(chosenEvent);
        
        var emailList = new List<string>();

        if(emails.Count!=0)
        {
            if(UserInput())
            {
                foreach(var email in emails)
                {
                    chosenEvent.RemoveEmail(email);
                    emailList.Add(email);
                }
                foreach (var item in emailList){
                    people.Find(x => x.Email == item).RemoveEvent(chosenEvent.Id);
                }
                Console.WriteLine("You have successfully removed wanted people!");
            }
            else{
                Console.WriteLine("\nAction has been stopped!\n");
                return;
            }
        }
        else {
            Console.WriteLine("You didn't enter correct person!");
            return;
        }
    }
    static void EndedEvents(List<Event>events,List<Person>people)
    {
        var peopleThatHaveAtended = new List<Person>();
        var peopleThatDidNotAtend = new List<Person>();
        Person myPerson;

        foreach(var item in events)
            {
                if(item.IsOver())
                {
                    foreach(var email in item.GetEmail())
                    {
                        myPerson=people.Find(x=>x.Email==email);

                        if(myPerson.IsAttendingEvent(item.Id))
                            peopleThatHaveAtended.Add(myPerson);
                        else
                            peopleThatDidNotAtend.Add(myPerson);
                    }
            
                    item.PrintEndedEvents();
                    if(peopleThatHaveAtended.Count==0&&peopleThatDidNotAtend.Count!=0){
                        //Console.WriteLine($"\nList of people that didn't attend event:\n{string.Join(", ",peopleThatDidNotAtend)}");
                        Console.WriteLine($"\nList of people that didn't attend event:");
                        PrintAttendees(item,peopleThatHaveAtended);
                    }

                    else if(peopleThatDidNotAtend.Count==0&&peopleThatHaveAtended.Count!=0){
                        //Console.WriteLine($"\nList of attendees:\n{string.Join(", ",peopleThatHaveAtended)}");
                        Console.WriteLine($"\nList of attendees:");
                        PrintAttendees(item,peopleThatDidNotAtend);
                    }

                    else{
                        Console.WriteLine($"\nList of people that attended event:");
                        PrintAttendees(item,peopleThatHaveAtended);
                        Console.WriteLine($"\nList of people that did not attend event:");
                        PrintAttendees(item,peopleThatDidNotAtend);
                    }
                }
            }
    }
    static string GetName()
    {
        var newName = Console.ReadLine();
        while (string.IsNullOrEmpty(newName))
        {
            Console.WriteLine("Entry not valid! Please try again: ");
            newName = Console.ReadLine();
        }
        return newName;
    }
    static DateTime GetDate()
    {
        var dateSuccess = DateTime.TryParse(Console.ReadLine(), out DateTime newDate);
        if (!dateSuccess)
        {
            Console.WriteLine("Entry not valid! Please try again!");
            dateSuccess = DateTime.TryParse(Console.ReadLine(), out newDate);
        }
        if(newDate<=DateTime.Now){
            Console.WriteLine("The event can't end/start at this moment!");
        }
        return newDate;
    }
    
    static void CreateEvent(List<Event>events,List<Person>people)
    {
        var newEvent = new Event();
        var alreadyAttendingEvent=new List<string>(){};
        var comingToEvent=new List<string>(){};
        var notInList=new List<string>(){};
        var InList=new List<string>(){};

        Console.WriteLine("Please enter name of the event ");
        var newName = GetName();
        
        Console.WriteLine("Enter the location for your event ");
        var newLocation=GetName();

        Console.WriteLine("Please enter begging date for your event in format yyyy/mm/dd hh:mm:ss!");
        var newStartDate=GetDate();

        Console.WriteLine("Please enter end date of your event in format yyyy/mm/dd hh:mm:ss! ");
        var newEndDate=GetDate();

        if(newEndDate<newStartDate){
            Console.WriteLine("Your event can't end before it began!");
            return;
        }
        Console.WriteLine("Enter emails of the people who will attend your event:");
        var newEmails=Console.ReadLine().Trim();

        if(newEmails==""){
            Console.WriteLine("Wrong input! Please try again:");
            newEmails=Console.ReadLine().Trim();
        }
        var theEmails=newEmails.Split(",");
    
        foreach(var item in theEmails)
        {
            if(CheckIfEmailIsValid(item))
            {
                if(people.Find(x=>x.Email==item) is null){
                    Console.WriteLine("Person is not located in list!\n");
                    notInList.Add(item);
                }
                else
                    InList.Add(item);
            }
            else{
                Console.WriteLine("Email not valid!");
                notInList.Add(item);
            }
        }
        //var flag=false;
        foreach(var item in InList)
        {
            if(people.Find(x=>x.Email==item).AreEventsOverlaping(newStartDate,newEndDate,events)){
                alreadyAttendingEvent.Add(item);
                //flag=true;
            }
            else
                comingToEvent.Add(item);
        }
        if(alreadyAttendingEvent.Count>0&&comingToEvent.Count==0){
            Console.WriteLine("No one can attend your event! Please try creating different event!");
            return;
        }
        if (comingToEvent.Count>0&&alreadyAttendingEvent.Count==0)
            Console.WriteLine($"Here is a list of people available for your event: {string.Join(", ",comingToEvent)}");
        if(alreadyAttendingEvent.Count>0&&comingToEvent.Count>0)
        {
            //PrintAttendees(newEvent,alreadyAttendingEvent);
            Console.WriteLine($"List of people that are already attending different event at that time: {string.Join(", ", alreadyAttendingEvent)}");
            Console.WriteLine($"Here is a list of people available for your event: {string.Join(", ",comingToEvent)}");
        }
        
        Console.WriteLine("You have drafted an event");
        if(UserInput())
        {
            var theEvent=new Event(Guid.NewGuid(),comingToEvent)
            {
                EventName=newName,
                Location=newLocation,
                StartDate=newStartDate,
                EndDate=newEndDate
            };

            if(theEvent.DoesAnEventExist(events))
                return;
            else{
            events.Add(theEvent);
            foreach(var person in comingToEvent)
            {
                people.Find(x=>x.Email==person).AddEvent(theEvent.Id);
            }
            Console.WriteLine("You have successfully created an event!");
            }
            Console.WriteLine("If you wish to add another person that is not in list enter '1' if not enter '0' to get back to main menu\n");
            int.TryParse(Console.ReadLine(),out var choice);
            if(choice==1)
            {
                AddNewPersonToBeAttendee(events,people,theEvent);
            }
            else if(choice==0)
                return;
        }
    }
    static void AddNewPersonToBeAttendee(List<Event>events,List<Person> people, Event eventGoingTo)
    {
        var newPerson = new Person();
        if (UserInput())
        {
            eventGoingTo.AddPerson(newPerson);
            people.Add(newPerson);
            Console.WriteLine("Adding to event:");
            newPerson.PersonDetails();
        }
        else Console.WriteLine("Person not added!");
    }
    static List<Person> AddPeople(List<Event>events)
    {
        var person1=new Person("Vlado","Vladic","vlado.vladic@gmail.com",new Dictionary<Guid, bool>(){{events[0].Id,true}});
        var person2=new Person("Milana","Mavic","milana.mavic@gmail.com",new Dictionary<Guid, bool>(){{events[0].Id,true}});
        var person3=new Person("Jura","Juric","jura.jur@gmail.com",new Dictionary<Guid, bool>(){{events[2].Id,true}});
        var person4=new Person("Frane","Franic","frane.fran@gmail.com",new Dictionary<Guid, bool>(){{events[1].Id,true},{events[3].Id,true}});
        var person5=new Person("Kaytlaynay","Bul","kat.b@gmail.com",new Dictionary<Guid, bool>(){{events[0].Id,true},{events[1].Id,true},{events[3].Id,true}});
        var person6=new Person("Yvonna","Matulj","yvonna.matulj@gmail.com",new Dictionary<Guid, bool>(){{events[0].Id,true},{events[1].Id,true},{events[2].Id,true},{events[3].Id,false}});
        var person7=new Person("Martina","Martic","martina.martic@gmail.com",new Dictionary<Guid, bool>(){{events[3].Id,true}});
        var person8=new Person("Nina","Mart","nina.mart@gmail.com",new Dictionary<Guid, bool>(){{events[0].Id,true},{events[1].Id,true}});
        var person9=new Person("Mia","Mara","mia.mara@gmail.com",new Dictionary<Guid, bool>(){{events[0].Id,true},{events[1].Id,true},{events[2].Id,true},{events[3].Id,true},{events[4].Id,true}});
        var person10=new Person("Ana","Banana","ana.banana@gmail.com",new Dictionary<Guid, bool>(){{events[1].Id,true},{events[4].Id,true}});

        var people = new List<Person>() {person1, person2, person3,person4,person5,person6,person7,person8,person9,person10};
        return people;
    }
    static List<Event> AddEvent()
    {
         var event1=new Event(Guid.NewGuid(),new List<string>{"kat.b@gmail.com","yvonna.matulj@gmail.com","mia.mara@gmail.com","nina.mart@gmail.com","milana.mavic@gmail.com","vlado.vladic@gmail.com"})
        {
            EventName="Cyber security week",
            Location="Split,FESB",
            StartDate=new DateTime(2022,11,27,10,00,00),
            EndDate=new DateTime(2022,12,02,20,00,00)
        };
        var event2=new Event(Guid.NewGuid(),new List<string>{"ana.banana@gmail.com","kat.b@gmail.com","yvonna.matulj@gmail.com","mia.mara@gmail.com","nina.mart@gmail.com","frane.fran@gmail.com"})
        {
            EventName="Concert Maneskin",
            Location="italy,Milano",
            StartDate=new DateTime(2023,07,25),
            EndDate=new DateTime(2023,07,26)
        };
        var event3=new Event(Guid.NewGuid(),new List<string>{"yvonna.matulj@gmail.com","mia.mara@gmail.com","jura.jur@gmail.com","vlaho.misic@gmail.com"})
        {
            EventName="Oktoberfest",
            Location="Germany,Munich",
            StartDate=new DateTime(2023,09,16),
            EndDate=new DateTime(2023,10,03)
        };
        var event4=new Event(Guid.NewGuid(),new List<string>{"kat.b@gmail.com","yvonna.matulj@gmail.com","mia.mara@gmail.com","frane.fran@gmail.com","martina.martic@gmail.com"})
        {
            EventName="Concert Artic Monkeys",
            Location="Pula",
            StartDate=new DateTime(2022,08,18,20,0,0),
            EndDate=new DateTime(2022,08,19,23,30,00)
        };
        var event5=new Event(Guid.NewGuid(),new List<string>{"nina.mart@gmail.com","mia.mara@gmail.com","ana.banana@gmail.com"})
        {
            EventName="Skiing contenst",
            Location="Madonna di Campiglio",
            StartDate=new DateTime(2023,01,05,10,00,00),
            EndDate=new DateTime(2023,01,10,20,00,00)
        };

        var events=new List<Event>(){event1,event2,event3,event4,event5};
        return events;
            
    }
}
}