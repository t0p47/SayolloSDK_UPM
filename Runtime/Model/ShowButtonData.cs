using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sayollo {
    public class ShowButtonData
    {
        public ShowButtonData(string getProduct)
        {
            this.GetProduct = getProduct;
        }

        public string GetProduct { get; set; }
    }

}
