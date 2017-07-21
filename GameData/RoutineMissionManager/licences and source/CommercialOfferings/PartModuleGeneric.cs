/*Copyright (c) 2014, Flip van Toly
 All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are 
permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of 
conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of 
conditions and the following disclaimer in the documentation and/or other materials provided with 
the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to 
endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
OF SUCH DAMAGE.*/

//Namespace Declaration 
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;


namespace CommercialOfferings
{
    public partial class RMMModule : PartModule
    {
        string[] allowedBodies = null;
        string[] allowedHomeBodies = null;

        //Resources 

        public double mass(string resourceName, double amount)
        {
            PartResourceDefinition prd = PartResourceLibrary.Instance.GetDefinition(resourceName);
            return (amount * prd.density);
        }
        public double cost(string resourceName, double amount)
        {
            PartResourceDefinition prd = PartResourceLibrary.Instance.GetDefinition(resourceName);
            return (amount * prd.unitCost);
        }

        private double getCargoMass(string returnResourcesString)
        {
            double cargoMass = 0.0;

            string[] cargoArray = new string[0];
            getCargoArray(returnResourcesString, ref cargoArray);

            foreach (Part p in vessel.parts)
            {
                foreach (PartResource r in p.Resources)
                {
                    if (cargoArray.Contains(r.info.name))
                    {
                        cargoMass = cargoMass + mass(r.info.name, r.amount);
                    }
                }
            }
            return (cargoMass);
        }

        private void getCargoArray(string returnResourcesString, ref string[] cargoArray)
        {
            string[] propellantArray = new string[0];
            getProppellantArray(returnResourcesString, ref propellantArray);

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

        private void getProppellantArray(string returnResourcesString, ref string[] propellantArray)
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

        private void determineProppellantArray(ref string[] propellantArray)
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

        private double readResource(Vessel ves, string ResourceName)
        {
            double amountCounted = 0;
            if (ves.packed && !ves.loaded)
            {
                return 0;
            }
            else
            {
                foreach (Part p in ves.parts)
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

        //Crew

        private int crewCount(Vessel ves)
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

        private int astronautCrewCount(Vessel ves)
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

        private int crewCapacityCount(Vessel ves)
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

        public bool AllowedBody()
        {
            return AllowedBody(vessel.mainBody.name);
        }

        public bool AllowedBody(string bodyName)
        {
            if (allowedBodies == null)
            {
                allowedBodies = System.IO.File.ReadAllLines(GamePath + CommercialOfferingsPath + "AllowedBodies.txt");
            }

            foreach (String allowedBody in allowedBodies)
            {
                if (vessel.mainBody.name == allowedBody.Trim()) { return(true); }
            }

            return(false);
        }

        public bool HomeBody()
        {
            return HomeBody(vessel.mainBody.name);
        }

        public bool HomeBody(string bodyName)
        {
            if (allowedHomeBodies == null)
            {
                allowedHomeBodies = System.IO.File.ReadAllLines(GamePath + CommercialOfferingsPath + "HomeBodies.txt");
            }

            foreach (String allowedHomeBody in allowedHomeBodies)
            {
                if (bodyName == allowedHomeBody.Trim()) { return (true); }
            }

            return (false);
        }

        public string HomeBodyName()
        {
            if (allowedHomeBodies == null)
            {
                allowedHomeBodies = System.IO.File.ReadAllLines(GamePath + CommercialOfferingsPath + "HomeBodies.txt");
            }

            foreach (String allowedHomeBody in allowedHomeBodies)
            {
                return allowedHomeBody.Trim();
            }

            return ("Home");
        }

        private static void CompleteContractParameter(object objInstance)
        {
            System.Reflection.MethodInfo m;
            try
            {
                m = objInstance.GetType().GetMethod("SetComplete", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (ReferenceEquals(m, null))
                {
                    print("There is no method '" +
                     "SetComplete" + "' for type '" + objInstance.GetType().ToString() + "'.");
                    return;
                }

                object objRet = m.Invoke(objInstance, null);
            }
            catch
            {
                throw;
            }
        }

//        private static object CompleteContract(System.Type t, string
//strMethod, object objInstance, object[] aobjParams, BindingFlags eFlags)
//        {
//            MethodInfo m;
//            try
//            {
//                m = t.GetMethod(", eFlags);
//                if (m == null)
//                {
//                    throw new ArgumentException("There is no method '" +
//                     strMethod + "' for type '" + t.ToString() + "'.");
//                }
//
//                object objRet = m.Invoke(objInstance, aobjParams);
//                return objRet;
//            }
//            catch
//            {
//                throw;
//            }
//        } //end of method

    }
}