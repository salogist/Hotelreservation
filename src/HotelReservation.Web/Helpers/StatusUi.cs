namespace HotelReservation.Web.Helpers;

/// <summary>
/// Provides CSS class maps for colored status badges used across the UI,
/// so colors stay consistent on every list, details and dashboard page.
/// </summary>
public static class StatusUi
{
    /// <summary>Bootstrap background class for a reservation status value.</summary>
    public static string ReservationBadge(string? status) => status switch
    {
        "Confirmed" => "bg-success",
        "Pending" => "bg-warning text-dark",
        "Cancelled" => "bg-danger",
        "Completed" => "bg-secondary",
        _ => "bg-secondary"
    };

    /// <summary>Bootstrap background class for a room status value.</summary>
    public static string RoomBadge(string? status) => status switch
    {
        "Available" => "bg-success",
        "Occupied" => "bg-info text-dark",
        "Maintenance" => "bg-secondary",
        _ => "bg-secondary"
    };

    /// <summary>Bootstrap background class for a payment status value.</summary>
    public static string PaymentBadge(string? status) => status switch
    {
        "Paid" => "bg-success",
        "Pending" => "bg-warning text-dark",
        "Partial" => "bg-info text-dark",
        "Refunded" => "bg-danger",
        _ => "bg-secondary"
    };

    /// <summary>The full list of reservation statuses used in dropdowns.</summary>
    public static readonly string[] ReservationStatuses =
        { "Pending", "Confirmed", "Cancelled", "Completed" };

    /// <summary>The full list of room statuses used in dropdowns.</summary>
    public static readonly string[] RoomStatuses =
        { "Available", "Occupied", "Maintenance" };

    /// <summary>The full list of payment statuses used in dropdowns.</summary>
    public static readonly string[] PaymentStatuses =
        { "Paid", "Pending", "Partial", "Refunded" };

    /// <summary>The full list of payment methods used in dropdowns.</summary>
    public static readonly string[] PaymentMethods =
        { "Cash", "CreditCard", "DebitCard", "OnlinePayment", "BankTransfer" };

    /// <summary>Common service categories used in dropdowns.</summary>
    public static readonly string[] ServiceCategories =
        { "Dining", "Spa", "Transport", "Laundry", "Entertainment", "Business", "Other" };

    /// <summary>Employee positions used as suggestions in the create/edit form.</summary>
    public static readonly string[] EmployeePositions =
        { "General Manager", "Receptionist", "Concierge", "Housekeeper", "Chef", "Waiter", "Bellboy", "Security" };
}
