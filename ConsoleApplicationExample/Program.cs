using System;
using System.Collections.Generic;
using System.Text;

// Required DDay.iCal namespace
using DDay.iCal;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load the calendar file
            IICalendarCollection calendars = iCalendar.LoadFromFile(@"Business.ics");

            //
            // Get all events that occur today.
            //
            IList<Occurrence> occurrences = calendars.GetOccurrences(DateTime.Today, DateTime.Today.AddDays(1));

            Console.WriteLine("Today's Events:");

            // Iterate through each occurrence and display information about it
            foreach (Occurrence occurrence in occurrences)
            {
                DateTime occurrenceTime = occurrence.Period.StartTime.Local;
                IRecurringComponent rc = occurrence.Source as IRecurringComponent;
                if (rc != null)
                    Console.WriteLine(rc.Summary + ": " + occurrenceTime.ToShortTimeString());
            }

            //
            // Get all occurrences for the next 7 days, starting tomorrow.
            //
            occurrences = calendars.GetOccurrences(DateTime.Today.AddDays(1), DateTime.Today.AddDays(7));

            Console.WriteLine(Environment.NewLine + "Upcoming Events:");

            // Start with tomorrow
            foreach (Occurrence occurrence in occurrences)
            {
                DateTime occurrenceTime = occurrence.Period.StartTime.Local;
                IRecurringComponent rc = occurrence.Source as IRecurringComponent;
                if (rc != null)
                    Console.WriteLine(rc.Summary + ": " + occurrenceTime.ToString());
            }
        }
    }
}