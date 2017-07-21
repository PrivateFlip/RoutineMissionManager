using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialOfferings
{
    public static class RmmUtil
    {
        public static string GamePath
        {
            get { return KSPUtil.ApplicationRootPath; }
        }
        public static string CommercialOfferingsPath
        {
            get { return "GameData" + Path.DirectorySeparatorChar + "RoutineMissionManager" + Path.DirectorySeparatorChar; }
        }
        public static System.Random Rand = new System.Random();

        public static double VesselOrbitAltitude(Vessel vessel)
        {
            return ((vessel.orbit.semiMajorAxis - vessel.mainBody.Radius) / 1000);
        }

        public static bool IsPreLaunch(Vessel ves)
        {
            if (ves.situation == Vessel.Situations.PRELAUNCH ||
                (ves.situation == Vessel.Situations.LANDED &&
                 (ves.landedAt == "KSC_LaunchPad_Platform" ||
                  ves.landedAt == "Runway")))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public static int CountVesselParts(Vessel ves)
        {
            int count = 0;
            foreach (Part p in ves.parts)
            {
                count = count + 1;
            }
            return (count);
        }

        public static bool CheckDocked(Vessel vessel, Part part)
        {
            ModuleDockingNode dockingPort = part.Modules.OfType<ModuleDockingNode>().FirstOrDefault();

            //this port is docked
            if (dockingPort.state.Length >= 6 && dockingPort.state.Substring(0, 6) == "Docked" && null != dockingPort.vesselInfo.name)
                return (true);

            //no joined port filled
            if (dockingPort.dockedPartUId == 0) { return (false); }

            //find joined port
            foreach (Part p in vessel.parts)
            {
                if (p.flightID == dockingPort.dockedPartUId)
                {
                    foreach (PartModule pm in p.Modules)
                    {
                        if (pm.ClassName == "ModuleDockingNode")
                        {
                            ModuleDockingNode joinedDockingPort = p.Modules.OfType<ModuleDockingNode>().FirstOrDefault();
                            if (part.flightID == joinedDockingPort.dockedPartUId)
                            {
                                if (joinedDockingPort.state.Length >= 6 && joinedDockingPort.state.Substring(0, 6) == "Docked" && null != joinedDockingPort.vesselInfo.name)
                                    return (true);
                            }
                        }
                    }
                }
            }
            return (false);
        }

        public static bool CheckDocking(Part part)
        {
            ModuleDockingNode dockingPort = part.Modules.OfType<ModuleDockingNode>().FirstOrDefault();
            if (dockingPort.state.Length >= 7 && dockingPort.state.Substring(0, 7) == "Acquire")
                return (true);
            else
                return (false);
        }

        public static bool ForeignDockingPorts(Vessel ves, string trackID)
        {
            foreach (Part p in ves.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleDockingNode")
                    {
                        RMMModule ComOffMod = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        if (ComOffMod.trackID != trackID)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        

        #region Crew

        public static int CrewCount(Vessel ves)
        {
            int crew = 0;

            foreach (Part p in ves.parts)
            {
                if (p.protoModuleCrew.Count > 0)
                {
                    crew = crew + p.protoModuleCrew.Count;
                }
            }

            return (crew);
        }

        public static int AstronautCrewCount(Vessel ves)
        {
            int crew = 0;

            foreach (Part p in ves.parts)
            {
                if (p.protoModuleCrew.Count > 0)
                {
                    foreach (ProtoCrewMember cr in p.protoModuleCrew)
                    {
                        if (cr.type == ProtoCrewMember.KerbalType.Crew)
                        {
                            crew++;
                        }

                    }
                }
            }

            return (crew);
        }

        public static int CrewCapacityCount(Vessel ves)
        {
            int capacity = 0;

            foreach (Part p in ves.parts)
            {
                if (p.CrewCapacity > 0)
                {
                    capacity = capacity + p.CrewCapacity;
                }
            }

            return (capacity);
        }

        #endregion Crew

        #region Body

        private static string[] allowedBodies = null;
        private static string[] allowedHomeBodies = null;

        public static bool AllowedBody(string bodyName)
        {
            if (allowedBodies == null)
            {
                allowedBodies = System.IO.File.ReadAllLines(RmmUtil.GamePath + RmmUtil.CommercialOfferingsPath + "AllowedBodies.txt");
            }

            foreach (String allowedBody in allowedBodies)
            {
                if (bodyName == allowedBody.Trim()) { return (true); }
            }

            return (false);
        }

        public static bool HomeBody(string bodyName)
        {
            if (allowedHomeBodies == null)
            {
                allowedHomeBodies = System.IO.File.ReadAllLines(RmmUtil.GamePath + RmmUtil.CommercialOfferingsPath + "HomeBodies.txt");
            }

            foreach (String allowedHomeBody in allowedHomeBodies)
            {
                if (bodyName == allowedHomeBody.Trim()) { return (true); }
            }

            return (false);
        }

        public static string HomeBodyName()
        {
            if (allowedHomeBodies == null)
            {
                allowedHomeBodies = System.IO.File.ReadAllLines(RmmUtil.GamePath + RmmUtil.CommercialOfferingsPath + "HomeBodies.txt");
            }

            foreach (String allowedHomeBody in allowedHomeBodies)
            {
                return allowedHomeBody.Trim();
            }

            return ("Home");
        }

        #endregion Body

        #region Resources

        public static double GetCargoMass(Vessel vessel, string returnResourcesString)
        {
            double cargoMass = 0.0;

            string[] cargoArray = new string[0];
            RmmUtil.GetCargoArray(vessel, returnResourcesString, ref cargoArray);

            foreach (Part p in vessel.parts)
            {
                foreach (PartResource r in p.Resources)
                {
                    if (cargoArray.Contains(r.info.name))
                    {
                        cargoMass = cargoMass + Mass(r.info.name, r.amount);
                    }
                }
            }
            return (cargoMass);
        }

        public static void GetCargoArray(Vessel vessel, string returnResourcesString, ref string[] cargoArray)
        {
            string[] propellantArray = new string[0];
            RmmUtil.GetProppellantArray(returnResourcesString, ref propellantArray);

            foreach (Part p in vessel.parts)
            {
                foreach (PartResource r in p.Resources)
                {
                    if (!propellantArray.Contains(r.info.name) && !cargoArray.Contains(r.info.name) && r.info.name != "ElectricCharge")
                    {
                        Array.Resize(ref cargoArray, cargoArray.Length + 1);
                        cargoArray[cargoArray.Length - 1] = r.info.name;
                    }
                }
            }
        }

        public static void GetProppellantArray(string returnResourcesString, ref string[] propellantArray)
        {
            string[] SplitArray = returnResourcesString.Split(',');

            foreach (String st in SplitArray)
            {
                string[] SplatArray = st.Split(':');
                string resourceName = SplatArray[0].Trim();
                double amount = Convert.ToDouble(SplatArray[1].Trim());

                Array.Resize(ref propellantArray, propellantArray.Length + 1);
                propellantArray[propellantArray.Length - 1] = resourceName;
            }
        }

        public static void DetermineProppellantArray(Vessel vessel, ref string[] propellantArray)
        {
            foreach (Part p in vessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleEngines")
                    {
                        ModuleEngines mer = p.Modules.OfType<ModuleEngines>().FirstOrDefault();
                        foreach (Propellant pr in mer.propellants)
                        {
                            if (!propellantArray.Contains(pr.name) && pr.name != "ElectricCharge")
                            {
                                Array.Resize(ref propellantArray, propellantArray.Length + 1);
                                propellantArray[propellantArray.Length - 1] = pr.name;
                            }
                        }
                    }

                    if (pm.ClassName == "ModuleEnginesFX")
                    {
                        ModuleEnginesFX mefxr = p.Modules.OfType<ModuleEnginesFX>().FirstOrDefault();
                        foreach (Propellant pr in mefxr.propellants)
                        {
                            if (!propellantArray.Contains(pr.name) && pr.name != "ElectricCharge")
                            {
                                Array.Resize(ref propellantArray, propellantArray.Length + 1);
                                propellantArray[propellantArray.Length - 1] = pr.name;
                            }
                        }
                    }

                    if (pm.ClassName == "ModuleRCS")
                    {
                        ModuleRCS mrcs = p.Modules.OfType<ModuleRCS>().FirstOrDefault();
                        if (!propellantArray.Contains(mrcs.resourceName) && mrcs.resourceName != "ElectricCharge")
                        {
                            Array.Resize(ref propellantArray, propellantArray.Length + 1);
                            propellantArray[propellantArray.Length - 1] = mrcs.resourceName;
                        }
                    }

                    if (pm.ClassName == "ModuleRCSFX")
                    {
                        ModuleRCS mrcs = p.Modules.OfType<ModuleRCSFX>().FirstOrDefault();
                        if (!propellantArray.Contains(mrcs.resourceName) && mrcs.resourceName != "ElectricCharge")
                        {
                            Array.Resize(ref propellantArray, propellantArray.Length + 1);
                            propellantArray[propellantArray.Length - 1] = mrcs.resourceName;
                        }
                    }

                    if (pm.ClassName == "ModuleAblator")
                    {
                        ModuleAblator mabl = p.Modules.OfType<ModuleAblator>().FirstOrDefault();
                        if (!propellantArray.Contains(mabl.ablativeResource) && mabl.ablativeResource != "ElectricCharge")
                        {
                            Array.Resize(ref propellantArray, propellantArray.Length + 1);
                            propellantArray[propellantArray.Length - 1] = mabl.ablativeResource;
                        }
                    }
                }
            }
        }

        public static double ReadResource(Vessel vessel, string ResourceName)
        {
            double amountCounted = 0;
            if (vessel.packed && !vessel.loaded)
            {
                return 0;
            }
            else
            {
                foreach (Part p in vessel.parts)
                {
                    foreach (PartResource r in p.Resources)
                    {
                        if (r.resourceName == ResourceName)
                        {
                            amountCounted = amountCounted + r.amount;
                        }
                    }
                }
            }
            return amountCounted;
        }

        public static double Mass(string resourceName, double amount)
        {
            PartResourceDefinition prd = PartResourceLibrary.Instance.GetDefinition(resourceName);
            return (amount * prd.density);
        }
        public static double Cost(string resourceName, double amount)
        {
            PartResourceDefinition prd = PartResourceLibrary.Instance.GetDefinition(resourceName);
            return (amount * prd.unitCost);
        }

        #endregion Resources
    }
}
