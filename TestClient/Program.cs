using System;
using System.ServiceModel;
using TestClient.ProductsService;


namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            int controlNumber;
            Console.WriteLine("List all products: Enter 1");
            Console.WriteLine("Change Price: Enter 2");
            string number = Console.ReadLine();
            if (Int32.TryParse(number, out controlNumber))
            {
                using (Client client = new Client())
                {
                    client.TestProductsService(controlNumber);

                    Console.WriteLine("Press ENTER to finish");
                    Console.ReadLine();
                }
            }
        }
    }

    public class Client : ProductsServiceCallback, IDisposable
    {
        private ProductsServiceClient proxy = null;

        public void TestProductsService(int controlNumber)
        {
            // Create a proxy object and connect to the service
            proxy = new ProductsServiceClient(new InstanceContext(this), "WSDualHttpBinding_ProductsService");

            // Test the operations in the service
            try
            {
                proxy.SubscribeToPriceChangedEvent();

                // Obtain a list of products
                if (controlNumber == 1)
                {
                    Console.WriteLine("Control Number 1: List all products");
                    ProductData[] products = proxy.ListProducts();
                    foreach (ProductData product in products)
                    {
                        Console.WriteLine("Name: {0}", product.Name);
                        Console.WriteLine("Price: {0}", product.ListPrice);

                    }
                    Console.WriteLine();
                }


                // Modify the price of all products
                if (controlNumber == 2)
                {
                    Console.WriteLine("Control Number 2: Change product price");
                    ProductData[] products = proxy.ListProducts();
                    foreach (ProductData product in products)
                    {
                        proxy.ChangePrice(product.Name, product.ListPrice + 10);

                    }
                    Console.WriteLine();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }

        public void OnPriceChanged(ProductData product)
        {
            Console.WriteLine("\nCallback from service:\nPrice of {0} changed to {1:C}",
                product.Name, product.ListPrice);
        }

        public void Dispose()
        {
            // Disconnect from the service
            proxy.Close();
        }
    }
}
