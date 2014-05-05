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
                        //building.GetPropertySet("Pset_BuildingCommon");

                        IEnumerable<IfcBuildingStorey> levels = building.GetBuildingStoreys();
                        foreach (IfcBuildingStorey level in levels)
                        {
                            Console.WriteLine("---" + level.Name);

                            IEnumerable<IfcSpace> spaces = level.GetSpaces();
                            foreach (IfcSpace space in spaces)
                            {
                                Console.WriteLine("----" + space.Name + " " + space.LongName + "["

                                    + " net floor area=" + space.GetNetFloorArea()
                                    //+", height=" + space.GetHeight()
                                    //+", ps=" + space.GetAllPropertySets().ToArray().ToString()
                                    + "]");
                                List<IfcPropertySet> sets = space.GetAllPropertySets();
                                foreach (IfcPropertySet set in sets)
                                {
                                    Console.WriteLine("----->" + set.Name);
                                    foreach (IfcProperty prop in set.HasProperties)
                                    {
                                        //Console.WriteLine("-------" + prop.Name + "="+space.GetPropertySingleValue(set.Name, prop.Name));
                                    }
                                }


                            }
                        }
                    }

                }


                IEnumerable<IfcWall> walls = model.Instances.OfType<IfcWall>();
                
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
