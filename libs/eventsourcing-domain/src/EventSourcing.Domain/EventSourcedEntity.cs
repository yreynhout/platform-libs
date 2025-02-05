﻿using System;

namespace Logicality.EventSourcing.Domain;

public abstract class EventSourcedEntity
{
    private readonly EventPlayer   _player   = new EventPlayer();
    private readonly EventRecorder _recorder = new EventRecorder();

    public void RestoreFromEvents(object[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        if (_recorder.HasRecordedEvents)
        {
            throw new InvalidOperationException("Restoring from events can not be done on an instance with recorded events.");
        }

        foreach (var @event in events)
        {
            _player.Play(@event);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public object[] TakeEvents()
    {
        var recorded = _recorder.RecordedEvents;
        _recorder.Reset();
        return recorded;
    }

    protected void On<TEvent>(Action<TEvent> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }
            
        _player.Register(handler);
    }

    protected void Apply(object @event)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }
            
        _player.Play(@event);
        _recorder.Record(@event);
    }
}