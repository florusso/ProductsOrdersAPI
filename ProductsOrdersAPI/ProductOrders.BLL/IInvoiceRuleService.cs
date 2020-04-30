namespace ProductsOrders.BLL
{
    public interface IInvoiceRuleService
    {
        double Apply(int CustomerCode, double summation);
    }
}