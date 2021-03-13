using System;
using FullSerializer;

namespace SeedFinder
{
    public static class JsonSerializer
    {
        private static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize(Type type, object value)
        {
            _serializer.TrySerialize(type, value, out fsData data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }

        public static object Deserialize(Type type, string serializedState)
        {
            fsData data = fsJsonParser.Parse(serializedState);

            object deserialized = null;
            _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

            return deserialized;
        }
    }
}