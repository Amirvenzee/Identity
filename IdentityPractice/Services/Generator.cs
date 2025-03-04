namespace IdentityPractice.Services
{
    public  class Generator
    {
        public static string RandomNumber()
        {
            var random = new Random();

            var Number = random.Next(0, 100000).ToString("D6");

            return Number;
        }
    }
}
