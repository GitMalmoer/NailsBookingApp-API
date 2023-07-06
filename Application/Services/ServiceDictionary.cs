namespace NailsBookingApp_API.Services
{
    public static class ServiceDictionary
    {
        private static Dictionary<int, double> servicePrices = new Dictionary<int, double>()
        {
            { 1, 400 },
            { 2, 450 },
            { 3, 400 },
            { 4, 350 },
            { 5, 350 },
            { 6, 100 },
        };
        public static double GetPriceByService(int serviceValue)
        {
            double priceOfService;

            if (servicePrices.TryGetValue(serviceValue, out priceOfService))
            {
                return priceOfService;
            }
            else
            {
                return 0;
            }
        }

    }
}
