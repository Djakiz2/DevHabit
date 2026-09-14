
namespace DevHabit.Api.DTOs.Common;

public sealed class CollectionResponse<T> : ICollectionResponse<T>, ILinksResponse
{
    public List<LinkDto> Links { get; set; }
    public List<T> Items { get; init; }
}
