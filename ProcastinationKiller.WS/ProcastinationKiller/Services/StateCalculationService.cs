﻿using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services.Abstract;
using ProcastinationKiller.Services.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services
{
    public class StateCalculationService : IStateCalculationService
    {
        private Dictionary<Type, Func<BaseEvent, UserState, UserState>> _handlers = new Dictionary<Type, Func<BaseEvent, UserState, UserState>>();

        public StateCalculationService()
        {
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            AddHandler<TodoCompletedEvent, TodoCompletedCalculationHandler>();
            AddHandler<DailyLoginEvent, DailyLoginCalculationHandler>();
        }

        private void AddHandler<TEvent, THandler>()
            where THandler: IEventCalculationHandler<TEvent>, new()
            where TEvent: BaseEvent
        {
            var handler = (THandler)Activator.CreateInstance<THandler>();
            _handlers.Add(typeof(TEvent), (operation, state) => handler.Calculate((TEvent)operation, state));
        }

        public UserState Calculate(IEnumerable<BaseEvent> baseEvents, DateTime currentTime)
        {
            // Posortuj eventy w kolejności wystąpienia

            var eventQueue = baseEvents.OrderBy(x => x.Date);

            BaseEvent lastOperation = null;
            UserState currentState = null;
            foreach(var @event in eventQueue)
            {
                var handler = _handlers[@event.GetType()];
                @event.State = handler(@event, UserState.Copy(lastOperation?.State ?? new UserState()));
                lastOperation = @event;

                if (@event.Date <= currentTime)
                    currentState = @event.State;
            }

            return currentState;
        }
    }
}
