using CassandraSimpleApp.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraSimpleApp.Tools
{
    class ShopCart
    {
		private List<Product> productList;
		private Client client;
		public ShopCart()
        {
			productList = new List<Product>();
			client = new Client();
		}
		public void SetClient(string clientName, string deliveryAddress)
        {
			client.ClientName = clientName;
			client.DeliveryAddress = deliveryAddress;
        }
		public bool IsClientEmpty()
        {
			return String.IsNullOrEmpty(client.ClientName) || String.IsNullOrEmpty(client.DeliveryAddress);
        }
		public Client GetClient()
        {
			return client;
        }
		public void AddToCart(Product p)
		{
			Product existingProduct;
			if ((existingProduct = productList.FirstOrDefault(item => item.ProductName.Equals(p.ProductName) && item.Category.Equals(p.Category))) != null)
			{
				existingProduct.Amount += p.Amount;
			}
			else
			{
				productList.Add(p);
			}
		}

		public void RemoveFromCart(int index)
		{
			try
			{
				productList.RemoveAt(index - 1);
			}
			catch (IndexOutOfRangeException e)
			{
				throw;
			}
		}

		public List<Product> GetProducts()
		{
			List<Product> tmp = new List<Product>(productList);
			productList.Clear();
			return tmp;
		}

		public void DisplayCart()
		{
			if (productList.Count == 0)
			{
				Console.WriteLine("Koszyk pusty");
			}

			for (int i = 0; i < productList.Count; i++)
			{
				Console.WriteLine((i + 1) + ". " + productList[i]);
			}
		}
	}
}
