using CassandraSimpleApp.DBEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CassandraSimpleApp.Tools
{
    class OverloadGenerator
    {
        private int[][] increasedProducerValues;
        private const int productsToOverload = 10;
        public void StartOverload(int producerCount, int iterations, int max, int min)
        {
            increasedProducerValues = new int[producerCount][];
            SessionManager sessionManager = new SessionManager();
            Product[] choosenProducts = EntityMapper.ToProducts(sessionManager.Invoke(Statements.SELECT_ALL_FROM_PRODUCTS).Take(10)).ToArray();
            int[] startingValues = new int[productsToOverload];
            Console.WriteLine("Wybrane produkty:");
            for (int i=0;i< productsToOverload; i++)
            {
                startingValues[i] = choosenProducts[i].Amount.Value;
                Console.WriteLine(i +". " + choosenProducts[i].Category + " " + choosenProducts[i].ProductName + " " + choosenProducts[i].Amount);
            }
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = false;
            backgroundWorker.DoWork += FinalBackgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync(argument: new object[] { producerCount, iterations, max, min, startingValues});
        }
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SessionManager sessionManager = new SessionManager();
            Product[] choosenProducts = EntityMapper.ToProducts(sessionManager.Invoke(Statements.SELECT_ALL_FROM_PRODUCTS).Take(10)).ToArray();
            object[] values = (object[])e.Argument;
            int iterations = (int)values[0];
            int max = (int)values[1] + 1;
            int min = (int)values[2];
            Random rnd = (Random)values[3];
            int index = (int)values[4];
            CountdownEvent countdownEvent = (CountdownEvent)values[5];
            int[] increasedValues = new int[productsToOverload];
            int nextInt;
            Product dbProduct;
            for (int i=0;i<iterations;i++)
            {
                for(int j =0; j< productsToOverload; j++)
                {
                    Thread.Sleep(rnd.Next(10,101));
                    nextInt = rnd.Next(min, max);
                    dbProduct = EntityMapper.ToProducts(sessionManager.Invoke("SELECT * FROM products WHERE category='"+choosenProducts[j].Category+"' AND productname='"+choosenProducts[j].ProductName+"'")).FirstOrDefault();
                    sessionManager.Invoke(Statements.UPDATE_PRODUCT_AMOUNT, new object[] { dbProduct.Amount+nextInt, choosenProducts[j].Category, choosenProducts[j].ProductName});
                    increasedValues[j] += nextInt;
                }
            }
            increasedProducerValues[index] = increasedValues;
            countdownEvent.Signal();
        }
        private void FinalBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] values = (object[])e.Argument;
            int producerCount = (int)values[0];
            int iterations = (int)values[1];
            int max = (int)values[2];
            int min = (int)values[3];
            int[] startingValues = (int[])values[4];
            CountdownEvent countdownEvent = new CountdownEvent(producerCount);
            Random rnd = new Random();
            BackgroundWorker[] backgroundWorkers = new BackgroundWorker[producerCount];
            for(int i=0; i< producerCount;i++)
            {
                backgroundWorkers[i] = new BackgroundWorker();
                backgroundWorkers[i].WorkerReportsProgress = false;
                backgroundWorkers[i].DoWork += BackgroundWorker_DoWork;
                backgroundWorkers[i].RunWorkerAsync(argument: new object[] { iterations, max, min, rnd, i, countdownEvent});
            }
            countdownEvent.Wait();
            int[] addedValues = new int[productsToOverload];
            for(int i=0;i<producerCount;i++)
            {
                for(int j =0;j<productsToOverload;j++)
                {
                    addedValues[j] += increasedProducerValues[i][j];
                }
            }
            Console.WriteLine("Uzyskane wyniki przeciążonego dodawania (początkowo-dodano-spodziewana ilość):");
            for(int i=0; i<productsToOverload; i++)
            {
                Console.WriteLine(i + ". " + startingValues[i] + "-" + addedValues[i] + "-" + (addedValues[i] + startingValues[i]));
            }
        }
    }
}
