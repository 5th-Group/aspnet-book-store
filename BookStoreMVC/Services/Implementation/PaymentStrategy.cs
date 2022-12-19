namespace BookStoreMVC.Services.Implementation;

public class PaymentStrategy : IPaymentStrategy
{
    private readonly IEnumerable<IPaymentService> _paymentServices;

    public PaymentStrategy(IEnumerable<IPaymentService> paymentServices)
    {
        _paymentServices = paymentServices ?? throw new ArgumentNullException();
    }

    public string MakePayment<T>(T model) where T : IPaymentModel
    {
        return GetPaymentService(model).MakePayment(model);
    }

    private IPaymentService GetPaymentService<T>(T model) where T : IPaymentModel
    {
        var result = _paymentServices.FirstOrDefault(p => p.AppliesTo(model.GetType()));

        if (result is null)
            throw new InvalidOperationException($"Payment service for {model.GetType().ToString()} not registered");

        return result;
    }
}