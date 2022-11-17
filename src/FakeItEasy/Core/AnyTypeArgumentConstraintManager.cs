namespace FakeItEasy.Core;

using System;
using System.Diagnostics.CodeAnalysis;

public interface IAnyTypeArgumentConstraintManager<T> : IHideObjectMembers
{
    T Matches(Func<object, bool> predicate, Action<IOutputWriter> descriptionWriter);
}

public interface IAnyTypeNegatableArgumentConstraintManager<T> : IAnyTypeArgumentConstraintManager<T>
{
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Not", Justification = "Part of the fluent syntax.")]
    IAnyTypeArgumentConstraintManager<T> Not { get; }
}

internal class AnyTypeArgumentConstraintManager<T>
    : IAnyTypeNegatableArgumentConstraintManager<T>
{
    private readonly Action<IArgumentConstraint> onConstraintCreated;

    public AnyTypeArgumentConstraintManager(Action<IArgumentConstraint> onConstraintCreated)
    {
        this.onConstraintCreated = onConstraintCreated;
    }

    public IAnyTypeArgumentConstraintManager<T> Not => new NotArgumentConstraintManager(this);

    public T Matches(Func<object, bool> predicate, Action<IOutputWriter> descriptionWriter)
    {
        this.onConstraintCreated(new MatchesConstraint(predicate, descriptionWriter));
        return default!;
    }

    private class NotArgumentConstraintManager : IAnyTypeArgumentConstraintManager<T>
    {
        private readonly IAnyTypeArgumentConstraintManager<T> parent;

        public NotArgumentConstraintManager(IAnyTypeArgumentConstraintManager<T> parent)
        {
            this.parent = parent;
        }

        public T Matches(Func<object, bool> predicate, Action<IOutputWriter> descriptionWriter)
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
        private readonly Func<object, bool> predicate;
        private readonly Action<IOutputWriter> descriptionWriter;

        public MatchesConstraint(Func<object, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            this.predicate = predicate;
            this.descriptionWriter = descriptionWriter;
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
            try
            {
                return this.predicate.Invoke(argument!);
            }
            catch (Exception ex)
            {
                throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException($"Argument matcher {this.GetDescription()}"), ex);
            }
        }

        private string GetDescription()
        {
            var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();
            ((IArgumentConstraint)this).WriteDescription(writer);
            return writer.Builder.ToString();
        }
    }
}
