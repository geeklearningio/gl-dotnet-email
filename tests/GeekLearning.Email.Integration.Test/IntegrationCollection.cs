namespace GeekLearning.Email.Integration.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    [CollectionDefinition(nameof(IntegrationCollection))]
    public class IntegrationCollection: ICollectionFixture<StoresFixture>
    {

    }
}
