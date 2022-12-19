namespace BookStoreMVC.Services;

public interface IPaymentService
{
    string MakePayment<T>(T model) where T : IPaymentModel;

    bool AppliesTo(Type provider);
}