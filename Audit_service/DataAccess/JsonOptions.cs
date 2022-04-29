using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.DataAccess
{
    public class IntJsonConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return 0;
            if (reader.TokenType == JsonTokenType.String)
            {
                if (string.IsNullOrEmpty(reader.GetString()) || reader.GetString() == null)
                    return 0;
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out int number, out int bytesConsumed) && span.Length == bytesConsumed)
                {
                    return number;
                }

                if (int.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }

            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class IntNullableJsonConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetInt32();
            if (reader.TokenType == JsonTokenType.Null || string.IsNullOrEmpty(reader.GetString()))
                return null;
            if (reader.TokenType == JsonTokenType.String)
            {
                if (string.IsNullOrEmpty(reader.GetString()) || reader.GetString() == null)
                    return 0;
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out int number, out int bytesConsumed) && span.Length == bytesConsumed)
                {
                    return number;
                }

                if (int.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }

            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(!value.HasValue ? null : value.ToString());
        }
    }
    public class DoubleJsonConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return 0;
            if (reader.TokenType == JsonTokenType.String)
            {
                if (string.IsNullOrEmpty(reader.GetString()) || reader.GetString() == null)
                    return (double)0;
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out double number, out int bytesConsumed) && span.Length == bytesConsumed)
                {
                    return number;
                }

                if (double.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }

            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class DoubleNullableJsonConverter : JsonConverter<double?>
    {
        public override double? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDouble();
            if (reader.TokenType == JsonTokenType.Null || string.IsNullOrEmpty(reader.GetString()))
                return null;
            if (reader.TokenType == JsonTokenType.String)
            {
                if (string.IsNullOrEmpty(reader.GetString()) || reader.GetString() == null)
                    return null;
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out double number, out int bytesConsumed) && span.Length == bytesConsumed)
                {
                    return number;
                }

                if (double.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }

            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(!value.HasValue ? null : value.ToString());
        }
    }
    public class FormatString : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.GetString().Trim();
        }
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            value = (value + "").Trim();
            writer.WriteStringValue(value);
        }
    }
    public class StatusImport : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
                return true;
            if (reader.TokenType == JsonTokenType.False)
                return false;

            var str = reader.GetString();
            return str == "Hoạt động" || str == "1" || str == "true" || str == "True";
        }
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value ? "1" : "0");
        }
    }
}
