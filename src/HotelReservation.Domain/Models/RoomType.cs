namespace HotelReservation.Domain.Models;

public class RoomType
{
    public int RoomTypeID { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
