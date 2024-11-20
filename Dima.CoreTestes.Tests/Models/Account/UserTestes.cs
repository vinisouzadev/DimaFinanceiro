using Dima.Core.Models.Account;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Models.Account
{
    [Trait("Category", "Models")]
    public class UserTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarDefaultNasPropriedadesCorretamente()
        {
            User user = new();

            user.Email.Should().Be(string.Empty);
            user.Claims.Should().Equal([]);
        }
    }
}
