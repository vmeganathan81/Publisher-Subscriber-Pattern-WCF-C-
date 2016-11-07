using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary
{
    // Data contract describing the details of a product passed to client applications
    [DataContract]
    public class ProductData
    {
        [DataMember]
        public string Name;

        [DataMember]
        public decimal ListPrice;
    }


    // Callback interface for notifying the client that the price has changed
    public interface IProductsServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnPriceChanged(ProductData product);
    }


    [ServiceContract(Namespace = "http://eclinicalworks.com",
                    Name = "ProductsService",
                    CallbackContract=typeof(IProductsServiceCallback))]
   
    public interface IProductsService
    {
        // Get the list of the products
        [OperationContract]
        List<ProductData> ListProducts();

        // Change the price of a product
        [OperationContract(IsOneWay = false)]
        void ChangePrice(string productName, decimal price);

        // Subscribe tob the "price changed" event
        [OperationContract]
        bool SubscribeToPriceChangedEvent();

        // Unsubscribe from the "price changed" event
        [OperationContract]
        bool UnsubscribeFromPriceChangedEvent();
    }
}
