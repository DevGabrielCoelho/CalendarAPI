using CalendarAPI.Data;
using CalendarAPI.Interfaces;
using CalendarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEmailService _emailService;

        public EmailBackgroundService(IServiceScopeFactory service, IEmailService emailService)
        {
            _scopeFactory = service;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var now = DateTime.UtcNow;

                    if (context.Events == null)
                        throw new InvalidOperationException("Events DbSet is null.");

                    var eventsToNotify = await context.Events
                        .Include(e => e.Reminders)
                        .Include(e => e.User)
                        .Where(e => e.DateStart > now && !e.Reminders.All(x => x.SendedEmail))
                        .ToListAsync(stoppingToken);

                    foreach (var calendarEvent in eventsToNotify)
                    {
                        var reminder = calendarEvent.Reminders
                            .OrderByDescending(x => x.MinutesBefore)
                            .FirstOrDefault(x => !x.SendedEmail);

                        if (reminder != null && calendarEvent.DateStart - now <= TimeSpan.FromMinutes(reminder.MinutesBefore))
                        {
                            await NotifyUser(calendarEvent);

                            reminder.SendedEmail = true;
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task NotifyUser(Event calendarEvent)
        {
            var emailBody = $@"
                <h1>Hello!</h1>
                <p>You have a scheduled event:</p>
                <ul>
                    <li><strong>Description:</strong> {calendarEvent.Description}</li>
                    <li><strong>Date:</strong> {calendarEvent.DateStart:MM/dd/yyyy HH:mm}</li>
                    <li><strong>Location:</strong> {calendarEvent.Location}</li>
                </ul>
            ";

            await _emailService.SendEmailAsync(calendarEvent.User.Email, "Event Reminder", emailBody);

            foreach (var guestEmail in calendarEvent.GuestsEmails)
            {
                await _emailService.SendEmailAsync(guestEmail, "Event Reminder", emailBody);
            }
        }
    }
}