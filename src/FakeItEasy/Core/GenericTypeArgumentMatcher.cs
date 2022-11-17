namespace FakeItEasy.Core;

using System;
using System.Collections.Concurrent;
using System.Reflection;

internal class GenericTypeArgumentMatcher
{
    private readonly ConcurrentDictionary<Type, ITypeMatcher?> matchers = new();

    public bool AreMatchingTypes(Type left, Type right)
    {
        if (left == right)
        {
            return true;
        }

        var leftMatcher = this.matchers.GetOrAdd(left, GetMatcherForType);
        if (leftMatcher is not null && leftMatcher.Matches(right))
        {
            return true;
        }

        var rightMatcher = this.matchers.GetOrAdd(right, GetMatcherForType);
        if (rightMatcher is not null && rightMatcher.Matches(left))
        {
            return true;
        }

        return false;
    }

    private static ITypeMatcher? GetMatcherForType(Type type)
    {
        var attribute = type.GetCustomAttribute<WildcardTypeAttribute>();
        if (attribute is not null)
        {
            return (ITypeMatcher?)Activator.CreateInstance(attribute.MatcherType);
        }

        return null;
    }
}
