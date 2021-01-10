using CassandraSimpleApp.DBEntities;
using CassandraSimpleApp.Tools;
using Dse;
using Dse.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CassandraSimpleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int choosenCommand;
            string[] wholeCommand;
            SessionManager sessionManager = new SessionManager();
            ShopCart shopCart = new ShopCart();
            string clientName;
            string deliveryAddress;
            while (true)
            {
                Console.Write("\nWybierz opcje wykonania się programu (wpisz liczbę i ewentualne parametry po spacjach):\n" +
                "1. Wypisz produkty (dodatkowy parametr - kategoria)\n" +
                "2. Zmień ilość dostępnych produktów (parametry - ilość;kategoria;nazwa produktu)\n" +
                "3. Stwórz produkt (parametry - nazwa produktu;kategoria;ilość;cena)\n" +
                "4. Zrealizuj zamówienie (parametry - nazwa klienta;ID zamówienia)\n" +
                "5. Sprawdź zamówienie (parametry - nazwa klienta;ID zamówienia)\n" +
                "6. Usuń zamówienie (parametry - nazwa klienta;ID zamówienia)\n" +
                "7. Nadaj klienta dla koszyka\n" +
                "8. Dodaj produkt do koszyka (parametry - nazwa produktu;nazwa kategorii;ilość;cena)\n" +
                "9. Usuń produkt z koszyka (parametry - indeks)\n" +
                "10. Przejrzyj koszyk\n" +
                "11. Zamów zawartość koszyka.\n" +
                "12. Przeciąż produkty.\n" +
                "13. Własna komenda\n");
                Console.Write(">");
                wholeCommand = Console.ReadLine().Split(';');
                if (!Int32.TryParse(wholeCommand[0], out choosenCommand))
                {
                    Console.WriteLine("Błędne polecenie, spróbuj ponownie ;-)");
                }
                else
                {
                    switch (choosenCommand)
                    {
                        case 1:
                            if(wholeCommand.Length>1)
                            {
                                List<Product> loadedProducts = EntityMapper.ToProducts(sessionManager.Invoke(Statements.SELECT_ALL_FROM_PRODUCTS_WITH_CATEGORY, new object[] { wholeCommand[1] }));
                                foreach(Product product in loadedProducts)
                                {
                                    Console.WriteLine(product.ToString());
                                }
                            }
                            else
                            {
                                List<Product> loadedProducts = EntityMapper.ToProducts(sessionManager.Invoke(Statements.SELECT_ALL_FROM_PRODUCTS));
                                foreach (Product product in loadedProducts)
                                {
                                    Console.WriteLine(product.ToString());
                                }
                            }
                            break;
                        case 2:
                            if (wholeCommand.Length == 4)
                            {
                                object[] parameter = new object[wholeCommand.Length - 1];
                                Array.Copy(wholeCommand, 1, parameter, 0, wholeCommand.Length - 1);
                                parameter[0] = Int32.Parse((string)parameter[0]);
                                sessionManager.Invoke(Statements.UPDATE_PRODUCT_AMOUNT, parameter);
                            }
                            else
                            {
                                Console.WriteLine("Zła liczba parametrów");
                            }
                            break;
                        case 3:
                            if (wholeCommand.Length == 5)
                            {
                                object[] parameter = new object[wholeCommand.Length - 1];
                                Array.Copy(wholeCommand, 1, parameter, 0, wholeCommand.Length - 1);
                                parameter[2] = Int32.Parse((string)parameter[2]);
                                parameter[3] = Double.Parse((string)parameter[3]);
                                sessionManager.Invoke(Statements.INSERT_PRODUCT_INTO_PRODUCTS, parameter);
                            }
                            else
                            {
                                Console.WriteLine("Zła liczba parametrów");
                            }
                            break;
                        case 4:
                            if (wholeCommand.Length == 3)
                            {
                                object[] parameter = new object[wholeCommand.Length - 1];
                                Array.Copy(wholeCommand, 1, parameter, 0, wholeCommand.Length - 1);
                                parameter[1] = Guid.Parse(wholeCommand[2]);
                                sessionManager.InvokeUpdateOrder(parameter);
                            }
                            else
                            {
                                Console.WriteLine("Zła liczba parametrów");
                            }
                            break;
                        case 5:
                            if (wholeCommand.Length == 3)
                            {
                                object[] parameter = new object[wholeCommand.Length - 1];
                                Array.Copy(wholeCommand, 1, parameter, 0, wholeCommand.Length - 1);
                                parameter[1] = Guid.Parse(wholeCommand[2]);
                                foreach(Order o in EntityMapper.ToOrders(sessionManager.Invoke(Statements.SELECT_ORDER_FROM_ORDER, parameter)))
                                {
                                    Console.WriteLine(o.ToString());
                                }
                            }
                            else
                            {
                                Console.WriteLine("Zła liczba parametrów");
                            }
                            break;
                        case 6:
                            if (wholeCommand.Length == 3)
                            {
                                object[] parameter = new object[wholeCommand.Length - 1];
                                Array.Copy(wholeCommand, 1, parameter, 0, wholeCommand.Length - 1);
                                parameter[1] = Guid.Parse(wholeCommand[2]);
                                sessionManager.InvokeDeleteOrder(parameter);
                            }
                            else
                            {
                                Console.WriteLine("Zła liczba parametrów");
                            }
                            break;
                        case 7:
                            Console.Write("Podaj nazwę klienta i adres dostawy:\n" +
                                "Nazwa klienta:\n>");
                            clientName = Console.ReadLine();
                            Console.Write("Adres dostawy:\n>");
                            deliveryAddress = Console.ReadLine();
                            shopCart.SetClient(clientName, deliveryAddress);
                            break;
                        case 8:
                            if (wholeCommand.Length == 5)
                            {
                                try
                                {
                                    if(shopCart.IsClientEmpty())
                                    {
                                        Console.Write("Podaj nazwę klienta i adres dostawy:\n" +
                                            "Nazwa klienta:\n>");
                                        clientName = Console.ReadLine();
                                        Console.Write("Adres dostawy:\n>");
                                        deliveryAddress = Console.ReadLine();
                                        shopCart.SetClient(clientName, deliveryAddress);
                                    }
                                    shopCart.AddToCart(EntityMapper.StringArrayToProduct(wholeCommand));
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("Błędnie wywołana funkcja, spróbuj ponownie ;-)");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Błędnie wywołana funkcja, spróbuj ponownie ;-)");
                            }
                            break;
                        case 9:
                            if (wholeCommand.Length == 2)
                            {
                                try
                                {
                                    shopCart.RemoveFromCart(Int32.Parse(wholeCommand[1]));
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    Console.WriteLine("Błędnie wywołana funkcja, spróbuj ponownie ;-)");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Błędnie wywołana funkcja, spróbuj ponownie ;-)");
                            }
                            break;
                        case 10:
                            shopCart.DisplayCart();
                            break;
                        case 11:
                            sessionManager.InvokeBatchStatement(shopCart.GetProducts(), shopCart.GetClient());
                            break;
                        case 12:
                            OverloadGenerator overloadGenerator = new OverloadGenerator();
                            overloadGenerator.StartOverload(5, 10, 6, 2);
                            break;
                        case 13:
                            Console.Write("Wpisz komendę CQL:\n>");
                            var rows = sessionManager.Invoke(Console.ReadLine());
                            foreach(var row in rows)
                            {
                                for(int i=0;i< row.Length;i++)
                                {
                                    Console.Write(row.GetValue<object>(i) + " ");
                                }
                                Console.Write("\n");
                            }
                            break;
                        default:
                            Console.WriteLine("Nie wykryto komendy.");
                            break;
                    }
                }
            }
        }
    }
}
