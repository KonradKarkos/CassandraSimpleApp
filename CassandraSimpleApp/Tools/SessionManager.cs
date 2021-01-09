using CassandraSimpleApp.DBEntities;
using Dse;
using Dse.Mapping;
using System.Collections.Generic;

namespace CassandraSimpleApp.Tools
{
    class SessionManager
    {
        private DseCluster cluster;
        private IDseSession session;
        private Dictionary<Statements, PreparedStatement> preparedStatements;
        private Dictionary<Statements, SimpleStatement> simpleStatements;
        private Mapper mapper;
        public SessionManager()
        {
            cluster = DseCluster.Builder().AddContactPoint("127.0.0.1").Build();
            session = cluster.Connect();
            mapper = new Mapper(session);
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
        private void PrepareStatements()
        {
            simpleStatements = new Dictionary<Statements, SimpleStatement>();
            simpleStatements.Add(Statements.SELECT_ALL_FROM_PRODUCTS, new SimpleStatement("SELECT * FROM products"));
            preparedStatements = new Dictionary<Statements, PreparedStatement>();
            preparedStatements.Add(Statements.SELECT_ALL_FROM_PRODUCTS_WITH_CATEGORY, session.Prepare("SELECT * FROM products WHERE category=?"));
            preparedStatements.Add(Statements.UPDATE_PRODUCT_AMOUNT, session.Prepare("UPDATE products SET amount=? WHERE category=? AND productName=?"));
            preparedStatements.Add(Statements.INSERT_PRODUCT_INTO_PRODUCTS, session.Prepare("INSERT INTO products (productName, category, amount, price) VALUES (?,?,?,?,?);"));
            preparedStatements.Add(Statements.INSERT_ORDER_INTO_ORDERS, session.Prepare("INSERT INTO orders (clientName, date, productName, category, amount, deliveryAddress, status, price) VALUES (?,?,?,?,?,?,?);"));
            preparedStatements.Add(Statements.UPDATE_STATUS_ORDER, session.Prepare("UPDATE orders SET status ='COMPLETED' WHERE clientName =? AND date =?"));
            preparedStatements.Add(Statements.SELECT_ORDER_FROM_ORDER, session.Prepare("SELECT * FROM orders WHERE clientName=? AND date=?"));
            preparedStatements.Add(Statements.DELETE_ORDER_FROM_ORDERS, session.Prepare("DELETE FROM orders WHERE clientName=? AND date=?"));
        }
    }
}
