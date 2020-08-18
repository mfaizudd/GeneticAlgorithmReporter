using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeneticAlgorithmReporter
{
    class GeneConverter : JsonConverter<Gene>
    {
        public override Gene Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var gene = new Gene();
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    gene.value = reader.GetString();
                    break;
                case JsonTokenType.Number:
                    gene.value = reader.GetDouble();
                    break;
            }
            return gene;
        }

        public override void Write(Utf8JsonWriter writer, Gene value, JsonSerializerOptions options)
        {
            
        }
    }
}
