using System.Linq;
using CC.Core;
using CC.Core.Utilities;
using MF.Core.Domain;
using MF.Core.Enumerations;

namespace MF.Core.Services
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
                AddSessions(client,
                    payment,
                    AppointmentType.Hour,
                    payment.FullHour,
                    payment.FullHourPrice);
            }
            if (payment.HalfHour > 0)
            {
                AddSessions(client,
                    payment,
                    AppointmentType.HalfHour,
                    payment.HalfHour,
                    payment.HalfHourPrice);
            }
            if (payment.Pair > 0)
            {
                AddSessions(client,
                    payment,
                    AppointmentType.Pair,
                    payment.Pair,
                    payment.PairPrice);
            }
            if (payment.FullHourTenPack> 0)
            {
                AddSessions(client,
                    payment,
                    AppointmentType.Hour,
                    payment.FullHourTenPack * 10,
                    payment.FullHourTenPackPrice/10);
            }
            if (payment.HalfHourTenPack > 0)
            {
                AddSessions(client,
                    payment,
                    AppointmentType.HalfHour,
                    payment.HalfHourTenPack * 10,
                    payment.HalfHourTenPackPrice/10);
            }
            if (payment.PairTenPack > 0)
            {
                AddSessions(client,
                    payment,
                    AppointmentType.Pair,
                    payment.PairTenPack * 10,
                    payment.PairTenPackPrice/10);
            }
            return client;
        }

        private static void AddSessions(Client client,
            Payment payment,
            AppointmentType type,
            int count,
            double price)
        {
            var sessions = count;
            var typeName = type.ToString();
            client.Sessions
                .Where(x => x.AppointmentType == typeName && x.InArrears)
                .OrderBy(x => x.Appointment.Date)
                .ForEachItem(x =>
                {
                    if(sessions > 0)
                    {
                        x.InArrears = false;
                        x.PurchaseBatchNumber = payment.PaymentBatchId.ToString();
                        x.Cost = price;
                        sessions--;
                    }
                });
            for (int i = 0; i < sessions; i++)
            {
                client.AddSession(new Session
                {
                    Client = client,
                    Cost = price,
                    AppointmentType = typeName,
                    PurchaseBatchNumber = payment.PaymentBatchId.ToString(),
                });
            }
        }
    }
}