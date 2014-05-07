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
using System.Collections.Specialized;
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
            
            Dictionary<string, string> header = new Dictionary<string, string>();
            List<object> data = new List<object>(); //Dictionary<string, Dictionary<string, Dictionary<string, string>>> //old type
            Dictionary<string, object> output = new Dictionary<string, object>();

            IEnumerable<IfcProduct> products = model.Instances.OfType<IfcProduct>();
            int products_counter = 0;
            foreach (IfcProduct product in products)
            {
                Dictionary<string, object> product_d = new Dictionary<string, object>();//Dictionary<string, Dictionary<string, string>>//old type

                //1)Product's Information
                product_d.Add("name", product.Name);
                product_d.Add("ifctype", product.GetType());
                product_d.Add("description", product.Description);
                product_d.Add("global_id", product.GlobalId);

                //2)Product's Summary
                Dictionary<string, object> summaries_d = new Dictionary<string, object>();
                StringCollection summaries = product.SummaryString();
                summaries_d.Add("entity", summaries[0]);
                summaries_d.Add("guid", summaries[1]);
                summaries_d.Add("type", summaries[2]);
                summaries_d.Add("name", summaries[3]);
                product_d.Add("summary", summaries_d);

                //3)Product's Properties
                Dictionary<string, Dictionary<string, string>> props_set_d = new Dictionary<string, Dictionary<string, string>>();
                List<IfcPropertySet> sets = product.GetAllPropertySets();
                foreach (IfcPropertySet set in sets)
                {
                    Dictionary<string, string> props_d = new Dictionary<string, string>();
                    
                    foreach (IfcProperty prop in set.HasProperties)
                    {
                        props_d.Add(prop.Name.ToString(), product.GetPropertySingleValue(set.Name, prop.Name) + "");
                    }

                    if (props_set_d.ContainsKey(set.Name.ToString())) //mearge if already exist.
                    { 
                        foreach (KeyValuePair<string, string> entry in props_d)
                        {//pset_d.ToList().ForEach(x => product_d[set.Name].Add(x.Key, x.Value));//old way.
                            if (props_set_d.ContainsKey(set.Name))
                            {
                                props_set_d[set.Name][entry.Key] = entry.Value; //replace
                            }
                            else 
                            {
                                props_set_d[set.Name].Add(entry.Key, entry.Value); //add new
                            }
                            
                        }
                    } 
                    else 
                    {
                        props_set_d.Add(set.Name.ToString(), props_d);
                    }
                    
                }
                product_d.Add("properties_set", props_set_d);
                
                

                //---------- Insert packed product into ouput body----------------
                data.Insert(products_counter, product_d);
                products_counter++;

                Console.Write("\rCurrent count - {0}", products_counter-1);
                if (products_counter == 10) { break; }
            }
            
            //--------------------- Packing Project in one object ----------------
            header.Add("project_name", model.IfcProject.Name);
            header.Add("project_description", model.IfcProject.Description);
            header.Add("ifcproducts_count", products.Count().ToString());
            
            output.Add("header", header);
            output.Add("data", data);

            string output_str = Newtonsoft.Json.JsonConvert.SerializeObject(output);

            System.IO.File.WriteAllText(@"C:\Users\yemaw\gelement\sync\ifcproducts.txt", output_str);
            //System.IO.File.WriteAllText(Directory.GetCurrentDirectory()+"/ifcproducts.txt", output);
        }
    }
}