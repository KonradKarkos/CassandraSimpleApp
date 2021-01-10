using CassandraSimpleApp.DBEntities;
using Dse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CassandraSimpleApp.Tools
{
    class SessionManager
    {
        private DseCluster cluster;
        private IDseSession session;
        private Dictionary<Statements, PreparedStatement> preparedStatements;
        private Dictionary<Statements, SimpleStatement> simpleStatements;
        public SessionManager()
        {
            cluster = DseCluster.Builder().AddContactPoint("127.0.0.1").Build();
            session = cluster.Connect("test");
            var row = session.Execute("select * from system.local").First();
            Console.WriteLine("Podłączono do klastra " + row.GetValue<string>("cluster_name"));
            PrepareStatements();
        }
        public RowSet Invoke(string command)
        {
            return session.Execute(command);
        }
        public RowSet Invoke(Statements s)
        {
            return session.Execute(simpleStatements[s]);
        }
        public RowSet Invoke(Statements s, object[] parameters)
        {
            return session.Execute(preparedStatements[s].Bind(parameters));
        }
        public RowSet InvokeUpdateOrder(object[] parameters)
        {
            BatchStatement batch = new BatchStatement();
            List<Order> orderProducts = EntityMapper.ToOrders(session.Execute(preparedStatements[Statements.SELECT_ORDER_FROM_ORDER].Bind(parameters)));
            foreach(Order orderToUpdate in orderProducts)
            {
                batch.Add(preparedStatements[Statements.UPDATE_STATUS_ORDER].Bind(parameters[0], parameters[1], orderToUpdate.ProductName));
            }
            return session.Execute(batch);
        }
        public RowSet InvokeDeleteOrder(object[] parameters)
        {
            BatchStatement batch = new BatchStatement();
            List<Order> orderProducts = EntityMapper.ToOrders(session.Execute(preparedStatements[Statements.SELECT_ORDER_FROM_ORDER].Bind(parameters)));
            Product dbProduct;
            foreach (Order cancelledOrder in orderProducts)
            {
                dbProduct = EntityMapper.ToProducts(session.Execute(session.Prepare("SELECT * FROM products WHERE category=? AND productname=?").Bind(cancelledOrder.Category, cancelledOrder.ProductName))).FirstOrDefault();
                batch.Add(preparedStatements[Statements.UPDATE_PRODUCT_AMOUNT].Bind(dbProduct.Amount+cancelledOrder.Amount, cancelledOrder.Category, cancelledOrder.ProductName));
            }
            batch.Add(preparedStatements[Statements.DELETE_ORDER_FROM_ORDERS].Bind(parameters));
            return session.Execute(batch);
        }
        public RowSet InvokeBatchStatement(List<Product> shopCartProducts, Client client)
        {
            BatchStatement batch = new BatchStatement();
            DateTime dateTime = DateTime.Now;
            object guid = Guid.NewGuid();
            Product dbProduct;
            foreach (Product p in shopCartProducts)
            {
                dbProduct = EntityMapper.ToProducts(session.Execute(session.Prepare("SELECT * FROM products WHERE category=? AND productname=?").Bind(p.Category, p.ProductName))).FirstOrDefault();
                if (dbProduct.Amount >= p.Amount)
                {
                    batch.Add(preparedStatements[Statements.INSERT_ORDER_INTO_ORDERS].Bind(guid, client.ClientName, dateTime, p.ProductName, p.Category, p.Amount, client.DeliveryAddress, "UNPAID", p.Price));
                    batch.Add(preparedStatements[Statements.UPDATE_PRODUCT_AMOUNT].Bind(dbProduct.Amount-p.Amount, p.Category, p.ProductName));
                }
                else
                {
                    Console.WriteLine("Nie udało się zrealizować zamówienia przez niewystarczyającą ilość produktu na stanie");
                }
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"Guids.txt", true))
            {
                file.WriteLine(((Guid)guid).ToString());
            }
            return session.Execute(batch);
        }
        private void PrepareStatements()
        {
            simpleStatements = new Dictionary<Statements, SimpleStatement>();
            simpleStatements.Add(Statements.SELECT_ALL_FROM_PRODUCTS, new SimpleStatement("SELECT * FROM products"));
            preparedStatements = new Dictionary<Statements, PreparedStatement>();
            preparedStatements.Add(Statements.SELECT_ALL_FROM_PRODUCTS_WITH_CATEGORY, session.Prepare("SELECT * FROM products WHERE category=?"));
            preparedStatements.Add(Statements.UPDATE_PRODUCT_AMOUNT, session.Prepare("UPDATE products SET amount=? WHERE category=? AND productname=?"));
            preparedStatements.Add(Statements.INSERT_PRODUCT_INTO_PRODUCTS, session.Prepare("INSERT INTO products (productname, category, amount, price) VALUES (?,?,?,?);"));
            preparedStatements.Add(Statements.INSERT_ORDER_INTO_ORDERS, session.Prepare("INSERT INTO orders (orderid, clientname, date, productname, category, amount, deliveryaddress, status, price) VALUES (?,?,?,?,?,?,?,?,?);"));
            preparedStatements.Add(Statements.UPDATE_STATUS_ORDER, session.Prepare("UPDATE orders SET status='COMPLETED' WHERE clientname=? AND orderid=? AND productname=?"));
            preparedStatements.Add(Statements.SELECT_ORDER_FROM_ORDER, session.Prepare("SELECT * FROM orders WHERE clientname=? AND orderid=?"));
            preparedStatements.Add(Statements.DELETE_ORDER_FROM_ORDERS, session.Prepare("DELETE FROM orders WHERE clientname=? AND orderid=?"));
        }
    }
}
