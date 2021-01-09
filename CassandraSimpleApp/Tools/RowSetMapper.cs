using CassandraSimpleApp.DBEntities;
using Dse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CassandraSimpleApp.Tools
{
    static class RowSetMapper
    {
        public static List<Product> ToProducts(RowSet sessionResult)
        {
            List<Product> products = new List<Product>();
            Row[] resultRows = sessionResult.ToArray();
            foreach(Row loadedProduct in resultRows)
            {
                products.Add(new Product(){
                    ProductName = loadedProduct.GetValue<string>("productName"),
                    Category = loadedProduct.GetValue<string>("category"),
                    Amount = loadedProduct.GetValue<int>("amount"),
                    Price = loadedProduct.GetValue<double>("price")
                    });;
            }
            return products;
        }
        public static List<Order> ToOrders(RowSet sessionResult)
        {
            List<Order> orders = new List<Order>();
            Row[] resultRows = sessionResult.ToArray();
            foreach(Row loadedOrder in resultRows)
            {
                orders.Add(new Order()
                {
                    ClientName = loadedOrder.GetValue<string>("clientName"),
                    Timestamp = loadedOrder.GetValue<DateTime>("timestamp"),
                    ProductName = loadedOrder.GetValue<string>("productName"),
                    Category = loadedOrder.GetValue<string>("category"),
                    Amount = loadedOrder.GetValue<int>("amount"),
                    DeliveryAddress = loadedOrder.GetValue<string>("deliveryAddress"),
                    Status = loadedOrder.GetValue<string>("status"),
                    Price = loadedOrder.GetValue<double>("price")
                });
            }
            return orders;
        }
    }
}
