using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class Utf8JsonRabbitMqSerializer : IRabbitMqSerializer
    {
        public Utf8JsonRabbitMqSerializer()
        {            
        }

        public byte[] Serialize(object obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj, obj.GetType());
        }

        public byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes<T>(obj);
        }

        public object Deserialize(byte[] value, Type type)
        {
            return JsonSerializer.Deserialize(new ReadOnlySpan<byte>(value), type);
        }

        public T Deserialize<T>(byte[] value)
        {
            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(value));
        }
    }
}
