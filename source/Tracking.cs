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

namespace CommercialOfferings
{
    public partial class RMMModule : PartModule
    {
        //GUI Tracking
        private static Rect windowPosGUITracking = new Rect(200, 200, 200, 30);

        //tracking variables
        [KSPField(isPersistant = true, guiActive = false)]
        public bool trackingActive = false;
        [KSPField(isPersistant = true, guiActive = false)]
        public bool trackingPrimary = false;

        [KSPField(isPersistant = true, guiActive = false)]
        public string trackID = "";

        [KSPField(isPersistant = true, guiActive = false)]
        public float trackPartCount = 0.0f;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool returnMission = false;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool returnMissionDeparted = false;

        [KSPField(isPersistant = true, guiActive = false)]
        public string trackFolderName = "";
        [KSPField(isPersistant = true, guiActive = false)]
        public string trackName = "";
        [KSPField(isPersistant = true, guiActive = false)]
        public string trackCompanyName = "";
        [KSPField(isPersistant = true, guiActive = false)]
        public string trackVehicleName = "";
        [KSPField(isPersistant = true, guiActive = false)]
        public string trackLaunchSystemName = "";
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackPrice = 0.0f;
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackVehicleReturnFee = 0.0f;
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackMissionStartTime = 0.0f;
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackMissionTime = 0.0f;
        [KSPField(isPersistant = true, guiActive = false)]
        public string trackBody = "";
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackMaxOrbitAltitude = 0.0f;
        [KSPField(isPersistant = true, guiActive = false)]
        public string trackDescription = "";
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackMinimumCrew = 0.0f;
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackMaximumCrew = 0.0f;
        [KSPField(isPersistant = true, guiActive = false)]
        public bool trackReturnEnabled = false;
        [KSPField(isPersistant = true, guiActive = false)]
        public bool trackSafeReturn = false;
        [KSPField(isPersistant = true, guiActive = false)]
        public string trackReturnResources = "";
        [KSPField(isPersistant = true, guiActive = false)]
        public double trackReturnCargoMass = 0.0;
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackPort = 0.0f;
        [KSPField(isPersistant = true, guiActive = false)]
        public float trackDockingDistance = 0.15f;

        private double landedTime = 0.0;
        private int landedTimeMessage = 0;

        //GUI variables
        private string trackStrName = "";
        private string trackStrVehicleName = "";
        private string trackStrLaunchSystemName = "";


        private bool handleTracking()
        {
            if (trackingActive && trackingPrimary)
            {
                nextLogicTime = Planetarium.GetUniversalTime() + 1;

                if (!returnMission)
                {
                    updatePartCount();
                    updateMinCrewCount();
                    updateMaxCrewCount();
                }

                updateTrackingVars(vessel);

                //check for succesfull docking
                if (checkDocked() && !returnMission && vessel.situation == Vessel.Situations.ORBITING && bodyAllowed())
                {
                    //print("DOCKED");
                    if (foreignDockingPorts(vessel))
                    {
                        trackArrival();
                        return (false);
                    }
                }
                else if (!returnMission && !checkDocking() && vessel.situation == Vessel.Situations.ORBITING && bodyAllowed())
                {
                    //determineDockingPort();
                    takeVesselSnapShot();
                    //return (false);
                }

                if (returnMission)
                {
                    if (!checkDocked() && !returnMissionDeparted)
                    {
                        determineReturnResources();
                        if (trackPartCount != countVesselParts(vessel)) { trackAbort(); }
                        returnMissionDeparted = true;
                    }
                }

                if ((foreignDockingPorts(vessel) || countVesselParts(vessel) > trackPartCount) && (!returnMission ||(returnMission && returnMissionDeparted)))
                {
                    trackAbort();
                    return (false);
                }

                if (returnMission)
                {
                    //check for succesfull return
                    if (!trackReturnEnabled && vessel.mainBody.name == "Kerbin" && vessel.situation == Vessel.Situations.SUB_ORBITAL)
                    {
                        //print("SUBORBITAL");
                        trackReturn();
                    }

                    //check for succesfull landing
                    if (!trackSafeReturn && vessel.mainBody.name == "Kerbin" && (vessel.situation == Vessel.Situations.LANDED || vessel.situation == Vessel.Situations.SPLASHED))
                    {
                        if (landedTime == 0.0)
                        {
                            landedTime = Planetarium.GetUniversalTime();
                            landedTimeMessage = 0;
                        }
                        else if (Planetarium.GetUniversalTime() - landedTime > 0.0 && Planetarium.GetUniversalTime() - landedTime < 10)
                        {
                            if (Planetarium.GetUniversalTime() - landedTime > landedTimeMessage + 1.0)
                            {
                                landedTimeMessage = landedTimeMessage + 1;
                                ScreenMessages.PostScreenMessage("landing confirmation " + (10-landedTimeMessage).ToString() + " seconds", 1, ScreenMessageStyle.UPPER_CENTER);
                            }
                        }
                        else if (Planetarium.GetUniversalTime() - landedTime > 10)
                        {
                            //print("LANDING");
                            trackLanding();
                        }


                    }
                    else
                    {
                        landedTime = 0.0;
                    }
                }
                return false;
            }
            if (trackingPrimary && returnMission && !trackingActive)
            {
                nextLogicTime = Planetarium.GetUniversalTime() + 1;
                if (!checkDocked())
                {
                    trackAbort();
                }
                return false;
            }

            if (trackingActive && !trackingPrimary && returnMission && returnMissionDeparted)
            {
                nextLogicTime = Planetarium.GetUniversalTime() + 1;

                ensurePrimaryExists();

                return false;
            }
            return true;
        }

        private void updatePartCount()
        {
            int currentCount = countVesselParts(vessel);

            if (currentCount < trackPartCount)
            {
                trackPartCount = currentCount;
            }
        }

        private void updateMinCrewCount()
        {
            int currentCount = astronautCrewCount(vessel);

            if (currentCount < trackMinimumCrew)
            {
                trackMinimumCrew = currentCount;
            }
        }

        private void updateMaxCrewCount()
        {
            int currentCount = crewCapacityCount(vessel);

            if (currentCount < trackMaximumCrew)
            {
                trackMaximumCrew = currentCount;
            }
        }

        private void determineReturnResources()
        {
            string content = "";

            string[] arrResource = new string[0];
            getProppellantArray(ref arrResource);

            foreach (String res in arrResource)
            {
                print(res);
                double amount = 0;

                foreach (Part p in vessel.parts)
                {
                    foreach (PartResource r in p.Resources)
                    {
                        if (r.info.name == res)
                        {
                            amount = amount + r.amount;
                        }
                    }
                }

                if (content == "")
                {
                    content = res + ":" + amount.ToString();
                }
                else
                {
                    content = content + "," + res + ":" + amount.ToString();
                }
                //print("content" + content);
            }
            trackReturnResources = content;
        }

        private void determineReturnCargoMass()
        {
            trackReturnCargoMass = getCargoMass();
            print(trackReturnCargoMass);
        }

        private void takeVesselSnapShot()
        {
            ConfigNode savenode = new ConfigNode();
            vessel.BackupVessel();
            vessel.protoVessel.Save(savenode);
            savenode.Save(GamePath + CommercialOfferingsPath + "/Missions/" + trackFolderName + "/vesselfiletrack");
            vessel.BackupVessel();
        }

        private void cleanVesselSnapShot()
        {
            string[] line = System.IO.File.ReadAllLines(GamePath + CommercialOfferingsPath + "/Missions/" + trackFolderName + "/vesselfiletrack");

//            bool inpart = false;
//            //bool inmodule = false;
//            int bracket = 0;
//            
//            //clean crew 
//            for (int i = 0; i < line.Length; i++)
//            {
//                if (line[i].Trim().Length >= 4 && line[i].Trim().Substring(0, 4) == "PART" && bracket == 0) { inpart = true; }
//                //if (line[i].Trim().Length >= 6 && line[i].Trim().Substring(0, 6) == "MODULE" && bracket == 1) { inmodule = true; }
//
//                if (line[i].Trim().Length >= 1 && line[i].Trim().Substring(0, 1) == "{") { bracket++; }
//                
//                if (line[i].Trim().Length >= 1 && line[i].Trim().Substring(0, 1) == "}") 
//                {
//                    if (bracket == 1) { inpart = false; }
//                    bracket--;
//                }
//
//                if (inpart == true && bracket == 1 && line[i].Trim().Length >= 6 && line[i].Trim().Substring(0, 6) == "crew =") { line[i] = ""; }
//
//                //if (inpart == true && inmodule == true && bracket == 2 && line[i].Trim().Length >= 9 && line[i].Trim().Substring(0, 9) == "trackID =") { line[i] = "trackID = 0"; }
//            }

            System.IO.File.WriteAllLines(GamePath + CommercialOfferingsPath + "/Missions/" + trackFolderName + "/vesselfile", line);
        }

        private void updateTrackingVars(Vessel ves)
        {
            foreach (Part p in ves.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleDockingNode" && part.flightID != p.flightID)
                    {
                        RMMModule ComOffMod = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        if (ComOffMod.trackID == trackID)
                        {
                            ComOffMod.trackingActive = trackingActive;
                            ComOffMod.trackPartCount = trackPartCount;
                            ComOffMod.returnMission = returnMission;
                            ComOffMod.returnMissionDeparted = returnMissionDeparted;
                            ComOffMod.trackFolderName = trackFolderName;
                            ComOffMod.trackName = trackName;
                            ComOffMod.trackCompanyName = trackCompanyName;
                            ComOffMod.trackVehicleName = trackVehicleName;
                            ComOffMod.trackLaunchSystemName = trackLaunchSystemName;
                            ComOffMod.trackPrice = trackPrice;
                            ComOffMod.trackVehicleReturnFee = trackVehicleReturnFee;
                            ComOffMod.trackMissionStartTime = trackMissionStartTime;
                            ComOffMod.trackMissionTime = trackMissionTime;
                            ComOffMod.trackBody = trackBody;
                            ComOffMod.trackMaxOrbitAltitude = trackMaxOrbitAltitude;
                            ComOffMod.trackDescription = trackDescription;
                            ComOffMod.trackMinimumCrew = trackMinimumCrew;
                            ComOffMod.trackMaximumCrew = trackMaximumCrew;
                            ComOffMod.trackReturnEnabled = trackReturnEnabled;
                            ComOffMod.trackSafeReturn = trackSafeReturn;
                            ComOffMod.trackReturnResources = trackReturnResources;
                            ComOffMod.trackReturnCargoMass = trackReturnCargoMass;
                            ComOffMod.trackPort = trackPort;
                            ComOffMod.trackDockingDistance = trackDockingDistance;
                        }
                    }
                }
            }
        }

        private void updateNextLogicTime(Vessel ves)
        {
            foreach (Part p in ves.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleDockingNode" && part.flightID != p.flightID)
                    {
                        RMMModule ComOffMod = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        if (ComOffMod.trackID == trackID)
                        {
                            ComOffMod.nextLogicTime = nextLogicTime;
                        }
                    }
                }
            }
        }

        private void ensurePrimaryExists()
        {
            foreach (Part p in vessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleDockingNode")
                    {
                        RMMModule ComOffMod = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        if (ComOffMod.trackID == trackID && ComOffMod.trackingPrimary)
                        {
                            return;
                        }
                    }
                }
            }

            foreach (Part p in vessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleDockingNode")
                    {
                        RMMModule ComOffMod = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        if (ComOffMod.trackID == trackID)
                        {
                            if (p.flightID > part.flightID) { return; }
                        }
                    }
                }
            }

            trackingPrimary = true;
        }

        public bool foreignDockingPorts(Vessel ves)
        {
            foreach (Part p in ves.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleDockingNode")
                    {
                        //print("aaa");
                        RMMModule ComOffMod = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        //print(ComOffMod.trackID);
                        if (ComOffMod.trackID != trackID)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void trackArrival()
        {
            //print("ARRIVAL");

            trackMissionTime = (float)((Planetarium.GetUniversalTime() - trackMissionStartTime) + 21600);

            trackBody = vessel.mainBody.name;

            if (vessel.mainBody.name == "Kerbin")
            {
                trackMaxOrbitAltitude = (float)(vesselOrbitAltitude() * 1.2);
            }
            else
            {
                trackMaxOrbitAltitude = 0;
            }

            cleanVesselSnapShot();

            PortCode = "";
            trackingActive = false;
            returnMission = true;

            createMissionFile();
            ScreenMessages.PostScreenMessage("mission tracking-ARRIVAL", 4, ScreenMessageStyle.UPPER_CENTER);
            updateTrackingVars(vessel);
        }

        private void trackReturn()
        {
            trackReturnEnabled = true;

            createMissionFile();
            ScreenMessages.PostScreenMessage("mission tracking-REENTRY", 4, ScreenMessageStyle.UPPER_CENTER);
            updateTrackingVars(vessel);
        }

        private void trackLanding()
        {
            if (trackMinimumCrew <= astronautCrewCount(vessel) || trackMaximumCrew <= crewCapacityCount(vessel) )
            {
                determineReturnCargoMass();
                trackSafeReturn = true;
                trackVehicleReturnFee = (CalculateVesselPrice(false) * 0.9f);
            }

            createMissionFile();
            trackingActive = false;
            ScreenMessages.PostScreenMessage("mission tracking-LANDING", 4, ScreenMessageStyle.UPPER_CENTER);
            updateTrackingVars(vessel);
        }

        private void trackAbort()
        {
            trackingActive = false;
            trackingPrimary = false;
            returnMission = false;
            ScreenMessages.PostScreenMessage("mission tracking ended", 4, ScreenMessageStyle.UPPER_CENTER);
            updateTrackingVars(vessel);
        }

        private void createMissionFile()
        {
            string[] data = new string[16];

            data[0] = "Name=" + trackName;
            data[1] = "CompanyName=" + trackCompanyName;
            data[2] = "VehicleName=" + trackVehicleName;
            data[3] = "LaunchSystemName=" + trackLaunchSystemName;
            data[4] = "Price=" + trackPrice.ToString();
            data[5] = "VehicleReturnFee=" + trackVehicleReturnFee.ToString();
            data[6] = "Time=" + trackMissionTime.ToString();
            data[7] = "Body=" + trackBody;
            data[8] = "MaxOrbitAltitude=" + trackMaxOrbitAltitude.ToString();
            data[9] = "MinimumCrew=" + trackMinimumCrew.ToString();
            data[10] = "MaximumCrew=" + trackMaximumCrew.ToString();
            data[11] = "ReturnEnabled=" + trackReturnEnabled.ToString();
            data[12] = "SafeReturn=" + trackSafeReturn.ToString();
            data[13] = "ReturnResources=" + trackReturnResources;
            data[14] = "ReturnCargoMass=" + trackReturnCargoMass.ToString();
            data[15] = "DockingDistance=0.15";

            System.IO.File.WriteAllLines(GamePath + CommercialOfferingsPath + "/Missions/" + trackFolderName + "/info.txt", data);

        }


        private void debugFile()
        {
            string[] data = new string[17];

            data[0] = "Name=" + trackName;
            data[1] = "CompanyName=" + trackCompanyName;
            data[2] = "VehicleName=" + trackVehicleName;
            data[3] = "LaunchSystemName=" + trackLaunchSystemName;
            data[4] = "Price=" + trackPrice.ToString();
            data[5] = "VehicleReturnFee=" + trackVehicleReturnFee.ToString();
            data[6] = "Time=" + trackMissionTime.ToString();
            data[7] = "Body=" + trackBody;
            data[8] = "MaxOrbitAltitude=" + trackMaxOrbitAltitude.ToString();
            data[9] = "Partcount" + trackPartCount;
            data[10] = "MinimumCrew=" + trackMinimumCrew.ToString();
            data[11] = "MaximumCrew=" + trackMaximumCrew.ToString();
            data[12] = "ReturnEnabled=" + trackReturnEnabled.ToString();
            data[13] = "SafeReturn=" + trackSafeReturn.ToString();
            data[14] = "ReturnResources=" + trackReturnResources;
            data[15] = "Port=" + trackPort.ToString();
            data[16] = "DockingDistance=0.15";
        }

        int trackWinMode = -1;
        /// <summary>
        /// GUI Tracking
        /// </summary>
        /// <param name="windowID"></param>
        private void WindowGUITracking(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 200, 30));
            GUILayout.BeginVertical();

            if (vessel.situation == Vessel.Situations.PRELAUNCH && !trackingActive)
            {
                if (trackWinMode != 0)
                {
                    windowPosGUITracking = new Rect(200, 200, 300, 100);
                    trackWinMode = 0;
                }
                createMissionForm();
            }
            else if (trackingActive)
            {
                if (trackWinMode != 1)
                {
                    windowPosGUITracking = new Rect(0, 400, 200, 50);
                    trackWinMode = 1;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("tracking", labelStyle, GUILayout.Width(60));
                if (GUILayout.Button("stop tracking", buttonStyle, GUILayout.Width(100), GUILayout.Height(22)))
                {
                    trackAbort();
                }
                GUILayout.EndHorizontal();
            }
            else if (returnMission && !trackingActive && checkDocked())
            {
                if (trackWinMode != 2)
                {
                    windowPosGUITracking = new Rect(200, 200, 300, 50);
                    trackWinMode = 2;
                }
                returnMissionForm();
            }
            else
            {
                closeGUITracking();
            }

            GUILayout.EndVertical();
        }

        private void createMissionForm()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mission Name", labelStyle, GUILayout.Width(100));
            trackStrName = GUILayout.TextField(trackStrName, 20, textFieldStyle, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Vehicle:", labelStyle, GUILayout.Width(100));
            trackStrVehicleName = GUILayout.TextField(trackStrVehicleName, 50, textFieldStyle, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Launch System:", labelStyle, GUILayout.Width(100));
            trackStrLaunchSystemName = GUILayout.TextField(trackStrLaunchSystemName, 50, textFieldStyle, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Mission", buttonStyle, GUILayout.Width(200), GUILayout.Height(22)))
            {
                startMission();
            }
            if (GUILayout.Button("Cancel", buttonStyle, GUILayout.Width(100), GUILayout.Height(22)))
            {
                closeGUITracking();
            }
            GUILayout.EndHorizontal();
        }


        private void returnMissionForm()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Return Mission", buttonStyle, GUILayout.Width(200), GUILayout.Height(22)))
            {
                startReturnMission();
            }
            if (GUILayout.Button("Cancel", buttonStyle, GUILayout.Width(100), GUILayout.Height(22)))
            {
                closeGUITracking();
            }
            GUILayout.EndHorizontal();
        }

        private void startMission()
        {
            trackingActive = true;
            trackingPrimary = true;
            trackID = rand.Next(1000000, 9999999).ToString();
            trackPartCount = 99999;
            updatePartCount();
            returnMission = false;
            trackFolderName = trackID;
            trackName = trackStrName;
            trackCompanyName = "KSPCAMPAIGN:::" + HighLogic.SaveFolder;
            trackVehicleName = trackStrVehicleName;
            trackLaunchSystemName = trackStrLaunchSystemName;
            trackPrice = CalculateVesselPrice(true);
            trackVehicleReturnFee = 0;
            trackMissionStartTime = (float)Planetarium.GetUniversalTime();
            trackMissionTime = 0;
            trackBody = "";
            trackMaxOrbitAltitude = 0;
            trackDescription = "";
            trackMinimumCrew = 99999;
            updateMinCrewCount();
            trackMaximumCrew = 99999;
            updateMaxCrewCount();
            trackReturnEnabled = false;
            trackSafeReturn = false;
            trackReturnResources = "";
            trackReturnCargoMass = 0.0;
            trackPort = 0;
            trackDockingDistance = 0.15f;

            Directory.CreateDirectory(GamePath + CommercialOfferingsPath + "/Missions/" + trackFolderName);
            nextLogicTime = Planetarium.GetUniversalTime();
            setOtherModules();
        }


        private void setOtherModules()
        {
            foreach (Part p in vessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleDockingNode")
                    {
                        RMMModule ComOffMod = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        ComOffMod.trackID = trackID;
                    }
                }
            }
        }

        private void setThisPortPrimary()
        {
            foreach (Part p in vessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "ModuleDockingNode")
                    {
                        RMMModule ComOffMod = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        ComOffMod.trackingPrimary = false;
                        ComOffMod.PortCode = "";
                    }
                }
            }
            trackingPrimary = true;
            PortCode = "Mission Port";
        }

        private void startReturnMission()
        {
            trackingActive = true;
            returnMission = true;
            nextLogicTime = Planetarium.GetUniversalTime();
            updateTrackingVars(vessel);
            updateNextLogicTime(vessel);
        }


        private float CalculateVesselPrice(bool withCargo)
        {
            double price = 0.0f;

            string[] propellantResources = new string[0];
            getProppellantArray(ref propellantResources);

            //cost parts
            foreach (Part p in vessel.parts)
            {
                double missingResCost = 0;
                foreach (PartResource r in p.Resources)
                {
                    if (withCargo || propellantResources.Contains(r.info.name))
                    {
                        missingResCost = missingResCost + cost(r.info.name, (r.maxAmount - r.amount));
                    }
                    else
                    {
                        missingResCost = missingResCost + cost(r.info.name, r.maxAmount);
                    }
                }

                price = price + (p.partInfo.cost - missingResCost + part.GetModuleCosts(0));
            }
            return ((float)price);
        }

        private void drawGUITracking()
        {
            windowPosGUITracking = GUILayout.Window(3406, windowPosGUITracking, WindowGUITracking, "Mission Tracking", windowStyle);
        }

        [KSPEvent(name = "tracking", isDefault = false, guiActive = false, guiActiveEditor = true, guiName = "Track Mission")]
        public void tracking()
        {
            if (HighLogic.LoadedScene == GameScenes.EDITOR) 
            {
                if (trackingPrimary)
                {
                    trackingPrimary = false;
                    PortCode = "";
                }
                else
                {
                    trackingPrimary = true;
                    PortCode = "Mission Port";
                }
                
            }
            
            if (HighLogic.LoadedScene == GameScenes.FLIGHT) { openGUITracking(); }
        }



        public void openGUITracking()
        {
            if (!trackingActive && vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                trackStrName = vessel.vesselName;
                trackStrVehicleName = vessel.vesselName;
                trackStrLaunchSystemName = vessel.vesselName;
                setThisPortPrimary();
            }

            closeGUITracking();
            initStyle();
            RenderingManager.AddToPostDrawQueue(346, new Callback(drawGUITracking));
        }

        public void closeGUITracking()
        {
            RenderingManager.RemoveFromPostDrawQueue(346, new Callback(drawGUITracking));
        }
    }
}