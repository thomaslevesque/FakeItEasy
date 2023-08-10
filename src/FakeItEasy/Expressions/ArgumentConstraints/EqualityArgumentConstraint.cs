namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    internal class EqualityArgumentConstraint
        : IArgumentConstraint
    {
        private readonly object expectedValue;
        private readonly Type parameterType;

        public EqualityArgumentConstraint(object expectedValue, Type parameterType)
        {
            this.expectedValue = expectedValue;
            this.parameterType = parameterType;
        }

        public string ConstraintDescription => this.ToString();

        public bool IsValid(object? argument)
        {
            if (argument is null)
            {
                return false;
            }

            var argumentEqualityComparer = ServiceLocator.Resolve<ArgumentEqualityComparer>();
            return argumentEqualityComparer.AreEqual(this.expectedValue, argument, this.parameterType);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any type of exception may be encountered.")]
        public override string ToString()
        {
            try
            {
                var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();
                writer.WriteArgumentValue(this.expectedValue);
                return writer.Builder.ToString();
            }
            catch (Exception ex) when (ex is not UserCallbackException)
            {
                return Fake.TryGetFakeManager(this.expectedValue, out var manager)
                    ? manager.FakeObjectDisplayName
                    : this.expectedValue.GetType().ToString();
            }
        }

        public void WriteDescription(IOutputWriter writer)
        {
            writer.Write(this.ConstraintDescription);
        }
    }
}
