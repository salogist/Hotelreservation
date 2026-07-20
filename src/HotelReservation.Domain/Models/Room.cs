namespace HotelReservation.Domain.Models;

public class Room
{
    public int RoomID { get; set; }
    public int HotelID { get; set; }
    public int RoomTypeID { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public string Status { get; set; } = "Available";
    public bool HasBalcony { get; set; }
    public bool HasSeaView { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public RoomType RoomType { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
