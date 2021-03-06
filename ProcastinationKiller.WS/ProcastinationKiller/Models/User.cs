﻿using ProcastinationKiller.Models.Enums;
using ProcastinationKiller.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public string Email { get; set; }

        public DateTime Regdate { get; set; }

        public ICollection<TodoItem> UserTodos { get; set; }

        public ICollection<BaseEvent> Events { get; set; }

        public UserStatus UserStatus { get; set; }

        public RegistartionCode Code { get; set; }

        public UserState CurrentState { get; set; }

        // Ogarnąc jak zmusić EF do DI
        private StateCalculationService _calculationService = new StateCalculationService();

        public User()
        {
            UserTodos = new HashSet<TodoItem>();
            _days = _days ?? new HashSet<Day>();
        }

        [NotMapped]
        public ICollection<Day> Callendar
        {
            get
            {
                InitializeCallendar();
                return _days;
            }
            private set
            {

            }
        }

        private ICollection<Day> _days;

        private void InitializeCallendar()
        {

            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            for(int day = 1; day <= days; day++)
            {
                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);

                if (UserTodos.Any(x => x.TargetDate.Date == date))
                {
                    _days.Add(new Day()
                    {
                        Todos = UserTodos.Where(x => x.TargetDate.Date == date).ToList(),
                        AllCompleted = UserTodos.Where(x => x.TargetDate.Date == date).All(x => x.Completed),
                        Date = date
                    });
                }
                else
                {
                    _days.Add(new Day()
                    {
                        Date = date,
                        Todos = new List<TodoItem>()
                    });
                }
            }

            foreach (var todo in UserTodos.Where(x => !(x.TargetDate.Month == DateTime.Now.Month && x.TargetDate.Year == DateTime.Now.Year)))
            {
                Day day = _days.SingleOrDefault(x => x.Date.Date == todo.TargetDate.Date);

                if (day == null)
                {
                    day = new Day();
                    day.Date = todo.TargetDate;
                    _days.Add(day);
                }

                if (day.Todos == null)
                {
                    day.Todos = new HashSet<TodoItem>();
                }

                day.Todos.Add(todo);

                if (day.AllCompleted && !todo.Completed)
                    day.AllCompleted = false;
            }
        }

        internal void AddTodoCompletedEvent(TodoItem todo)
        {
            if (Events.OfType<TodoCompletedEvent>().Any(x => x.CompletedItem == todo && !x.Hidden))
            {
                throw new Exception("User already have event about completeing this item.");
            }

            AddEvents(new TodoCompletedEvent()
            {
                Hidden = false,
                Date = todo.FinishTime.Value,
                CompletedItem = todo
            });
        }

        public void AddCode(string code)
        {
            UserStatus = UserStatus.New;
            Code = new RegistartionCode()
            {
                Code = code,
                IsConfirmed = false
            };
        }

        internal void AddDailyLoginReward(DateTime currentTime)
        {
            if (Events.OfType<DailyLoginEvent>().Any(x => x.Date.Date == currentTime.Date && !x.Hidden))
                return;

            AddEvents(new DailyLoginEvent()
            {
                Date = currentTime,
                Hidden = false
            });

            if (ShouldAddWeeklyEvent(currentTime))
            {
                AddEvents(new WeeklyLoginEvent()
                {
                    Date = currentTime,
                    Hidden = false
                });
            }
        }

        internal void AddEvents(IEnumerable<BaseEvent> events)
        {
            Events = Events.Concat(events).ToList();
            this.CurrentState = _calculationService.Calculate(Events, DateTime.Now);
        }

        internal void AddEvents(BaseEvent @event)
        {
            Events.Add(@event);
            this.CurrentState = _calculationService.Calculate(Events, DateTime.Now);
        }

        private bool ShouldAddWeeklyEvent(DateTime currentTime)
        {
            var expectedDates = new List<DateTime>()
            {
                currentTime,
                currentTime.AddDays(-1),
                currentTime.AddDays(-2),
                currentTime.AddDays(-3),
                currentTime.AddDays(-4),
                currentTime.AddDays(-5),
                currentTime.AddDays(-6),
            };

            var dailyLoginOperations = Events.OfType<DailyLoginEvent>().Where(x => !x.Hidden).Select(x => x.Date);

            return expectedDates.All(x => dailyLoginOperations.Any(y => y.Date == x.Date))
                && !Events.OfType<WeeklyLoginEvent>()
                    .Any(x => 
                        x.Date.Date >= currentTime.AddDays(-7) 
                        && x.Date.Date <= currentTime.Date);
        }

        internal void Calculate()
        {
            CurrentState = _calculationService.Calculate(Events, DateTime.Now);
        }
    }
}
