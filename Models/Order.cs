using System.ComponentModel.DataAnnotations;
using EasyBook.Filters;

namespace EasyBook.Models;

public enum OrderStatus {
    Processing,
    Delievery,
    Delievered,
    Canceled
}

public class OrderDTO{
    public long Id { get; set; }
    public long UserId { get; set; }
    public OrderStatus? Status { get; set; }

    public IEnumerable<OrderItemDTO> OrderedItems { get; set; }

    public static explicit operator OrderDTO(Order order){
        return new OrderDTO{
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            OrderedItems = order.OrderedItems.Select(x => (OrderItemDTO)x)
        };
    }

    public static implicit operator Order(OrderDTO order_dto){
        return new Order{
            Id = order_dto.Id,
            UserId = order_dto.UserId,

            // Gets prefilled with Processing, during posting
            Status = (OrderStatus) order_dto.Status!,
            OrderedItems = order_dto.OrderedItems.Select(x => (OrderItem)x).ToList()
        };
    }
}

public class Order : ICreatedByUser {
    public long Id { get; set; }
    public long UserId { get; set; }
    [Required]
    public User User { get; set; }

    public OrderStatus Status { get; set; }

    [Required]
    public IEnumerable<OrderItem> OrderedItems { get; set; }
}

public class OrderItemDTO{
    public long Id { get; set; }
    public long ItemId { get; set; }

    [Range(1, 200)]
    public int Quantity { get; set; }

    public static explicit operator OrderItemDTO(OrderItem order_item){
        return new OrderItemDTO{
            Id = order_item.Id,
            ItemId = order_item.ItemId,
            Quantity = order_item.Quantity
        };
    }

    public static implicit operator OrderItem(OrderItemDTO order_item_dto){
        return new OrderItem{
            Id = order_item_dto.Id,
            ItemId = order_item_dto.ItemId,
            Quantity = order_item_dto.Quantity
        };
    }
}

public class OrderItem {
    public long Id { get; set; }

    public long ItemId { get; set; }
    [Required]
    public BookItem Item { get; set; }

    [Range(1, 200)]
    public int Quantity { get; set; }
}