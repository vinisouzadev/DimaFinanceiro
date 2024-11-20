using Dima.Core.Requests.Categories;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Categories
{
    [Trait("Category", "Requests")]
    public class UpdateCategoryRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            UpdateCategoryRequest request = new();

            request.Title.Should().BeEmpty();
        }
    }
}
