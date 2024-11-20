using Bogus;
using Dima.Core.Models.Reports;
using FluentAssertions;
using System.Numerics;

namespace Dima.CoreTestes.Tests.Models.Reports
{
    [Trait("Category", "Models")]
    public class FinancialSummaryTestes
    {
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        public void Total_DadoExpensesMenorQueZero_EntaoDeveInverterValorDeExpensesDuranteCalculo()
        {

            string anyUserId = _faker.Person.UserName;
            decimal anyIncomesValue = _faker.Random.Decimal(0m);
            decimal anyNegativeExpenseValue = _faker.Random.Decimal(-10000m, -0.1m);
            decimal expectedTotal = anyIncomesValue - (anyNegativeExpenseValue * -1);

            FinancialSummary financialSummary = new(anyUserId, anyIncomesValue, anyNegativeExpenseValue);

            financialSummary.Total.Should().Be(expectedTotal);
        }

        [Fact]
        public void Total_DadoExpensesMaiorOuIgualAZero_EntaoDeveCalcularValorTotalCorretamente()
        {
            string anyUserId = _faker.Person.UserName;
            decimal anyIncomesValue = _faker.Random.Decimal(0m);
            decimal anyPositiveExpenseValue = _faker.Random.Decimal(0m);
            decimal expectedTotal = anyIncomesValue - anyPositiveExpenseValue;

            FinancialSummary financialSummary = new(anyUserId, anyIncomesValue, anyPositiveExpenseValue);

            financialSummary.Total.Should().Be(expectedTotal);
        }
    }
}
