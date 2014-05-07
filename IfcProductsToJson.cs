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
/*
 IfcProduct
 -- PropertiesSet
 ---- Dimension
 ---- Constraints
 --
 */ 

namespace XBIMConsole
{
    class IfcProductsToJson
    {
        public void writeProducts(XbimModel model) {
            Dictionary<string, Dictionary<string, string>> hash = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> header = new Dictionary<string, string>();
            List<Dictionary<string, Dictionary<string, string>>> data = new List<Dictionary<string, Dictionary<string, string>>>();

            IEnumerable<IfcProduct> products = model.Instances.OfType<IfcProduct>();
            
            header.Add("project_name", model.IfcProject.Name);
            header.Add("products_count", products.Count().ToString());

            int count = 0;
            foreach (IfcProduct product in products)
            {
                Dictionary<string, Dictionary<string, string>> product_d = new Dictionary<string, Dictionary<string, string>>();
                
                //Properties
                List<IfcPropertySet> sets = product.GetAllPropertySets();
                foreach (IfcPropertySet set in sets)
                {
                    Dictionary<string, string> pset_d = new Dictionary<string, string>();
                    
                    foreach (IfcProperty prop in set.HasProperties)
                    {
                        pset_d.Add(prop.Name.ToString(), product.GetPropertySingleValue(set.Name, prop.Name) + "");
                    }

                    if (product_d.ContainsKey(set.Name.ToString())) //mearge if already exist.
                    { 
                        foreach (KeyValuePair<string, string> entry in pset_d)
                        {
                            if (product_d.ContainsKey(set.Name))
                            {
                                product_d[set.Name][entry.Key] = entry.Value; //replace
                            }
                            else 
                            {
                                product_d[set.Name].Add(entry.Key, entry.Value); //add new
                            }
                            
                        }

                        //pset_d.ToList().ForEach(x => product_d[set.Name].Add(x.Key, x.Value));
                    } 
                    else 
                    {
                        product_d.Add(set.Name.ToString(), pset_d);
                    }
                    
                }

                Console.Write("\rCurrent count - {0}", count);
                data.Insert(count, product_d);
                count++;
                //if (count == 100) { break; }
            }
             
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }
    }
}