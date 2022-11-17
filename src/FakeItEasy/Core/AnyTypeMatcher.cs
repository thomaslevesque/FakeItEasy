namespace FakeItEasy.Core;

using System;

public class AnyTypeMatcher : ITypeMatcher
{
    public bool Matches(Type type) => true;
}
