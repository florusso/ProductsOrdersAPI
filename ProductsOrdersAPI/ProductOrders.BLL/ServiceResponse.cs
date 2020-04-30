namespace ProductsOrders.BLL
{
    public class ServiceResponse<Tentity>
    {
        public bool IsSuccess { get; set; }
        public string ResponseMessage { get; set; }

        public Tentity ResponseObject { get; set; }
    }
}