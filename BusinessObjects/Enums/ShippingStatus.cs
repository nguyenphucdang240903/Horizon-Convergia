namespace BusinessObjects.Enums
{
    public enum ShippingStatus
    {
        Pending, // watting admin chose shipper     
        InTransit,     // shipper is delivering the order
        Delivered,      // done    
        Failed,
        Returned,
        Cancelled
    }
}
