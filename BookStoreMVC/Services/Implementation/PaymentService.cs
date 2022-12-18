namespace BookStoreMVC.Services.Implementation;

public abstract class PaymentService<TModel> : IPaymentService where TModel : IPaymentModel
{
    public string MakePayment<T>(T model) where T : IPaymentModel
    {
        return MakePayment((TModel)(object)model);
    }

    public virtual bool AppliesTo(Type provider)
    {
        return typeof(TModel) == provider;
    }

    protected abstract string MakePayment(TModel model);
}