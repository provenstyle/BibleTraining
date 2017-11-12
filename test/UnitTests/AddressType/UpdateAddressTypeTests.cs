namespace UnitTests.AddressType
{
    using System.Linq;
    using System.Threading.Tasks;
    using BibleTraining.Api.AddressType;
    using BibleTraining.Entities;
    using FizzWare.NBuilder;
    using Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Mediate;
    using Rhino.Mocks;

    [TestClass]
    public class UpdateAddressTypeTests : TestScenario
    {
        [TestMethod]
        public async Task ShouldUpdateAddressType()
        {
            var addressType= new AddressType()
            {
                Id         = 1,
                Name       = "a",
                RowVersion = new byte[] { 0x01 }
            };

            var addressTypeData = Builder<AddressTypeData>.CreateNew()
                .With(c => c.Id = 1).And(c => c.RowVersion = new byte[] { 0x01 })
                .Build();

            _context.Expect(c => c.AsQueryable<AddressType>())
                .Return(new[] { addressType }.AsQueryable().TestAsync());

            _context.Expect(c => c.CommitAsync())
                .WhenCalled(inv => addressType.RowVersion = new byte[] { 0x02 })
                .Return(Task.FromResult(1));

            var result = await _handler.Send(new UpdateAddressType(addressTypeData));
            Assert.AreEqual(1, result.Id);
            CollectionAssert.AreEqual(new byte[] { 0x02 }, result.RowVersion);

            Assert.AreEqual(addressTypeData.Name, addressType.Name);

            _context.VerifyAllExpectations();
        }
    }
}