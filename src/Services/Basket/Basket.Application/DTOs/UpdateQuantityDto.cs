namespace Basket.Application.DTOs
{
    public class UpdateQuantityDto
    {
        public string BasketItemId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
} 