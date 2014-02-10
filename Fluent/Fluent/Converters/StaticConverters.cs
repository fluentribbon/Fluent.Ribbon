namespace Fluent.Converters
{
    public static class StaticConverters
    {
        public static readonly InvertNumericConverter InvertNumericConverter = new InvertNumericConverter();

        public static readonly ThicknessConverter ThicknessConverter = new ThicknessConverter();
    }
}