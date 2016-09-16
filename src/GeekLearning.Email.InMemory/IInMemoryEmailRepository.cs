namespace GeekLearning.Email.InMemory
{
    using System.Collections.Generic;

    public interface IInMemoryEmailRepository
    {
        IReadOnlyCollection<InMemoryEmail> Store { get;}

        void Save(InMemoryEmail email);
    }
}
