namespace FakeItEasy.Specs;

using System;
using Core;
using Xbehave;

public static class GenericCallMatchingSpecs
{
    public interface IFoo
    {
        void Bar<T>();

        void Bar<T>(T value);

        void Baz<T>() where T : struct, IFormattable;

        void Baz<T>(T value) where T : struct, IFormattable;
    }

    [Scenario]
    public static void CallToGenericMethodWithAnyTypeArgumentAndNoParameter(IFoo fake, Action callback)
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

    [Scenario]
    public static void CallToGenericMethodWithAnyTypeArgumentAndAParameter(IFoo fake, Action callback)
    {
        "Given a generic method that takes a parameter of the generic type parameter in an interface"
            .See<IFoo>(foo => foo.Bar<int>(0));

        "And a fake of this interface"
            .x(() => fake = A.Fake<IFoo>());

        "And a fake callback"
            .x(() => callback = A.Fake<Action>());

        "When the generic method is configured to invoke the callback for any type argument"
            .x(() => A.CallTo(() => fake.Bar(A<AnyType>.OfAnyType._)).Invokes(callback));

        "And the generic method is called with a specific type argument"
            .x(() => fake.Bar<string>("hello"));

        "Then the callback should be invoked"
            .x(() => A.CallTo(callback).MustHaveHappened());
    }

    [Scenario]
    public static void CallToGenericMethodWithCustomWildcardTypeArgumentAndNoParameter(IFoo fake, Action callback)
    {
        "Given a generic method that takes a parameter of the generic type parameter in an interface"
            .See<IFoo>(foo => foo.Baz<int>());

        "And a fake of this interface"
            .x(() => fake = A.Fake<IFoo>());

        "And a fake callback"
            .x(() => callback = A.Fake<Action>());

        "When the generic method is configured to invoke the callback for any type argument, using a custom wildcard type"
            .x(() => A.CallTo(() => fake.Baz<AnyTypeFormattableStruct>()).Invokes(callback));

        "And the generic method is called with a specific type argument"
            .x(() => fake.Baz<int>());

        "Then the callback should be invoked"
            .x(() => A.CallTo(callback).MustHaveHappened());
    }

    [Scenario]
    public static void CallToGenericMethodWithCustomWildcardTypeArgumentAndAParameter(IFoo fake, Action callback)
    {
        "Given a generic method in an interface"
            .See<IFoo>(foo => foo.Baz<int>(0));

        "And a fake of this interface"
            .x(() => fake = A.Fake<IFoo>());

        "And a fake callback"
            .x(() => callback = A.Fake<Action>());

        "When the generic method is configured to invoke the callback for any type argument, using a custom wildcard type"
            .x(() => A.CallTo(() => fake.Baz<AnyTypeFormattableStruct>(An<AnyTypeFormattableStruct>.OfAnyType._)).Invokes(callback));

        "And the generic method is called with a specific type argument"
            .x(() => fake.Baz<int>(42));

        "Then the callback should be invoked"
            .x(() => A.CallTo(callback).MustHaveHappened());
    }

    [WildcardType(typeof(AnyTypeMatcher))]
    private struct AnyTypeFormattableStruct : IFormattable
    {
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            throw new NotImplementedException();
        }
    }
}
