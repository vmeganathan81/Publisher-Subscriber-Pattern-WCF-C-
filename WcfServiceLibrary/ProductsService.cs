using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace WcfServiceLibrary
{
    public class ProductsService : IProductsService
    {
        static List<IProductsServiceCallback> subscribers =
         new List<IProductsServiceCallback>();


        public List<ProductData> ListProducts()
        {
            // Create a list for holding product numbers
            List<ProductData> productsList = new List<ProductData>();

            try
            {
                // Add list of productdata to the product list
                ProductData productData = null;

                productData = new ProductData()
                {
                    Name = @"Dummy 1",
                    ListPrice = 19.99M
                };

                productsList.Add(productData);

                productData = new ProductData()
                {
                    Name = @"Dummy 2",
                    ListPrice = 29.99M
                };

                productsList.Add(productData);


            }
            catch (Exception e)
            {
                // Ignore exceptions in this implementation
            }

            // Return the list of product numbers
            return productsList;
        }

        public void ChangePrice(string productName, decimal price)
        {
            // Modify the price of the selected product 
            // If the update is successful then return true, otherwise return false.
            ProductData product = null;

            try
            {
                List<ProductData> products = ListProducts();

                // Find the specified product
                product = (from p in products
                           where String.Compare(p.Name, productName) == 0
                           select p).First();

                // Change the price for the product
                product.ListPrice = price;

            }
            catch
            {
                // If an exception occurs, return false to indicate failure
                return;
            }

            // Notify the client that the price has been changed successfully
            ProductData productData = new ProductData()
            {
                Name = product.Name,
                ListPrice = product.ListPrice,
            };

            raisePriceChangedEvent(productData);
        }

        public bool SubscribeToPriceChangedEvent()
        {
            try
            {
                IProductsServiceCallback callback =
                    OperationContext.Current.GetCallbackChannel<IProductsServiceCallback>();
                if (!subscribers.Contains(callback))
                {
                    subscribers.Add(callback);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UnsubscribeFromPriceChangedEvent()
        {
            try
            {
                IProductsServiceCallback callback =
                    OperationContext.Current.GetCallbackChannel<IProductsServiceCallback>();
                subscribers.Remove(callback);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        //raise the price change event, and notify the clients (subscribers) the price has been changed
        private void raisePriceChangedEvent(ProductData product)
        {
            subscribers.AsParallel().ForAll(callback =>
            {
                if (((ICommunicationObject)callback).State == CommunicationState.Opened)
                {
                    callback.OnPriceChanged(product);
                }
                else
                {
                    subscribers.Remove(callback);
                }
            }
        );
        }
    }
}
