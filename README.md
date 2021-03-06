![Icon](icons/package_icon.png)

# RampUp
RampUp is a .NET library providing a performant low/no-alloc environment for demanding systems. It's based on understanding the modern hardware and applying the mechanical sympathy.

## The journey
RampUp has been started as a journey project and it's still in this phase. The goal of this journey is simple: provide a high level abstraction, a layer that enables writing extremely performant systems (probably not applications) in C#/.NET for modern hardware. Initial tests show that this approach may be valid. The provided infrastructure is able to handle 1 million messages in ~100 miliseconds on a single machine. Yes, that's 10 millions per second without any allocations at all! It's worth to mention that messages are sent by two publishers and there's only one consumer!
I'm aiming at ending this journey with a real OSS product for building highly demanding systems.

## The aim
RampUp aims at providing foundations for systems like databases, queues, web servers and other distributed systems. The foundations are based on knowledge about moder hardware and are meant to abstract these concerns properly. Hopefully, a faster version of a network IO will be provided as well, possibly using an approach similar to Aeron (Nack based publication) for local networks. Again, it's still a journey.


## Example
The example of the current high level API for creating in memory actors on one machine can be find below. I hope you'll like this ping pong peformed by Bruce & Lee.

```
public class Program
{
    public static void Main()
    {
        var system = new ActorSystem();

        system.Add(new Bruce(), ctx => { ctx.Actor.Bus = ctx.Bus; });
        system.Add(new Lee(), ctx => { ctx.Actor.Bus = ctx.Bus; });

        var c = system.Start();

        var p = new Pong();
        c.Publish(ref p); // start with Pong

        // ... later

        c.Stop();
    }
}

public class Bruce : IHandle<Ping>
{
    public IBus Bus;

    public void Handle(ref Envelope envelope, ref Ping msg)
    {
        var p = new Pong();
        Bus.Publish(ref p);
    }
}

public class Lee : IHandle<Pong>
{
    public IBus Bus;

    public void Handle(ref Envelope envelope, ref Pong msg)
    {
        var p = new Ping();
        Bus.Publish(ref p);
    }
}

public struct Pong : IMessage {}

public struct Ping : IMessage {}
```


## The inspiration corner
There are many great projects out there that are an inspiration for RampUp:
- [EventStore](https://github.com/EventStore/EventStore) - an event database; it uses SEDA approach and an in-memory messaging
- [Akka.NET](https://github.com/akkadotnet/akka.net) - a very active actor's framework for .NET
- [Aeron](https://github.com/real-logic/Aeron) - an extremely performant messaging system using UDP + negative acknowledgements; brought to life by Martin Thomson, the Java mechanical sympathy guru

## Icon
<a href="https://thenounproject.com/term/graph/32972/" target="_blank">Graph</a> designed by <a href="https://thenounproject.com/phdieuli/" target="_blank">Pham Thi Dieu Linh</a> from The Noun Project
