using System.Linq;

namespace DevHabit.Api.Services.Sorting;



public sealed record SortMapping(string SortField, string PropertyName, bool Reverse = false);
// Age DESC-> DateOfBirth ASC
// 30 -> 1996
