using FerienspassWebApp.Data;
using FerienspassWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FerienspassWebApp.Services
{
    public class EventService
    {
        private readonly ApplicationDbContext _db;

        public EventService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool IsChildAvailable(int childId, DateTime start, DateTime end)
        {
            return !_db.EventChildren
                .Include(x => x.Event)
                .Any(x =>
                x.ChildId == childId &&
                x.Event.StartZeit < end &&
                x.Event.EndeZeit < start);
        }

        public void UpdateEventStatus(Event ev)
        {
            var confirmed = _db.EventChildren.Count(x =>
                x.EventId == ev.Id && x.Status == EnrollmentStatus.Confirmed);

            var waiting = _db.EventChildren.Count(x =>
            x.EventId == ev.Id && x.Status == EnrollmentStatus.WaitingList);

            if (waiting > ev.WaitlisteLimit)
                ev.IsFull = true;
            else
                ev.IsFull = false;
        }
    }
}
