namespace FakeItEasy
{
    using Core;

    internal interface IArgumentConstraintManagerFactory
    {
        INegatableArgumentConstraintManager<T> Create<T>();

        IAnyTypeNegatableArgumentConstraintManager<T> CreateAnyType<T>();
    }
}
