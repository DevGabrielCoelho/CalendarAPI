using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Data;
using CalendarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EmailService _emailService;

        public EmailBackgroundService(IServiceScopeFactory service, EmailService emailService)
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
                    if(context.Events == null)throw new Exception();
                    var eventsToNotify = await context.Events
                        .Include(e => e.Reminders)
                        .Include(e => e.User)
                        .Where(e => e.DateStart > now && !e.Reminders.All(x => x.SendedEmail))
                        .ToListAsync(stoppingToken);

                    foreach (var calendarEvent in eventsToNotify)
                    {
                        var reminder = new Reminder();
                        foreach(Reminder x in calendarEvent.Reminders.OrderByDescending(x => x.MinutesBefore)){
                            if(!x.SendedEmail){
                                reminder = x;
                                break;
                            }
                        }
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
        <h1>Olá!</h1>
        <p>Você tem um evento agendado:</p>
        <ul>
            <li><strong>Descrição:</strong> {calendarEvent.Description}</li>
            <li><strong>Data:</strong> {calendarEvent.DateStart:dd/MM/yyyy HH:mm}</li>
        </ul>";
            await _emailService.SendEmailAsync(calendarEvent.User.Email, "Lembrete de Evento", emailBody);
            foreach(string x in calendarEvent.GuestsEmails){
                await _emailService.SendEmailAsync(x, "Lembrete de Evento", emailBody);
            }
        }

    }
}