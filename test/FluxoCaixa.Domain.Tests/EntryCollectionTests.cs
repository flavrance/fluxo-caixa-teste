namespace FluxoCaixa.Domain.Tests
{
    using Xunit;
    using FluxoCaixa.Domain.CashFlows;
    using System;
    using System.Collections.Generic;

    public class EntryCollectionTests
    {
        [Fact]
        public void Multiple_Entries_Should_Be_Added()
        {
            EntryCollection entryCollection = new EntryCollection();
            entryCollection.Add(new List<IEntry>()
            {
                new Credit(Guid.Empty, 100),
                new Debit(Guid.Empty, 30)
            });

            Assert.Equal(2, entryCollection.GetEntries().Count);
        }
    }
}
