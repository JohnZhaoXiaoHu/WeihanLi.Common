﻿namespace WeihanLi.Common.Event;

public interface IEventQueue
{
    ICollection<string> GetQueues();

    Task<ICollection<string>> GetQueuesAsync();

    bool Enqueue<TEvent>(string queueName, TEvent @event)
        where TEvent : class, IEventBase;

    Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event)
        where TEvent : class, IEventBase;

    IEventBase? Dequeue(string queueName);

    Task<IEventBase?> DequeueAsync(string queueName);
}

public static class EventQueueExtensions
{
    private const string DefaultQueueName = "events";

    public static bool Enqueue<TEvent>(this IEventQueue eventQueue, TEvent @event)
        where TEvent : class, IEventBase
    {
        return eventQueue.Enqueue(DefaultQueueName, @event);
    }

    public static Task<bool> EnqueueAsync<TEvent>(this IEventQueue eventQueue, TEvent @event)
        where TEvent : class, IEventBase
    {
        return eventQueue.EnqueueAsync(DefaultQueueName, @event);
    }

    public static bool TryDequeue(this IEventQueue eventQueue, string queueName, out IEventBase? @event)
    {
        @event = eventQueue.Dequeue(queueName);
        return @event is not null;
    }

    public static bool TryDequeue(this IEventQueue eventQueue, out IEventBase? @event)
        => TryDequeue(eventQueue, DefaultQueueName, out @event);

    public static IEventBase? Dequeue(this IEventQueue eventQueue)
    {
        return eventQueue.Dequeue(DefaultQueueName);
    }

    public static Task<IEventBase?> DequeueAsync(this IEventQueue eventQueue)
    {
        return eventQueue.DequeueAsync(DefaultQueueName);
    }
}
