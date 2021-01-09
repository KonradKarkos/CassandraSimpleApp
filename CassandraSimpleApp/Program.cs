using CassandraSimpleApp.DBEntities;
using CassandraSimpleApp.Tools;
using Dse;
using Dse.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CassandraSimpleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int choosenCommand;
            string[] wholeCommand;
            SessionManager sessionManager = new SessionManager();
            Console.Write("Wybierz opcje wykonania się programu (wpisz liczbę i ewentualne parametry po spacjach):\n" +
                "1. Wypisz produkty (dodatkowy parametr - kategoria)\n" +
                "2. Zmień liczbę dostępnych produktów (parametry - ilość, kategoria, nazwa produktu)\n" +
                "3. Stwórz produkt (parametry - nazwa produktu, kategoria, ilość, cena)\n" +
                "4. Stwórz zamówienie (parametry - nazwa klienta, nazwa produktu, nazwa kategorii, ilość, adres, cena)\n" +
                "5. Zrealizuj zamówienie (parametry - nazwa klienta, data)\n" +
                "6. Sprawdź zamówienie (parametry - nazwa klienta, data)\n" +
                "7. Usuń zamówienie (parametry - nazwa klienta, data)\n" +
                "8. Dodaj produkt do koszyka (parametry - nazwa produktu, nazwa kategorii, ilość, cena)\n" +
                "9. Usuń produkt z koszyka (parametry - indeks)\n" +
                "10. Przejrzyj koszyk\n" +
                "11. Własna komenda");
            while (true)
            {
                wholeCommand = Console.ReadLine().Split(' ');
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
                                List<Product> loadedProducts = RowSetMapper.ToProducts(sessionManager.Invoke(Statements.SELECT_ALL_FROM_PRODUCTS_WITH_CATEGORY, new object[] { wholeCommand[1] }));
                                foreach(Product product in loadedProducts)
                                {
                                    Console.WriteLine(product.ToString());
                                }
                            }
                            else
                            {
                                List<Product> loadedProducts = RowSetMapper.ToProducts(sessionManager.Invoke(Statements.SELECT_ALL_FROM_PRODUCTS));
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
                                wholeCommand.CopyTo(parameter, 1);
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
                                wholeCommand.CopyTo(parameter, 1);
                                sessionManager.Invoke(Statements.INSERT_PRODUCT_INTO_PRODUCTS, parameter);
                            }
                            else
                            {
                                Console.WriteLine("Zła liczba parametrów");
                            }
                            break;
                        case 4:
                            if (wholeCommand.Length == 7)
                            {
                                object[] parameter = new object[wholeCommand.Length - 1];
                                wholeCommand.CopyTo(parameter, 1);
                                sessionManager.Invoke(Statements.INSERT_ORDER_INTO_ORDERS, parameter);
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
                                wholeCommand.CopyTo(parameter, 1);
                                sessionManager.Invoke(Statements.UPDATE_STATUS_ORDER, parameter);
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
                                wholeCommand.CopyTo(parameter, 1);
                                sessionManager.Invoke(Statements.SELECT_ORDER_FROM_ORDER, parameter);
                            }
                            else
                            {
                                Console.WriteLine("Zła liczba parametrów");
                            }
                            break;
                        case 7:
                            if (wholeCommand.Length == 3)
                            {
                                object[] parameter = new object[wholeCommand.Length - 1];
                                wholeCommand.CopyTo(parameter, 1);
                                sessionManager.Invoke(Statements.INSERT_PRODUCT_INTO_PRODUCTS, parameter);
                            }
                            else
                            {
                                Console.WriteLine("Zła liczba parametrów");
                            }
                            break;
                        case 8:
                            break;
                        case 9:
                            break;
                        case 10:
                            break;
                        case 11:
                            Console.WriteLine("Wpisz komendę CQL:");
                            sessionManager.Invoke(Console.ReadLine());
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
