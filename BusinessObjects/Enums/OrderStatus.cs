namespace BusinessObjects.Enums
{
    public enum OrderStatus
    {
        Pending,          
        Confirmed,        // comfirmed by seller
        Processing,
        Shipping,           // choser shipper by admin
        Delivered,
        Cancelled,
        Returned,
        Refunded
    }
}

