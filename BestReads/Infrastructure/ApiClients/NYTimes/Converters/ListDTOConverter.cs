using BestReads.Infrastructure.ApiClients.NYTimes.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BestReads.Infrastructure.ApiClients.NYTimes.Converters;

public class ListDTOConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ListDTO);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jo = JObject.Load(reader);
        var list = new ListDTO();
        list.ListId = (int)jo["list_id"];
        list.ListName = (string)jo["list_name"];
        list.DisplayName = (string)jo["display_name"];
        list.Books = jo["books"].ToObject<List<BookDTO>>(serializer);
        return list;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}