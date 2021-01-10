using CassandraSimpleApp.DBEntities;
using Dse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CassandraSimpleApp.Tools
{
    static class EntityMapper
    {
        public static List<Product> ToProducts(RowSet sessionResult)
        {
            List<Product> products = new List<Product>();
            foreach(Row loadedProduct in sessionResult)
            {
                products.Add(new Product(){
                    ProductName = loadedProduct.GetValue<string>("productname"),
                    Category = loadedProduct.GetValue<string>("category"),
                    Amount = loadedProduct.GetValue<int?>("amount"),
                    Price = loadedProduct.GetValue<double?>("price")
                    });;
            }
            return products;
        }
        public static List<Order> ToOrders(RowSet sessionResult)
        {
            List<Order> orders = new List<Order>();
            foreach(Row loadedOrder in sessionResult)
            {
                orders.Add(new Order()
                {
                    ClientName = loadedOrder.GetValue<string>("clientname"),
                    Timestamp = loadedOrder.GetValue<DateTime?>("date"),
                    ProductName = loadedOrder.GetValue<string>("productname"),
                    Category = loadedOrder.GetValue<string>("category"),
                    Amount = loadedOrder.GetValue<int?>("amount"),
                    DeliveryAddress = loadedOrder.GetValue<string>("deliveryaddress"),
                    Status = loadedOrder.GetValue<string>("status"),
                    Price = loadedOrder.GetValue<double?>("price")
                });
            }
            return orders;
        }
        public static Product StringArrayToProduct(string[] wholeCommand)
        {
            try
            {
                return new Product()
                {
                    ProductName = wholeCommand[1],
                    Category = wholeCommand[2],
                    Amount = Int32.Parse(wholeCommand[3]),
                    Price = Double.Parse(wholeCommand[4])
                };
            }
            catch (FormatException){
                throw;
            }
        }
        public static Order StringArrayToOrder(string[] wholeCommand)
        {
            try
            {
                return new Order()
                {
                    ClientName = wholeCommand[1],
                    Timestamp = DateTime.Parse(wholeCommand[2]),
                    ProductName = wholeCommand[3],
                    Category = wholeCommand[4],
                    Amount = Int32.Parse(wholeCommand[5]),
                    DeliveryAddress = wholeCommand[6],
                    Status = wholeCommand[7],
                    Price = Double.Parse(wholeCommand[8])
                };
            }
            catch (FormatException){
                throw;
            }
        }
    }
}
