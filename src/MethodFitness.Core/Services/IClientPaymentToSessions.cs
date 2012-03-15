
using System;
using System.Linq;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;

namespace MethodFitness.Core.Services
{
    public interface IClientPaymentToSessions
    {
        Client Execute(Client client, Payment payment);
    }

    public class ClientPaymentToSessions : IClientPaymentToSessions
    {
        public Client Execute(Client client, Payment payment)
        {
            if (payment.FullHours > 0)
            {
                HourSessions(client, payment);
            }
            if (payment.HalfHours > 0)
            {
                HalfHourSessions(client, payment);
            }
            if (payment.FullHourTenPacks> 0)
            {
                FullHourTenPackSessions(client, payment);
            }
            if (payment.HalfHourTenPacks > 0)
            {
                HalfHourTenPackSessions(client, payment);
            }
            if (payment.Pairs > 0)
            {
                PairSessions(client, payment);
            }
            return client;
        }

        private static void PairSessions(Client client, Payment payment)
        {
            var sessions = payment.Pairs;
            client.Sessions
                .Where(x => x.AppointmentType == AppointmentType.Pair.ToString() && x.InArrears)
                .Each(x =>
                          {
                              x.InArrears = false;
                              x.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                              x.Cost = payment.PairsPrice;
                              sessions--;
                          });
            for (int i = 0; i < sessions; i++)
            {
                var session = new Session
                                  {
                                      Client = client,
                                      Cost = payment.PairsPrice,
                                      AppointmentType = AppointmentType.Pair.ToString(),
                                      PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                  };
                client.AddSession(session);
            }
        }

        private static void HalfHourTenPackSessions(Client client, Payment payment)
        {
            var inArrears = client.Sessions.Count(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && x.InArrears);
            var price = payment.HalfHourTenPacksPrice/10;
            for (int i = 0; i < payment.HalfHourTenPacks; i++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (inArrears > 0)
                    {
                        var arrear = client.Sessions.First(s => s.AppointmentType == AppointmentType.HalfHour.ToString() && s.InArrears);
                        arrear.InArrears = false;
                        arrear.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                        arrear.Cost = price;
                        inArrears--;
                    }
                    else
                    {
                        var session = new Session
                                          {
                                              Client = client,
                                              Cost = price,
                                              AppointmentType = AppointmentType.HalfHour.ToString(),
                                              PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                          };
                        client.AddSession(session);
                    }
                }
            }
        }

        private static void FullHourTenPackSessions(Client client, Payment payment)
        {
            var inArrears = client.Sessions.Count(x => x.AppointmentType == AppointmentType.Hour.ToString() && x.InArrears);
            var price = payment.FullHourTenPacksPrice/10;
            for (int i = 0; i < payment.FullHourTenPacks; i++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (inArrears > 0)
                    {
                        var arrear = client.Sessions.First(s => s.AppointmentType == AppointmentType.Hour.ToString() && s.InArrears);
                        arrear.InArrears = false;
                        arrear.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                        arrear.Cost = price;
                        inArrears--;
                    }
                    else
                    {
                        var session = new Session
                                          {
                                              Client = client,
                                              Cost = price,
                                              AppointmentType = AppointmentType.Hour.ToString(),
                                              PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                          };
                        client.AddSession(session);
                    }
                }
            }
        }

        private static void HalfHourSessions(Client client, Payment payment)
        {
            var sessions = payment.HalfHours;
            client.Sessions
                .Where(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && x.InArrears)
                .Each(x =>
                {
                    x.InArrears = false;
                    x.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                    x.Cost = payment.HalfHoursPrice;
                    sessions--;
                });
            for (int i = 0; i < sessions; i++)
            {
                var session = new Session
                                  {
                                      Client = client,
                                      Cost = payment.HalfHoursPrice,
                                      AppointmentType = AppointmentType.HalfHour.ToString(),
                                      PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                  };
                client.AddSession(session);
            }
        }

        private static void HourSessions(Client client, Payment payment)
        {
            var sessions = payment.FullHours;
            client.Sessions
                .Where(x => x.AppointmentType == AppointmentType.Hour.ToString() && x.InArrears)
                .Each(x =>
                {
                    x.InArrears = false;
                    x.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                    x.Cost = payment.FullHoursPrice;
                    sessions--;
                });
            for (int i = 0; i < sessions; i++)
            {
                var session = new Session
                                  {
                                      Client = client,
                                      Cost = payment.FullHoursPrice,
                                      AppointmentType = AppointmentType.Hour.ToString(),
                                      PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                  };
                client.AddSession(session);
            }
        }
    }
}