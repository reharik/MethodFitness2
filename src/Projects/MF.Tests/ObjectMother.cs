using MF.Core.Domain;

namespace MF.Tests
{
    public static class ObjectMother
    {
        public static Client ValidClient(string name)
        {
            return new Client
                       {
                           FirstName = name,
                           LastName = "Harik",
                           Address1 = "1706 Willow St",
                           City = "Austin",
                           State = "Tx",
                           ZipCode = "78702",
                           Email = "reharik@gmail.com",
                           MobilePhone = "666-6666"
                       };
        }

        public static Payment ValidPayment()
        {
            return new Payment
                       {
                           FullHour = 2,
                           FullHourPrice = 2,
                           FullHourTenPack = 2,
                           FullHourTenPackPrice = 20,
                           HalfHourTenPack = 2,
                           HalfHourTenPackPrice = 20,
                           HalfHour = 2,
                           HalfHourPrice = 2,
                           Pair = 2,
                           PairPrice = 2,
                           PaymentTotal = 200
                       };
        }
    }
}