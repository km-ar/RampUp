﻿using System;
using NSubstitute;
using NUnit.Framework;
using RampUp.Actors;
using RampUp.Actors.Impl;
using RampUp.Ring;

namespace RampUp.Tests.Actors.Impl
{
    public class BusTests : ActorRegistryTestsBase
    {
        private const int RetryCount = 3;
        private IMessageWriter _writer;
        private IBus _bus;

        public struct NotRegistered
        {
        }

        [SetUp]
        public new void SetUp()
        {
            _writer = Substitute.For<IMessageWriter>();
            _bus = new Bus(ABId, Registry, RetryCount, _writer);
        }

        [Test]
        public void WhenNotRegisteredMessageSent_ThenThrows()
        {
            Assert.Throws(Is.AssignableTo<Exception>(), () =>
            {
                var msg = new NotRegistered();
                _bus.Publish(ref msg);
            });
        }

        [Test]
        public void WhenPublish_ThenAllReceiversAreIncluded()
        {
            ArraySegment<IRingBuffer> buffers;
            Registry.GetBuffers(typeof (A), out buffers);
            var expectedCount = buffers.Count;

            A any;
            var e = new Envelope();
            _writer.Write(ref e, ref any, null).ReturnsForAnyArgs(true);
            var a = new A();

            _bus.Publish(ref a);

            _writer.ReceivedWithAnyArgs(expectedCount).Write(ref e, ref a, null);
        }

        [Test]
        public void WhenWriteFailsRetryCountTimes_ThenExceptionIsThrown()
        {
            A any;
            var e = new Envelope();
            _writer.Write(ref e, ref any, null)
                .ReturnsForAnyArgs(call => false, call => false, call => false, call => true);
            var a = new A();

            Assert.Throws(Is.AssignableTo<Exception>(), () => _bus.Send(ref a, AId));
        }

        [Test]
        public void WhenWriteFailsRetryCountMinusOneTimes_ThenFinalSucceeds()
        {
            A any;
            var e = new Envelope();
            _writer.Write(ref e, ref any, null).ReturnsForAnyArgs(call => false, call => false, call => true);
            var a = new A();

            _bus.Send(ref a, AId);
        }
    }
}