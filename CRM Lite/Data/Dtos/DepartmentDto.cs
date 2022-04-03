using System;
using Newtonsoft.Json;

namespace CRM_Lite.Data.Dtos
{
    [JsonObject]
    public class DepartmentDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}