using HotelReservation.Application.Common;

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
        DomainValues.Reservations.Confirmed => "bg-success",
        DomainValues.Reservations.Pending => "bg-warning text-dark",
        DomainValues.Reservations.Cancelled => "bg-danger",
        DomainValues.Reservations.Completed => "bg-secondary",
        _ => "bg-secondary"
    };

    /// <summary>Bootstrap background class for a room status value.</summary>
    public static string RoomBadge(string? status) => status switch
    {
        DomainValues.Rooms.Available => "bg-success",
        DomainValues.Rooms.Occupied => "bg-info text-dark",
        DomainValues.Rooms.Maintenance => "bg-secondary",
        _ => "bg-secondary"
    };

    /// <summary>Bootstrap background class for a payment status value.</summary>
    public static string PaymentBadge(string? status) => status switch
    {
        DomainValues.Payments.Paid => "bg-success",
        DomainValues.Payments.Pending => "bg-warning text-dark",
        DomainValues.Payments.Partial => "bg-info text-dark",
        DomainValues.Payments.Refunded => "bg-danger",
        _ => "bg-secondary"
    };

    /// <summary>The full list of reservation statuses used in dropdowns.</summary>
    public static readonly string[] ReservationStatuses = DomainValues.Reservations.All;

    /// <summary>The full list of room statuses used in dropdowns.</summary>
    public static readonly string[] RoomStatuses = DomainValues.Rooms.All;

    /// <summary>The full list of payment statuses used in dropdowns.</summary>
    public static readonly string[] PaymentStatuses = DomainValues.Payments.Statuses;

    /// <summary>The full list of payment methods used in dropdowns.</summary>
    public static readonly string[] PaymentMethods = DomainValues.Payments.Methods;

    /// <summary>Common service categories used in dropdowns.</summary>
    public static readonly string[] ServiceCategories =
        { "Dining", "Spa", "Transport", "Laundry", "Entertainment", "Business", "Other" };

    /// <summary>Employee positions used as suggestions in the create/edit form.</summary>
    public static readonly string[] EmployeePositions =
        { "General Manager", "Receptionist", "Concierge", "Housekeeper", "Chef", "Waiter", "Bellboy", "Security" };
}
