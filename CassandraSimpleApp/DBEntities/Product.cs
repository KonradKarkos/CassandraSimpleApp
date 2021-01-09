namespace CassandraSimpleApp.DBEntities
{
    class Product
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public override string ToString()
        {
            return ProductName + " " + Category + " " + Amount + " " + Price;
        }
    }
}
