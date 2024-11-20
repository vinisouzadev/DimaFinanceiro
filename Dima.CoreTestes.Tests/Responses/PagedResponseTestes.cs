using Bogus;
using Dima.Core.Common;
using Dima.Core.Responses;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Responses
{
    [Trait("Category", "Responses")]
    public class PagedResponseTestes
    {
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        public void Construtor_DataTotalCountCurrentPagePageSize_DadoApenasDataETotalCount_EntaoDeveSetarValoresDefaultCorretamente()
        {
            int expectedCurrentPageDefault = 1;
            int expectedPageSizeDefault = Configuration.DefaultPageSize;

            string anyDataValue = _faker.Lorem.Paragraph();
            int anyTotalCount = _faker.Random.Int(0);

            PagedResponse<string> pagedResponse = new(anyDataValue, totalCount: anyTotalCount);

            pagedResponse.CurrentPage.Should().Be(expectedCurrentPageDefault);
            pagedResponse.PageSize.Should().Be(expectedPageSizeDefault);
        }

        [Fact]
        public void Construtor_DataTotalCountCurrentPagePageSize_DadoTodosOsParametros_EntaoDeveSetarTodosOsValoresCorretamente()
        {
            string expectedDataValue = _faker.Lorem.Paragraph();
            int expectedTotalCount = _faker.Random.Int(0);
            int expectedPageSize = _faker.Random.Int(1);
            int expectedCurrentPage = _faker.Random.Int(1);

            PagedResponse<string> pagedResponse = new(expectedDataValue, expectedTotalCount,expectedCurrentPage, expectedPageSize);

            pagedResponse.Data.Should().Be(expectedDataValue);
            pagedResponse.TotalCount.Should().Be(expectedTotalCount);
            pagedResponse.PageSize.Should().Be(expectedPageSize);
            pagedResponse.CurrentPage.Should().Be(expectedCurrentPage);
        }

        [Fact]
        public void TotalPages_DadoValoresDeTotalCountEPageSize_EntaoDeveCalcularOTotalDePaginasCorretamente()
        {
            string anyDataValue = _faker.Lorem.Paragraph();
            int anyTotalCount = _faker.Random.Int(0);
            int anyPageSize = _faker.Random.Int(0);
            int expectedTotalPages = (int)Math.Ceiling(anyTotalCount / (double)anyPageSize);

            PagedResponse<string> pagedResponse = new(anyDataValue, anyTotalCount, pageSize: anyPageSize);

            pagedResponse.TotalPages.Should().Be(expectedTotalPages);
        }


        [Fact]
        public void Construtor_DataCodeMessage_DadoApenasData_EntaoDeveSetarValoresDefaultCorretamente()
        {
            int expectedCurrentPageDefault = 1;
            int expectedPageSizeDefault = Configuration.DefaultPageSize;

            string anyDataValue = _faker.Lorem.Paragraph();

            PagedResponse<string> pagedResponse = new(anyDataValue);

            pagedResponse.CurrentPage.Should().Be(expectedCurrentPageDefault);
            pagedResponse.PageSize.Should().Be(expectedPageSizeDefault);
            pagedResponse.Message.Should().BeNull();

        }
    }
}
