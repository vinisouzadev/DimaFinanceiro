using Dima.Core.Requests;
using Dima.Core.Requests.Categories;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests
{
    [Trait("Category", "Requests")]
    public class PagedRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {   
            //  GetAllCategoryRequest está representando qualquer classe que herde
            // de PagedRequest 
            PagedRequest request = new GetAllCategoryRequest();

            request.PageNumber.Should().Be(1);
            request.PageSize.Should().Be(25);
        }
    }
}
