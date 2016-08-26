namespace GeekLearning.Email.InMemory
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class InMemoryEmailRepository : IInMemoryEmailRepository
    {
        private List<InMemoryEmail> innerEmailStore = new List<InMemoryEmail>();

        public IReadOnlyCollection<InMemoryEmail> Store { get; private set; }

        public InMemoryEmailRepository()
        {
            this.Store = new ReadOnlyCollection<InMemoryEmail>(innerEmailStore);
        }

        public void Save(InMemoryEmail email)
        {
            this.innerEmailStore.Add(email);
        }
    }
}
