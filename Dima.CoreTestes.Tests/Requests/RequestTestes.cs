using Dima.Core.Requests;
using Dima.Core.Requests.Categories;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests
{
    [Trait("Category", "Requests")]
    public class RequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {   
            //  CreateCategoryRequest está representando qualquer classe que herde
            // de Request
            Request request = new CreateCategoryRequest();

            request.UserId.Should().BeEmpty();
        }
    }
}
