using Bogus;
using Dima.API.Data;
using Dima.API.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Dima.APITestes.Tests.Handlers
{
    [Trait("Category", "Handlers")]
    public class CategoryHandlerTestes
    {
        private readonly Faker _faker = new("pt_BR");

        private readonly AppDbContext _context;

        private readonly CategoryHandler _categoryHandler; 

        public CategoryHandlerTestes()
        {
            DbContextOptionsBuilder<AppDbContext> dbContextOptions = new();
            dbContextOptions.UseInMemoryDatabase("categoryHandler");

            _context = new(dbContextOptions.Options);

            _categoryHandler = new(_context);
        }

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_DadoValoresDeRequestCorretos_EntaoDeveRetornarUmResponseDeSucesso()
        {
            string correctlyUserId = _faker.Person.FirstName;
            string correctlyTitle = _faker.Lorem.Paragraph();
            string correctlyDescription = _faker.Lorem.Paragraph();

            CreateCategoryRequest request = new()
            {
                Title = correctlyTitle,
                Description = correctlyDescription,
                UserId = correctlyUserId
            };

            var result = await _categoryHandler.CreateAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.UserId.Should().Be(correctlyUserId);
            result.Data.Title.Should().Be(correctlyTitle);
            result.Data.Description.Should().Be(correctlyDescription);
        }

        [Fact]
        public async Task CreateAsync_DadoRequestCorreto_EntaoDeveSalvarCorretamenteNoBanco() 
        {
            string correctlyUserId = _faker.Person.FirstName;
            string correctlyTitle = _faker.Lorem.Paragraph();
            string correctlyDescription = _faker.Lorem.Paragraph();

            CreateCategoryRequest request = new()
            {
                Title = correctlyTitle,
                Description = correctlyDescription,
                UserId = correctlyUserId
            };

            var result = await _categoryHandler.CreateAsync(request);

            Category? createdCategoy = _context.Categories.SingleOrDefault(x => x.Id == result.Data!.Id && x.UserId == correctlyUserId);

            createdCategoy.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_DadoRequestComPropriedadesNulas_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            CreateCategoryRequest request = new();
            request.Title = null;
            request.Description = null;
            request.UserId = null;

            var result = await _categoryHandler.CreateAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível criar a sua categoria");

        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_DadoIdIncorretoNoRequest_EntaoDeveRetornarRespostaDeFalha()
        {
            string anyTitle = _faker.Person.FirstName;
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.FirstName;
            Category category = new Category 
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            long incorrectlyId = _faker.Random.Long(-10, -1);
            DeleteCategoryRequest request = new()
            {
                Id = incorrectlyId,
                UserId = anyUserId
            };

            var result = await _categoryHandler.DeleteAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa categoria");
        }

        [Fact]
        public async Task DeleteAsync_DadoUserIdIncorretoNoRequest_EntaoDeveRetornarRespostaDeFalha()
        {
            string anyTitle = _faker.Person.FirstName;
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.FirstName;

            Category category = new()
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            string incorrectlyUserId = _faker.Random.String(10);
            long correctlyId = category.Id;
            DeleteCategoryRequest request = new()
            {
                Id = correctlyId,
                UserId = incorrectlyUserId
            };

            var result = await _categoryHandler.DeleteAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa categoria");
        }

        [Fact]
        public async Task DeleteAsync_DadoUserIdEIrCorretosNoRequest_EntaoDeveApagarCategoriaCorretamente()
        {
            string anyTitle = _faker.Lorem.Paragraph();
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.UserName;
            Category category = new()
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            DeleteCategoryRequest request = new()
            {
                Id = category.Id,
                UserId = anyUserId
            };

            var result = await _categoryHandler.DeleteAsync(request);

            var deletedCategory = _context.Categories.SingleOrDefault(x => x.Id == category.Id);

            deletedCategory.Should().BeNull();
        }

        #endregion

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_DadoUmaListaDeCategoriasNoBancoDeUmUsuario_EntaoDeveRetornarTodasCategoriasCadastradasDesseUsuarioCorretamente()
        {
            string firstUserId = _faker.Person.UserName;
            string secondUserId = _faker.Person.FirstName;

            string firstCategoryTitle = _faker.Lorem.Paragraph();
            string firstCategoryDescription = _faker.Lorem.Paragraph();
            Category firstCategory = new()
            {
                Title = firstCategoryTitle,
                Description = firstCategoryDescription,
                UserId = firstUserId
            };

            string secondCategoryTitle = _faker.Lorem.Paragraph();
            string secondCategoryDescription = _faker.Lorem.Paragraph();
            Category secondCategory = new()
            {
                Title = secondCategoryTitle,
                Description = secondCategoryDescription,
                UserId = firstUserId
            };

            string tertiaryCategoryTitle = _faker.Lorem.Paragraph();
            string tertiaryCategoryDescription = _faker.Lorem.Paragraph();
            Category tertiaryCategory = new()
            {
                Title = tertiaryCategoryTitle,
                Description = tertiaryCategoryDescription,
                UserId = secondUserId
            };

            List<Category> categories = [];
            categories.Add(firstCategory);
            categories.Add(secondCategory);
            categories.Add(tertiaryCategory);

            _context.AddRange(categories);
            _context.SaveChanges();

            GetAllCategoryRequest request = new()
            {
                UserId = firstUserId
            };

            var result = await _categoryHandler.GetAllAsync(request);

            result.Data.Should().NotBeNull();
            result.Data!.Any(x => x.UserId == firstUserId).Should().BeTrue();
            result.Data!.Any(x => x.UserId == secondUserId).Should().BeFalse();
            result.IsSuccess.Should().BeTrue();
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_DadoIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyTitle = _faker.Person.FirstName;
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.FirstName;
            Category category = new Category
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            long incorrectlyId = _faker.Random.Long(-10, -1);
            GetByIdCategoryRequest request = new()
            {
                Id = incorrectlyId,
                UserId = anyUserId
            };

            var result = await _categoryHandler.GetByIdAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa categoria");
        }

        [Fact]
        public async Task GetByIdAsync_DadoUserIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyTitle = _faker.Person.FirstName;
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.FirstName;

            Category category = new()
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            string incorrectlyUserId = _faker.Random.String(10);
            long correctlyId = category.Id;
            GetByIdCategoryRequest request = new()
            {
                Id = correctlyId,
                UserId = incorrectlyUserId
            };

            var result = await _categoryHandler.GetByIdAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa categoria");
        }

        [Fact]
        public async Task GetByIdAsync_DadoUserIdEIdCorretos_EntaoDeveRetornarACategoriaCorreta()
        {
            string anyTitle = _faker.Lorem.Paragraph();
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.UserName;
            Category category = new()
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            long correctlyId = category.Id;
            GetByIdCategoryRequest request = new()
            {
                Id = correctlyId,
                UserId = anyUserId
            };

            var result = await _categoryHandler.GetByIdAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be(anyTitle);
            result.Data.Description.Should().Be(anyDescription);

        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_DadoIdIncorretoNoRequest_EntaoDeveRetornarRespostaDeFalha()
        {
            string anyTitle = _faker.Person.FirstName;
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.FirstName;
            Category category = new Category
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            long incorrectlyId = _faker.Random.Long(-10, -1);
            UpdateCategoryRequest request = new()
            {
                Id = incorrectlyId,
                UserId = anyUserId
            };

            var result = await _categoryHandler.UpdateAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa categoria");
        }

        [Fact]
        public async Task UpdateAsync_DadoUserIdIncorretoNoRequest_EntaoDeveRetornarRespostaDeFalha()
        {
            string anyTitle = _faker.Person.FirstName;
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.FirstName;

            Category category = new()
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            string incorrectlyUserId = _faker.Random.String(10);
            long correctlyId = category.Id;
            UpdateCategoryRequest request = new()
            {
                Id = correctlyId,
                UserId = incorrectlyUserId
            };

            var result = await _categoryHandler.UpdateAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa categoria");
        }

        [Fact]
        public async Task UpdateAsync_DadoPropriedadesCorretas_EntaoDeveAtualizarOsDadosCorretamente()
        {
            string anyTitle = _faker.Person.FirstName;
            string anyDescription = _faker.Lorem.Paragraph();
            string anyUserId = _faker.Person.FirstName;

            Category category = new()
            {
                Title = anyTitle,
                Description = anyDescription,
                UserId = anyUserId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            string newTitle = _faker.Address.Direction();
            string newDescription = _faker.Lorem.Paragraph();

            UpdateCategoryRequest request = new()
            {
                Title = newTitle,
                Description = newDescription,
                UserId = anyUserId,
                Id = category.Id
            };

            var result = await _categoryHandler.UpdateAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be(newTitle);
            result.Data.Description.Should().Be(newDescription);

        }

        #endregion
    }
}
