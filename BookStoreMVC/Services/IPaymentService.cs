namespace BookStoreMVC.Services;

public interface IPaymentService
{
    void MakePayment<T>(T model) where T : IPaymentModel;

    bool AppliesTo(Type provider);
}