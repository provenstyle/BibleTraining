namespace BibleTraining.Api.Email
{
    using EmailType;
    using Improving.MediatR;

    public class EmailData : Resource<int?>
    {
        public string        Address      { get; set; }
        public int?          PersonId     { get; set; }

        public EmailTypeData EmailType    { get; set; }
        public int?          EmailTypeId  { get; set; }
    }
}