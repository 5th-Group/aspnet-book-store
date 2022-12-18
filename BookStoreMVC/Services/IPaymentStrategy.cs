namespace BookStoreMVC.Services;

public interface IPaymentStrategy
{
    string MakePayment<T>(T model) where T : IPaymentModel;
}