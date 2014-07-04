namespace TinyJsonDumper.Loggable
{
    public static class LoggerExtension
    {
        public static string DumpLoggable<T>(this T instance) where T : class
        {
            var dumper = new LoggableDumper();
            return dumper.Dump(instance);
        }

        public static string DumpLoggable<T>(this T instance, bool enableIndentation) where T : class
        {
            var dumper = new LoggableDumper
            {
                EnableIndentation = enableIndentation
            };
            return dumper.Dump(instance);
        }
    }
}
