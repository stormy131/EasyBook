using System.ComponentModel.DataAnnotations;

namespace EasyBook.Models;

public class OrderDTO{
    public long Id { get; set; }
    public long UserId { get; set; }

    public ICollection<OrderItemDTO> OrderedItems { get; set; }

    public static explicit operator OrderDTO(Order order){
        return new OrderDTO{
            Id = order.Id,
            UserId = order.UserId,
            OrderedItems = order.OrderedItems.Select(x => (OrderItemDTO)x).ToList()
        };
    }

    public static implicit operator Order(OrderDTO order_dto){
        return new Order{
            Id = order_dto.Id,
            UserId = order_dto.UserId,
            OrderedItems = order_dto.OrderedItems.Select(x => (OrderItem)x).ToList()
        };
    }
}

public class Order{
    public long Id { get; set; }
    public long UserId { get; set; }
    [Required]
    public User User { get; set; }

    [Required]
    public IEnumerable<OrderItem> OrderedItems { get; set; }
}

public class OrderItemDTO{
    public long Id { get; set; }
    public long ItemId { get; set; }
    // public long OrderId { get; set; }

    [Range(1, 200)]
    public int Quantity { get; set; }

    public static explicit operator OrderItemDTO(OrderItem order_item){
        return new OrderItemDTO{
            Id = order_item.Id,
            ItemId = order_item.ItemId,
            Quantity = order_item.Quantity,
            // OrderId = order_item.OrderId
        };
    }

    public static implicit operator OrderItem(OrderItemDTO order_item_dto){
        return new OrderItem{
            Id = order_item_dto.Id,
            ItemId = order_item_dto.ItemId,
            Quantity = order_item_dto.Quantity,
            // OrderId = order_item_dto.OrderId
        };
    }
}

public class OrderItem {
    public long Id { get; set; }

    public long ItemId { get; set; }
    [Required]
    public BookItem Item { get; set; }

    // public long OrderId { get; set; }
    // [Required]
    // public Order Order { get; set; }

    [Range(1, 200)]
    public int Quantity { get; set; }
}