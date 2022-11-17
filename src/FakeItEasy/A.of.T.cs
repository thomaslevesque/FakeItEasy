namespace FakeItEasy
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Core;

    /// <summary>
    /// Provides an API entry point for constraining arguments of fake object calls.
    /// </summary>
    /// <typeparam name="T">The type of argument to validate.</typeparam>
    /// <remarks>If desired, <see cref="An{T}"/> may be used to specify constraints on types whose names begin with a vowel sound.</remarks>
    public static class A<T>
    {
        /// <summary>
        /// Gets an argument constraint object that will be used to constrain a method call argument.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is a special case where the type parameter acts as an entry point into the fluent api.")]
        public static INegatableArgumentConstraintManager<T> That =>
            ServiceLocator.Resolve<IArgumentConstraintManagerFactory>().Create<T>();

        /// <summary>
        /// Gets a constraint that considers any value of an argument as valid.
        /// </summary>
        /// <remarks>This is a shortcut for the "Ignored"-property.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is a special case where the type parameter acts as an entry point into the fluent api.")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "But it's kinda cool right?")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [CLSCompliant(false)]
#pragma warning disable SA1300 // Element must begin with upper-case letter
        public static T _ => Ignored;
#pragma warning restore SA1300 // Element must begin with upper-case letter

        /// <summary>
        /// Gets a constraint that considers any value of an argument as valid.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is a special case where the type parameter acts as an entry point into the fluent api.")]
        public static T Ignored
        {
            get { return That.Matches(x => true, x => x.Write(nameof(Ignored))); }
        }

        public static class OfAnyType
        {
            [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is a special case where the type parameter acts as an entry point into the fluent api.")]
            public static IAnyTypeNegatableArgumentConstraintManager<T> That =>
                ServiceLocator.Resolve<IArgumentConstraintManagerFactory>().CreateAnyType<T>();

            [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is a special case where the type parameter acts as an entry point into the fluent api.")]
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "But it's kinda cool right?")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            [CLSCompliant(false)]
#pragma warning disable SA1300 // Element must begin with upper-case letter
            public static T _ => Ignored;
#pragma warning restore SA1300 // Element must begin with upper-case letter

            [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is a special case where the type parameter acts as an entry point into the fluent api.")]
            public static T Ignored => That.Matches(x => true, x => x.Write(nameof(Ignored)));
        }
    }
}
