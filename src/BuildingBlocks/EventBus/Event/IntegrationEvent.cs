using System.Text.Json.Serialization;

namespace EventBus.Event;

public class IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid guid, DateTime createdDate)
    {
        Id = guid;
        CreationDate = createdDate;
    }

    [JsonInclude]
    public Guid Id { get; private set; }

    [JsonInclude]
    public DateTime CreationDate { get; private set; }
}