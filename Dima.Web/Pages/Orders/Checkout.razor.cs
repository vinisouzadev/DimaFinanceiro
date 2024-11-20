using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders
{
    public partial class CheckoutPageCode : ComponentBase
    {
        #region Paranetros

        [Parameter]
        public string ProductSlug { get; set; } = string.Empty;

        [SupplyParameterFromQuery(Name = "voucher")]
        public string? VoucherCode { get; set; }

        #endregion

        #region Propriedades

        public bool IsBusy { get; set; } = false;

        public bool IsValid { get; set; }

        public Product? Product { get; set; }

        public Voucher? Voucher { get; set; }

        public decimal Total { get; set; }

        public CreateOrderRequest InputModel { get; set; } = new();

        #endregion

        #region Dependências

        [Inject]
        public IProductHandler ProductHandler { get; set; } = null!;

        [Inject]
        public IOrderHandler OrderHandler { get; set; } = null!;

        [Inject]
        public IVoucherHandler VoucherHandler { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ProductHandler.GetProductBySlugAsync(new GetProductBySlugRequest { Slug = ProductSlug });

                if (!result.IsSuccess)
                {
                    Snackbar.Add("Não foi possível identificar o produto");
                    IsValid = false;
                    return;
                }

                Product = result.Data;

            }
            catch
            {
                Snackbar.Add("Não foi possível identificar o produto");
                IsValid = false;
                return;
            }

            if (Product is null)
            {
                Snackbar.Add("Não foi possível identificar o produto");
                IsValid = false;
                return;
            }

            if (!string.IsNullOrEmpty(VoucherCode))
            {
                try
                {
                    var result = await VoucherHandler.GetVoucherByCodeAsync(new GetVoucherByCodeRequest { Code = VoucherCode.Replace("-", "") });

                    if (!result.IsSuccess)
                    {
                        VoucherCode = string.Empty;
                        Snackbar.Add("Não foi possível identificar seu voucher", Severity.Error);
                    }

                    if (result.Data is null)
                    {
                        VoucherCode = string.Empty;
                        Snackbar.Add("Não foi possível identificar seu voucher", Severity.Error);
                    }

                    Voucher = result.Data;
                }
                catch
                {
                    VoucherCode = string.Empty;
                    Snackbar.Add("Não foi possível identificar seu voucher", Severity.Error);
                }

            }

            IsValid = true;

            Total = Product.Price - (Voucher?.Amount ?? 0);
        }

        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;

            try
            {
                InputModel = new CreateOrderRequest
                {
                    ProductId = Product!.Id,
                    VoucherId = Voucher?.Id
                };

                var result = await OrderHandler.CreateOrderAsync(InputModel);

                if (!result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Error);
                }
                else
                {
                    NavigationManager.NavigateTo($"/pedidos/{result.Data!.OrderCode}");
                }
            }
            catch(Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
