using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xbim;
using Xbim.IO;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc2x3;
using Xbim.XbimExtensions;

using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.ModelGeometry.Converter;
using Xbim.Ifc2x3.Extensions;

using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.Ifc2x3.RepresentationResource;

using System.Runtime.Serialization;
//using System.Web.Script.Serialization.JavaScriptSerializer;
//using Newtonsoft.Json;

namespace XBIMConsole
{
    class IfcProductsToJson
    {
        public void writeProducts(XbimModel model) {
            IEnumerable<IfcProduct> products = model.Instances.OfType<IfcProduct>();

            Dictionary<Dictionary<string, string>, Dictionary<string, string>> hash = new Dictionary<Dictionary<string, string>, Dictionary<string, string>>();

            Dictionary<string, string> header = new Dictionary<string,string>();
            header.Add("project_name", model.IfcProject.Name);
            header.Add("products_count", products.Count().ToString());

            //Dictionary<string, string> dict = products.ToDictionary(p => "ok", p => p.Description.ToString());
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(header));
            foreach (IfcProduct product in products) {
                //Console.WriteLine(product.Name + " - " + product.Description);
                //Console.Write("\rReading File {0}", product.Name + " - " + product.Description);

                List<IfcPropertySet> sets = product.GetAllPropertySets();
                foreach (IfcPropertySet set in sets)
                {
                    //Console.WriteLine("-" + set.Name);

                    foreach (IfcProperty prop in set.HasProperties)
                    {
                        //Console.WriteLine("--" + prop.Name + "=" + product.GetPropertySingleValue(set.Name, prop.Name));
                        
                    }
                }

                //hash.Add("name", product.Name);
                //hash.Add("description", product.Description);
            }
        }
    }
}
