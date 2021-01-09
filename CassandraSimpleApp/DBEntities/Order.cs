using System;

namespace CassandraSimpleApp.DBEntities
{
    class Order
    {
        public string ClientName { get; set; }
        public DateTime Timestamp { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public string DeliveryAddress { get; set; }
        public string Status { get; set; }
        public double Price { get; set; }
    }
}
