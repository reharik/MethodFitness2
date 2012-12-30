
using System;
using System.Linq;
using CC.Core;
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
            if (payment.FullHour > 0)
            {
                HourSessions(client, payment);
            }
            if (payment.HalfHour > 0)
            {
                HalfHourSessions(client, payment);
            }
            if (payment.FullHourTenPack> 0)
            {
                FullHourTenPackSessions(client, payment);
            }
            if (payment.HalfHourTenPack > 0)
            {
                HalfHourTenPackSessions(client, payment);
            }
            if (payment.Pair > 0)
            {
                PairSessions(client, payment);
            }
            if (payment.PairTenPack > 0)
            {
                PairTenPackSessions(client, payment);
            }
            return client;
        }

        private static void PairSessions(Client client, Payment payment)
        {
            var sessions = payment.Pair;
            client.Sessions
                .Where(x => x.AppointmentType == AppointmentType.Pair.ToString() && x.InArrears).OrderBy(x=>x.Appointment.Date)
                .ForEachItem(x =>
                          {
                              x.InArrears = false;
                              x.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                              x.Cost = payment.PairPrice;
                              sessions--;
                          });
            for (int i = 0; i < sessions; i++)
            {
                var session = new Session
                                  {
                                      Client = client,
                                      Cost = payment.PairPrice,
                                      AppointmentType = AppointmentType.Pair.ToString(),
                                      PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                  };
                client.AddSession(session);
            }
        }

        private static void PairTenPackSessions(Client client, Payment payment)
        {
            var inArrears = client.Sessions.Count(x => x.AppointmentType == AppointmentType.Pair.ToString() && x.InArrears);
            var price = payment.PairTenPackPrice/10;
            for (int i = 0; i < payment.PairTenPack; i++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (inArrears>0)
                    {
                        var arrear = client.Sessions.Where(s => s.AppointmentType == AppointmentType.Pair.ToString() && s.InArrears).OrderBy(s => s.Appointment.Date).First();
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
                                              AppointmentType = AppointmentType.Pair.ToString(),
                                              PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                          };
                        client.AddSession(session);
                    }
                }
            }
        }

        private static void HalfHourTenPackSessions(Client client, Payment payment)
        {
            var inArrears = client.Sessions.Count(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && x.InArrears);
            var price = payment.HalfHourTenPackPrice/10;
            for (int i = 0; i < payment.HalfHourTenPack; i++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (inArrears>0)
                    {
                        var arrear = client.Sessions.Where(s => s.AppointmentType == AppointmentType.HalfHour.ToString() && s.InArrears).OrderBy(s => s.Appointment.Date).First();
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
            var price = payment.FullHourTenPackPrice/10;
            for (int i = 0; i < payment.FullHourTenPack; i++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (inArrears>0)
                    {
                        var arrear = client.Sessions.Where(s => s.AppointmentType == AppointmentType.Hour.ToString() && s.InArrears).OrderBy(s => s.Appointment.Date).First();
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
            var sessions = payment.HalfHour;
            client.Sessions.Where(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && x.InArrears).OrderBy(x => x.Appointment.Date)
                .ForEachItem(x =>
                {
                    x.InArrears = false;
                    x.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                    x.Cost = payment.HalfHourPrice;
                    sessions--;
                });
            for (int i = 0; i < sessions; i++)
            {
                var session = new Session
                                  {
                                      Client = client,
                                      Cost = payment.HalfHourPrice,
                                      AppointmentType = AppointmentType.HalfHour.ToString(),
                                      PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                  };
                client.AddSession(session);
            }
        }

        private static void HourSessions(Client client, Payment payment)
        {
            var sessions = payment.FullHour;
            client.Sessions
                .Where(x => x.AppointmentType == AppointmentType.Hour.ToString() && x.InArrears).OrderBy(x => x.Appointment.Date)
                .ForEachItem(x =>
                {
                    if (sessions > 0)
                    {
                        x.InArrears = false;
                        x.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                        x.Cost = payment.FullHourPrice;
                        sessions--;
                    }
                });
            for (int i = 0; i < sessions; i++)
            {
                var session = new Session
                                  {
                                      Client = client,
                                      Cost = payment.FullHourPrice,
                                      AppointmentType = AppointmentType.Hour.ToString(),
                                      PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                                  };
                client.AddSession(session);
            }
        }
    }
}