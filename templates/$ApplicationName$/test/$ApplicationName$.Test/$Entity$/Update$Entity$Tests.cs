namespace $ApplicationName$.Test.$Entity$
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Threading.Tasks;
    using FizzWare.NBuilder;
    using Rhino.Mocks;
    using Api.$Entity$;
    using Entities;
    using Infrastructure;
    
    [TestClass]
    public class Update$Entity$Tests : TestScenario
    {
        [TestMethod]
        public async Task ShouldUpdate$Entity$()
        {
            var $entityLowercase$= new $Entity$()
            {
                Id         = 1,
                Name       = "a",
                RowVersion = new byte[] { 0x01 }
            };

            var $entityLowercase$Data = Builder<$Entity$Data>.CreateNew()
                .With(c => c.Id = 1).And(c => c.RowVersion = new byte[] { 0x01 })
                .Build();

            _context.Expect(c => c.AsQueryable<$Entity$>())
                .Return(new[] { $entityLowercase$ }.AsQueryable().TestAsync());

            _context.Expect(c => c.CommitAsync())
                .WhenCalled(inv => $entityLowercase$.RowVersion = new byte[] { 0x02 })
                .Return(Task.FromResult(1));

            var result = await _mediator.SendAsync(new Update$Entity$($entityLowercase$Data));
            Assert.AreEqual(1, result.Id);
            CollectionAssert.AreEqual(new byte[] { 0x02 }, result.RowVersion);

            Assert.AreEqual($entityLowercase$Data.Name, $entityLowercase$.Name);

            _context.VerifyAllExpectations();
        }
    }
}