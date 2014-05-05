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

 
namespace XBIMConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (XbimModel model = new XbimModel())
            {
                string ifcFile = "BCAA_-Function_Hall(Archi+Stru).ifc";
                string ifcLongFile = @"C:\Users\yemaw\gelement\Xbim\XBIMConsole\" + ifcFile;
                string xbimLongFile = System.IO.Path.ChangeExtension(ifcLongFile, "xbim");

                model.CreateFrom(ifcLongFile, xbimLongFile);
                model.Open(xbimLongFile, Xbim.XbimExtensions.XbimDBAccess.ReadWrite);
                Console.WriteLine("----------------------------------------------------");


                IfcProject project = model.IfcProject as IfcProject;
                Console.WriteLine(project.Name + " " + project.LongName);

                IEnumerable<IfcSite> sites = project.GetSites();
                foreach (IfcSite site in sites)
                {
                    Console.WriteLine("-" + site.Name);

                    IEnumerable<IfcBuilding> buildings = site.GetBuildings();
                    foreach (IfcBuilding building in buildings)
                    {
                        Console.WriteLine("--" + building.Name + "(" + building.BuildingAddress + ")");
                        
                        IEnumerable<IfcBuildingStorey> levels = building.GetBuildingStoreys();
                        foreach (IfcBuildingStorey level in levels)
                        {

                            IEnumerable<IfcObjectDefinition> children_l = level.SpatialStructuralElementChildren;

                            IEnumerable<IfcRelDecomposes> decomp = level.IsDecomposedBy;
                            IEnumerable<IfcObjectDefinition> objs = decomp.SelectMany(s => s.RelatedObjects);

                            Console.WriteLine("---" + level.Name + "[Total children=" + children_l.Count()+",decomposed related="+objs.Count()+"]");

                            IEnumerable<IfcProduct> products_l =  level.GetContainedElements();
                            string included_products_l = "";
                            foreach (IfcProduct product in products_l)
                            {
                                included_products_l += product.Name;
                            }
                            Console.WriteLine("products in level=>"+included_products_l);


                            //IfcShapeRepresentation axis = level.GetAxisRepresentation();
                            //IfcShapeRepresentation body = level.GetBodyRepresentation();
                            
                            //IEnumerable<IfcWall> walls = model.Instances.OfType<IfcWall>();

                            /*
                            IEnumerable<IfcRelDefines> defines =  level.IsDefinedBy;
                            foreach (IfcRelDefines define in defines) {
                                IfcObjectSet objects = define.RelatedObjects;
                                foreach(IfcObject o in objects){      
                                    Console.WriteLine(o.Name);
                                }
                            }
                            */

                            /*
                            IEnumerable<IfcRelContainedInSpatialStructure> ss = level.ContainsElements;
                            foreach (IfcRelContainedInSpatialStructure s in ss) {
                                Console.WriteLine(s.Name);
                            }
                            */
                            
                            IEnumerable<IfcSpace> spaces = level.GetSpaces();
                            foreach (IfcSpace space in spaces)
                            {
                                IfcObjectDefinition parent_s = space.SpatialStructuralElementParent;
                                IEnumerable<IfcObjectDefinition> children_s = space.SpatialStructuralElementChildren;
                                //IEnumerable<IfcRelContainedInSpatialStructure> c_structures = space.ContainsElements;

                                Console.WriteLine("----" + space.Name + " " + space.LongName + "["
                                    + " parent name=" + parent_s.Name
                                    + ", parent type=" + parent_s.GetType()
                                    //+ ", spatial structure count=" + c_structures.Count()
                                    + ", net floor area=" + space.GetNetFloorArea()
                                    + "]");

                                /* Products in a space */
                                IEnumerable<IfcProduct> products_s = space.GetContainedElements();
                                string included_products_s = "";
                                foreach (IfcProduct product in products_s)
                                {
                                    included_products_s += product.Name;
                                }
                                //Console.WriteLine("products in space=>"+included_products_s);

                                List<IfcPropertySet> sets = space.GetAllPropertySets();
                                foreach (IfcPropertySet set in sets)
                                {
                                    Console.WriteLine("----->" + set.Name);
                                    foreach (IfcProperty prop in set.HasProperties)
                                    {
                                       // Console.WriteLine("-------" + prop.Name + "="+space.GetPropertySingleValue(set.Name, prop.Name));
                                    }
                                }


                            }
                        }
                    }

                }
                
                //IEnumerable<IfcWall> walls = model.Instances.OfType<IfcWall>();
                //Console.WriteLine(walls.Count());

            }
                
            Console.WriteLine("----------------------------------------------------");
            Console.ReadLine();
        }
    }
}


/*
                //OBTER TODAS AS ENTIDA DES DO MODELO
                //-IfcProject project = model.IfcProject as IfcProject;
                //-IEnumerable<IfcProduct> products = model.IfcProducts.Cast<IfcProduct>();

                //IEnumerable<IfcBuildingStorey>
                var pisos = model.Instances.OfType<IfcBuildingStorey>();
                
                foreach (var piso in pisos)
                {
                    IEnumerable<IfcRelDecomposes> decomp = piso.IsDecomposedBy;
                    IEnumerable<IfcObjectDefinition> objs = decomp.SelectMany(s => s.RelatedObjects);
                    IEnumerable<IfcSpace> spaces = objs.OfType<IfcSpace>();
                    foreach (var space in spaces)
                    {
                        var area = space.GetGrossFloorArea();
                        //Console.WriteLine("Space: {0} Area: {1}\r\n", space.Name, area.Value);
                    }
                }
                model.Close(); 
                 */


/*
if (File.Exists(filesName))
{
    Console.WriteLine("file found");
    using (XbimModel model = new XbimModel())
    {
                    
        string xbimFile = Path.ChangeExtension(filesName, "xBIM");
        if (Path.GetExtension(filesName).ToLower() == ".xbim")
        {
            xbimFile = filesName;
        }
        else
        {
            model.CreateFrom(filesName, xbimFile, delegate(int percentProgress, object userState)
            {
                Console.Write("\rReading File {0}", percentProgress);
            }
        );
        }
        model.Open(xbimFile, XbimDBAccess.ReadWrite);
                    
        //model.CreateFrom(filesName);
        //model.Open(filesName+".xBIM", Xbim.XbimExtensions.XbimDBAccess.ReadWrite);
        Console.WriteLine("");
        Console.WriteLine(model.Instances.Count + " instances read");
                    
                    
                    
        using (Xbim.IO.XbimReadWriteTransaction txn = model.BeginTransaction())
        {
            var door = model.Instances.New<Xbim.Ifc2x3.SharedBldgElements.IfcDoor>();
            door.Name = "New Door";
            txn.Commit();
            model.SaveAs("BIM Logo with Door", Xbim.XbimExtensions.Interfaces.XbimStorageType.IFC);
        }
        Console.WriteLine(model.DefaultOwningApplication.ApplicationIdentifier.ToString());
        model.Close();

    }
}
*/
