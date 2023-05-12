using System.Text.Json.Serialization;

namespace DeliveryApi.Models
{
    public abstract class BaseModel
    {
        [JsonPropertyOrder(-1)]
        public string? Id { get; set; }

        [JsonPropertyOrder(1)]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyOrder(2)]
        public DateTime? UpdatedAt { get; set; }
    }
}