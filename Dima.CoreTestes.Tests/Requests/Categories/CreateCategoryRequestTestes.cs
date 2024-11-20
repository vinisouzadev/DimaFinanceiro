using Dima.Core.Requests.Categories;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Categories
{
    [Trait("Category", "Requests")]
    public class CreateCategoryRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            CreateCategoryRequest request = new();

            request.Title.Should().BeEmpty();
            request.Description.Should().BeEmpty();
        }
    }
}
