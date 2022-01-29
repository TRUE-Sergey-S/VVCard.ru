namespace vvcard
{
    public static class ServiceAndBotDetector
    {
        public static bool Check(string userAgent)
        {
            return userAgent.Contains("bot");
        }
    }
}
