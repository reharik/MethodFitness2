using MethodFitness.Core.Domain;

namespace MethodFitness.Tests
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
                           FullHours = 2,
                           FullHoursPrice = 2,
                           FullHourTenPacks = 2,
                           FullHourTenPacksPrice = 20,
                           HalfHourTenPacks = 2,
                           HalfHourTenPacksPrice = 20,
                           HalfHours = 2,
                           HalfHoursPrice = 2,
                           Pairs = 2,
                           PairsPrice = 2,
                           PaymentTotal = 200
                       };
        }
    }
}