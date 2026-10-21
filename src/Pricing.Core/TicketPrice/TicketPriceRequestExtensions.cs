namespace Pricing.Core.TicketPrice;

public static class TicketPriceRequestExtensions
{
    public static int GetDurationInHours(this TicketPriceRequest request)
    {
        const int upperHourLimit = 59;
        const int oneHourInMinutes = 60;

        var ticketDuration = (int)(request.Exit - request.Entry).TotalMinutes;

        return (ticketDuration + upperHourLimit) / oneHourInMinutes;
    }
}