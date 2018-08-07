using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// Required DDay.iCal namespace
using DDay.iCal;

public partial class _Default : System.Web.UI.Page
{
    #region Protected Fields

    /// <summary>
    /// The absolute path to the folder that contains our iCalendars
    /// </summary>
    protected string _CalendarAbsPath;

    /// <summary>
    /// A list of calendars that have been loaded from file
    /// </summary>
    protected iCalendarCollection _Calendars = null;

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        _CalendarAbsPath = MapPath("~/Calendars");

        if (!IsPostBack)
        {
            // Load our list of available calendars
            CalendarList.DataSource = LoadCalendarList();
            CalendarList.DataBind();

            // Select all calendars in the list by default
            foreach (ListItem li in CalendarList.Items)
                li.Selected = true;
        }

        // Build a list of today's events, and bind our list
        // to the repeater that will display the events.
        TodaysEvents.DataSource = GetTodaysEvents();
        TodaysEvents.DataBind();

        // Build a list of upcoming events and bind our list
        // to the repeater that will display the events.
        UpcomingEvents.DataSource = GetUpcomingEvents();
        UpcomingEvents.DataBind();
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Gets a list of iCalendars that are in the "Calendars" directory.
    /// </summary>
    /// <returns>
    /// A list of filenames, without extensions, of the iCalendars 
    /// in the "Calendars" directory
    /// </returns>
    protected IEnumerable<string> LoadCalendarList()
    {
        foreach (string file in Directory.GetFiles(_CalendarAbsPath, "*.ics"))
            yield return Path.GetFileNameWithoutExtension(file);
    }

    /// <summary>
    /// Loads and parses the selected calendars.
    /// </summary>
    protected void LoadSelectedCalendars()
    {
        _Calendars = new iCalendarCollection();
        foreach (ListItem li in CalendarList.Items)
        {
            // Make sure the item is selected
            if (li.Selected)
            {
                // Load the calendar from the file system
                _Calendars.AddRange(iCalendar.LoadFromFile(Path.Combine(_CalendarAbsPath, li.Text + @".ics")));                
            }
        }
    }

    /// <summary>
    /// Gets a list of events that occur today.
    /// </summary>
    /// <returns>A list of events that occur today</returns>
    protected IList<Occurrence> GetTodaysEvents()
    {
        // Load selected calendars, if we haven't already
        if (_Calendars == null)
            LoadSelectedCalendars();

        // Get all events that occur today.
        return _Calendars.GetOccurrences<IEvent>(DateTime.Today, DateTime.Today.AddDays(1));
    }

    /// <summary>
    /// Gets a list of upcoming events (event that will occur within the
    /// next week).
    /// </summary>
    /// <returns>A list of events that will occur within the next week</returns>
    protected IList<Occurrence> GetUpcomingEvents()
    {
        // Load selected calendars, if we haven't already
        if (_Calendars == null)
            LoadSelectedCalendars();

        return _Calendars.GetOccurrences<IEvent>(DateTime.Today.AddDays(1), DateTime.Today.AddDays(7));
    }

    /// <summary>
    /// Returns a string representation of the start
    /// time of an event.
    /// </summary>
    /// <param name="obj">The event for which to display the start time</param>
    /// <returns>A string representation of the start time of an event</returns>
    protected string GetTimeDisplay(object obj)
    {
        if (obj is Occurrence)
        {
            Occurrence occurrence = (Occurrence)obj;
            IEvent evt = occurrence.Source as IEvent;
            if (evt != null)
            {
                if (evt.IsAllDay)
                    return "All Day";
                else return occurrence.Period.StartTime.Local.ToString("h:mm tt");
            }
        }
        return string.Empty;
    }

    #endregion
}
