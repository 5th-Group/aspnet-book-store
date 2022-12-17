namespace BookStoreMVC.Services;

public interface IPaymentStrategy
{
    void MakePayment<T>(T model) where T : IPaymentModel;
}