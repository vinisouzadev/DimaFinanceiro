using Bogus;
using Dima.Core.Common.Extensions;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Common.Extensions
{
    [Trait("Category", "Extensions")]
    public class DateTimeExtensionsTestes
    {
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        public void GetFirstDay_DadoApenasParametroDateCorretamente_EntaoDeveRetornarOPrimeiroDiaDoMes()
        {
            DateTime correctlyDateTime = _faker.Date.Recent();

            DateTime correctlyDateTimeWithFirstDay = DateTimeExtension.GetFirstDay(correctlyDateTime);

            correctlyDateTimeWithFirstDay.Day.Should().Be(1);
            correctlyDateTimeWithFirstDay.Month.Should().Be(correctlyDateTime.Month);
            correctlyDateTimeWithFirstDay.Year.Should().Be(correctlyDateTime.Year);
            correctlyDateTimeWithFirstDay.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void GetFirstDay_DadoParametroDeAnoEMesCorretamente_EntaoDeveRetornarOPrimeiroDiaDoMes()
        {
            int correctlyYear = _faker.Date.Between(new DateTime(1900,1,30), new DateTime(2024,12,31)).Year;
            int correctlyMonth = _faker.Date.Between(new DateTime(1900, 1, 30), new DateTime(2024, 12, 31)).Month;
            DateTime expectedDateTime = new DateTime(correctlyYear, correctlyMonth, 1, 0, 0, 0, 0, 0, DateTimeKind.Utc);
            
            DateTime correctlyDateTimeWithFirstDay = DateTimeExtension.GetFirstDay(DateTime.Now, correctlyYear, correctlyMonth);

            correctlyDateTimeWithFirstDay.Should().Be(expectedDateTime);
        }

        [Fact]
        public void GetLastDay_DadoParametroDateCorretamente_EntaoDeveRetornarOUltimoDiaDoMes()
        {
            DateTime correctlyDateTime = _faker.Date.Recent();
            DateTime expectedDateTime = new DateTime(correctlyDateTime.Year, correctlyDateTime.Month, 1).AddMonths(1).AddDays(-1);

            DateTime correctlyDateTimeWithLastDay = DateTimeExtension.GetLastDay(correctlyDateTime);

            correctlyDateTimeWithLastDay.Should().Be(expectedDateTime);
            correctlyDateTimeWithLastDay.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void GetLastDay_DadoParametroDeAnoEMesCorretamente_EntaoDeveRetornarOUltimoDiaDoMes()
        {
            int correctlyYear = _faker.Date.Between(new DateTime(1900, 1, 30), new DateTime(2024, 12, 31)).Year;
            int correctlyMonth = _faker.Date.Between(new DateTime(1900, 1, 30), new DateTime(2024, 12, 31)).Month;
            DateTime expectedDateTime = new DateTime(correctlyYear, correctlyMonth, 1,0,0,0,0,0,DateTimeKind.Utc).AddMonths(1).AddDays(-1);
            
            DateTime dateTimeWithLastDayOfMonth = DateTimeExtension.GetLastDay(DateTime.Now, correctlyYear, correctlyMonth);

            dateTimeWithLastDayOfMonth.Should().Be(expectedDateTime);
        }
   
    }
}
