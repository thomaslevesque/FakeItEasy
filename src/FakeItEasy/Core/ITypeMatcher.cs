namespace FakeItEasy.Core;

using System;

public interface ITypeMatcher
{
    bool Matches(Type type);
}
