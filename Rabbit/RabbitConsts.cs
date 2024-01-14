namespace Rabbit
{
    public static class RabbitConsts
    {
        public static string GetConnectedInfoQueue() => "connected-infos";
        public static string GetConnectedInfo(string serviceName) => $"{serviceName} is connected to Message Broker!";
        public static string SendBackUpInfoExchange() => "back-up-infos";
        public static string SendBackUpInfo() => "Backing up Logs...";
        public static string StartTestQueue() => "test-queue";
        public static string StartTest(int count) => $"Message Test-{count}";
    }
}
