namespace UnitTests.Person
{
    using BibleTraining.Api.Person;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CreateUpdatePersonIntegrityTests
    {
        private CreatePerson createPerson;
        private CreateUpdatePersonIntegrity validator;

        [TestInitialize]
        public void TestInitialize()
        {
            createPerson =  new CreatePerson
            {
                Resource = new PersonData
                {
                    FirstName = "a",
                    LastName  = "b",
                    Gender    = Gender.Female
                }
            };

            validator = new CreateUpdatePersonIntegrity();
        }

        [TestMethod]
        public void IsValid()
        {
            var result = validator.Validate(createPerson);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void MustHaveFirstName()
        {
            createPerson.Resource.FirstName = string.Empty;
            var result = validator.Validate(createPerson);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void MustHaveLastName()
        {
            createPerson.Resource.LastName = string.Empty;
            var result = validator.Validate(createPerson);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void MustHaveGender()
        {
            createPerson.Resource.Gender = null;
            var result = validator.Validate(createPerson);
            Assert.IsFalse(result.IsValid);
        }
    }
}