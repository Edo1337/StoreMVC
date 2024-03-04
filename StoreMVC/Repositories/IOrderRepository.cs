namespace StoreMVC.Repositories
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> GetAllOrders();

    }
}