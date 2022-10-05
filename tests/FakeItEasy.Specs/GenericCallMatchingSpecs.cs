namespace FakeItEasy.Specs;

using System;
using Xbehave;

public static class GenericCallMatchingSpecs
{
    public interface IFoo
    {
        void Bar<T>();
    }

    [Scenario]
    public static void CallToGenericMethodWithAnyTypeArgument(IFoo fake, Action callback)
    {
        "Given a generic method in an interface"
            .See<IFoo>(foo => foo.Bar<int>());

        "And a fake of this interface"
            .x(() => fake = A.Fake<IFoo>());

        "And a fake callback"
            .x(() => callback = A.Fake<Action>());

        "When the generic method is configured to invoke the callback for any type argument"
            .x(() => A.CallTo(() => fake.Bar<AnyType>()).Invokes(callback));

        "And the generic method is called with a specific type argument"
            .x(() => fake.Bar<string>());

        "Then the callback should be invoked"
            .x(() => A.CallTo(callback).MustHaveHappened());
    }
}
