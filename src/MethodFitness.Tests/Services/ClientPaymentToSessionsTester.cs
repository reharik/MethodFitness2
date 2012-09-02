using System.Linq;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
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
}