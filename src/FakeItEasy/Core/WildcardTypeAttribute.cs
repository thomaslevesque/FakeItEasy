namespace FakeItEasy.Core;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public sealed class WildcardTypeAttribute : Attribute
{
    public WildcardTypeAttribute(Type matcherType)
    {
        if (!typeof(ITypeMatcher).IsAssignableFrom(matcherType))
        {
            throw new ArgumentException($"The specified type must implement {nameof(ITypeMatcher)}");
        }

        this.MatcherType = matcherType;
    }

    public Type MatcherType { get; }
}
