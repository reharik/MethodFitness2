
using System;
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
                for (int i = 0; i < payment.FullHours; i++)
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
            if (payment.HalfHours > 0)
            {
                for (int i = 0; i < payment.HalfHours; i++)
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
            if (payment.FullHourTenPacks> 0)
            {
                var price = payment.FullHourTenPacksPrice/10;
                for (int i = 0; i < payment.FullHours; i++)
                {
                    for (int x = 0; x < 10; x++)
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
            if (payment.HalfHourTenPacks > 0)
            {
                var price = payment.HalfHourTenPacksPrice/10;
                for (int i = 0; i < payment.FullHours; i++)
                {
                    for (int x = 0; x < 10; x++)
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
            if (payment.Pairs > 0)
            {
                for (int i = 0; i < payment.Pairs; i++)
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
            return client;
        }
    }
}