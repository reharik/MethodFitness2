using System;
using System.Linq;
using System.Web.Mvc;
using CC.Core;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using NUnit.Framework;

namespace MethodFitness.Tests.Services
{
    public class ClientPaymentToSessionsTester
    {
    }

    [TestFixture]
    public class when_calling_execute_on_service
    {
        private Client _client;
        private ClientPaymentToSessions _SUT;
        private Payment _payment;
        private Client _result;

        [SetUp]
        public void Setup()
        {
            _client = ObjectMother.ValidClient("raif");
            _payment = ObjectMother.ValidPayment();
            _SUT = new ClientPaymentToSessions();
            _result = _SUT.Execute(_client,_payment);
        }

        [Test]
        public void should_put_correct_number_of_full_hour_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.Hour.ToString()).ShouldHaveCount(22);
        }

        [Test]
        public void should_put_correct_number_of_half_hour_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.HalfHour.ToString()).ShouldHaveCount(22);
        }

        [Test]
        public void should_put_correct_number_of_pair_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.Pair.ToString()).ShouldHaveCount(2);
        }

        [Test]
        public void should_set_all_hour_sessions_to_correct_price()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.Hour.ToString()).ForEachItem(
                x => x.Cost.ShouldEqual(2));
        }

        [Test]
        public void should_set_all_half_hour_sessions_to_correct_price()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.HalfHour.ToString()).ForEachItem(
                x => x.Cost.ShouldEqual(2));
        }

    }


    [TestFixture]
    public class when_calling_execute_on_service_with_sessions_inarrears_just_singles
    {
        private Client _client;
        private ClientPaymentToSessions _SUT;
        private Payment _payment;
        private Client _result;
        private Session _halfInArrears;
        private Session _fullInArrears;
        private Session _pairInArrears;

        [SetUp]
        public void Setup()
        {
            _client = ObjectMother.ValidClient("raif");
            _halfInArrears = new Session{Client = _client,AppointmentType = AppointmentType.HalfHour.ToString(),SessionUsed = true,InArrears = true};
            _halfInArrears.Appointment = new Appointment {Date = DateTime.Now.AddDays(-1)};
            _fullInArrears = new Session { Client = _client, AppointmentType = AppointmentType.Hour.ToString(), SessionUsed = true, InArrears = true };
            _fullInArrears.Appointment = new Appointment { Date = DateTime.Now.AddDays(-1) };
            _pairInArrears = new Session { Client = _client, AppointmentType = AppointmentType.Pair.ToString(), SessionUsed = true, InArrears = true };
            _pairInArrears.Appointment = new Appointment { Date = DateTime.Now.AddDays(-1) };
            _client.AddSession(_halfInArrears);
            _client.AddSession(_fullInArrears);
            _client.AddSession(_pairInArrears);
            _payment = new Payment {HalfHour = 2,FullHour = 2,Pair = 2};
            _SUT = new ClientPaymentToSessions();
            _result = _SUT.Execute(_client, _payment);
        }

        [Test]
        public void should_put_correct_number_of_full_hour_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.Hour.ToString()
                && !x.InArrears
                && !x.SessionUsed).ShouldHaveCount(1);
        }

        [Test]
        public void should_put_correct_number_of_half_hour_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.HalfHour.ToString()
                && !x.InArrears
                && !x.SessionUsed ).ShouldHaveCount(1 );
        }

        [Test]
        public void should_put_correct_number_of_pairs_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.Pair.ToString()
                && !x.InArrears
                && !x.SessionUsed).ShouldHaveCount(1);
        }
    }

    [TestFixture]
    public class when_calling_execute_on_service_with_sessions_inarrears_tenpacks
    {
        private Client _client;
        private ClientPaymentToSessions _SUT;
        private Payment _payment;
        private Client _result;
        private Session _halfInArrears;
        private Session _fullInArrears;
        private Session _pairInArrears;

        [SetUp]
        public void Setup()
        {
            _client = ObjectMother.ValidClient("raif");
            _halfInArrears = new Session { Client = _client, AppointmentType = AppointmentType.HalfHour.ToString(), SessionUsed = true, InArrears = true };
            _halfInArrears.Appointment = new Appointment { Date = DateTime.Now.AddDays(-1) };
            _fullInArrears = new Session { Client = _client, AppointmentType = AppointmentType.Hour.ToString(), SessionUsed = true, InArrears = true };
            _fullInArrears.Appointment = new Appointment { Date = DateTime.Now.AddDays(-1) };
            _pairInArrears = new Session { Client = _client, AppointmentType = AppointmentType.Pair.ToString(), SessionUsed = true, InArrears = true };
            _pairInArrears.Appointment = new Appointment { Date = DateTime.Now.AddDays(-1) };
            _client.AddSession(_halfInArrears);
            _client.AddSession(_fullInArrears);
            _client.AddSession(_pairInArrears);
            _payment = new Payment { HalfHourTenPack = 1, FullHourTenPack= 1, PairTenPack= 1 };
            _SUT = new ClientPaymentToSessions();
            _result = _SUT.Execute(_client, _payment);
        }

        [Test]
        public void should_put_correct_number_of_full_hour_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.Hour.ToString()
                && !x.InArrears
                && !x.SessionUsed).ShouldHaveCount(9);
        }

        [Test]
        public void should_put_correct_number_of_half_hour_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.HalfHour.ToString()
                && !x.InArrears
                && !x.SessionUsed).ShouldHaveCount(9);
        }

        [Test]
        public void should_put_correct_number_of_pairs_sessions_on_client()
        {
            _result.Sessions.Where(x => x.AppointmentType == AppointmentType.Pair.ToString()
                && !x.InArrears
                && !x.SessionUsed).ShouldHaveCount(9);
        }
    }
}