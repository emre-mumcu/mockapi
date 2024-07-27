using Microsoft.AspNetCore.Mvc;

namespace MockAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FakerController : ControllerBase
    {
        [HttpGet("[action]")]
        public IActionResult Address()
        {
            var data = new
            {
                CaProvince = Faker.Address.CaProvince(),
                City = Faker.Address.City(),
                CityPrefix = Faker.Address.CityPrefix(),
                CitySuffix = Faker.Address.CitySuffix(),
                Country = Faker.Address.Country(),
                SecondaryAddress = Faker.Address.SecondaryAddress(),
                StreetAddress = Faker.Address.StreetAddress(),
                StreetName = Faker.Address.StreetName(),
                StreetSuffix = Faker.Address.StreetSuffix(),
                UkCountry = Faker.Address.UkCountry(),
                UkCounty = Faker.Address.UkCounty(),
                UkPostCode = Faker.Address.UkPostCode(),
                UsMilitaryState = Faker.Address.UsMilitaryState(),
                UsMilitaryStateAbbr = Faker.Address.UsMilitaryStateAbbr(),
                UsState = Faker.Address.UsState(),
                UsStateAbbr = Faker.Address.UsStateAbbr(),
                UsTerritory = Faker.Address.UsTerritory(),
                UsTerritoryStateAbbr = Faker.Address.UsTerritoryStateAbbr(),
                ZipCode = Faker.Address.ZipCode(),
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Boolean()
        {
            var data = new
            {
                CaProvince = Faker.Boolean.Random()
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Company()
        {
            var data = new
            {
                BS = Faker.Company.BS(),
                CatchPhrase = Faker.Company.CatchPhrase(),
                Name = Faker.Company.Name(),
                Suffix = Faker.Company.Suffix(),
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Country()
        {
            var data = new
            {
                Name = Faker.Country.Name(),
                TwoLetterCode = Faker.Country.TwoLetterCode(),
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Currency()
        {
            var data = new
            {
                Name = Faker.Currency.Name(),
                ThreeLetterCode = Faker.Currency.ThreeLetterCode(),
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Finance()
        {
            var data = new
            {
                Coupon = Faker.Finance.Coupon(),
                BondName = Faker.Finance.Credit.BondName(),
                BondClass = Faker.Finance.Credit.BondClass(),
                Isin = Faker.Finance.Isin(),
                Maturity = Faker.Finance.Maturity(),
                Ticker = Faker.Finance.Ticker(),
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Identification()
        {
            var data = new
            {
                BulgarianPin = Faker.Identification.BulgarianPin(),
                DateOfBirth = Faker.Identification.DateOfBirth(),
                MedicareBeneficiaryIdentifier = Faker.Identification.MedicareBeneficiaryIdentifier(),
                SocialSecurityNumber = Faker.Identification.SocialSecurityNumber(),
                UkNationalInsuranceNumber = Faker.Identification.UkNationalInsuranceNumber(),
                UkNhsNumber = Faker.Identification.UkNhsNumber(),
                UkPassportNumber = Faker.Identification.UkPassportNumber(),
                UsPassportNumber = Faker.Identification.UsPassportNumber(),
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Internet()
        {
            var data = new
            {
                DomainName = Faker.Internet.DomainName(),
                DomainSuffix = Faker.Internet.DomainSuffix(),
                DomainWord = Faker.Internet.DomainWord(),
                Email = Faker.Internet.Email(),
                FreeEmail = Faker.Internet.FreeEmail(),
                SecureUrl = Faker.Internet.SecureUrl(),
                Url = Faker.Internet.Url(),
                UserName = Faker.Internet.UserName(),
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Lorem()
        {
            var data = new
            {
                Sentence = Faker.Lorem.Sentence(),
                Sentences = Faker.Lorem.Sentences(10),
                Words = Faker.Lorem.Words(10),
                GetFirstWord = Faker.Lorem.GetFirstWord(),
                Paragraph = Faker.Lorem.Paragraph(),
                Paragraphs = Faker.Lorem.Paragraphs(10),
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Name()
        {
            var data = new {
                Suffix = Faker.Name.Suffix(),
                Name = Faker.Name.First(),
                Middle = Faker.Name.Middle(),
                Last = Faker.Name.Last(),
                FullName = Faker.Name.FullName()
            };            

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult Phone()
        {
            var data = new
            {
                Number = Faker.Phone.Number()
            };

            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult RandomNumber()
        {
            var data = new
            {
                Next = Faker.RandomNumber.Next(),
            };

            return Ok(data);
        }
    }
}