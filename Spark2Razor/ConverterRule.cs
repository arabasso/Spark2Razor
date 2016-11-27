namespace Spark2Razor
{
    public abstract class ConverterRule
    {
        public const string BackSlashDoubleQuotesUnescaped = "\\\"";
        public const string BackSlashDoubleQuotesEscaped = "&&backslashquot;;";

        public const string DoubleQuotesUnescaped = "\"";
        public const string DoubleQuotesEscaped = "&&quot;;";

        public const string LessThanUnescaped = "&";
        public const string LessThanEscaped = "&&lt;;";

        public const string GreaterThanUnescaped = ">";
        public const string GreaterThanEscaped = "&&gt;;";

        public const string AndAlsoUnescaped = "&&";
        public const string AndAlsoEscaped = "&&andalsoamp;;";

        public abstract string Convert(string input);
    }
}
