namespace FakeItEasy.Core
{
    using System;

    internal class DefaultArgumentConstraintManager<T>
        : INegatableArgumentConstraintManager<T>
    {
        private readonly Action<IArgumentConstraint> onConstraintCreated;
        private readonly GenericTypeArgumentMatcher genericTypeArgumentMatcher;

        public DefaultArgumentConstraintManager(Action<IArgumentConstraint> onConstraintCreated, GenericTypeArgumentMatcher genericTypeArgumentMatcher)
        {
            this.onConstraintCreated = onConstraintCreated;
            this.genericTypeArgumentMatcher = genericTypeArgumentMatcher;
        }

        public IArgumentConstraintManager<T> Not => new NotArgumentConstraintManager(this);

        public T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            this.onConstraintCreated(new MatchesConstraint(predicate, descriptionWriter, this.genericTypeArgumentMatcher));
            return default!;
        }

        private class NotArgumentConstraintManager
            : IArgumentConstraintManager<T>
        {
            private readonly IArgumentConstraintManager<T> parent;

            public NotArgumentConstraintManager(IArgumentConstraintManager<T> parent)
            {
                this.parent = parent;
            }

            public T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
            {
                return this.parent.Matches(
                    x => !predicate(x),
                    x =>
                    {
                        x.Write("not ");
                        descriptionWriter.Invoke(x);
                    });
            }
        }

        private class MatchesConstraint
            : ITypedArgumentConstraint
        {
            private static readonly bool IsNullable = typeof(T).IsNullable();

            private readonly Func<T, bool> predicate;
            private readonly Action<IOutputWriter> descriptionWriter;
            private readonly GenericTypeArgumentMatcher genericTypeArgumentMatcher;

            public MatchesConstraint(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter, GenericTypeArgumentMatcher genericTypeArgumentMatcher)
            {
                this.predicate = predicate;
                this.descriptionWriter = descriptionWriter;
                this.genericTypeArgumentMatcher = genericTypeArgumentMatcher;
            }

            public Type Type => typeof(T);

            public override string ToString() => this.GetDescription();

            void IArgumentConstraint.WriteDescription(IOutputWriter writer)
            {
                writer.Write("<");
                try
                {
                    this.descriptionWriter.Invoke(writer);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException("Argument matcher description"), ex);
                }

                writer.Write(">");
            }

            bool IArgumentConstraint.IsValid(object? argument)
            {
                if (!this.IsValueValidForType(argument))
                {
                    return false;
                }

                try
                {
                    return this.predicate.Invoke((T)argument!);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException($"Argument matcher {this.GetDescription()}"), ex);
                }
            }

            private bool IsValueValidForType(object? argument)
            {
                if (argument is null)
                {
                    return IsNullable;
                }

                return argument is T || this.genericTypeArgumentMatcher.AreMatchingTypes(argument.GetType(), typeof(T));
            }

            private string GetDescription()
            {
                var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();
                ((IArgumentConstraint)this).WriteDescription(writer);
                return writer.Builder.ToString();
            }
        }
    }
}
