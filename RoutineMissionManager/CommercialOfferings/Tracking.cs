using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CommercialOfferings
{
    public class Tracking
    {
        private RMMModule _partModule;

        private double landedTime = 0.0;
        private int landedTimeMessage = 0;

        //GUI Tracking
        private bool renderGUITracking = false;
        private static Rect windowPosGUITracking = new Rect(200, 200, 200, 30);
        //GUI variables
        private string trackStrName = "";
        private string trackStrVehicleName = "";
        private string trackStrLaunchSystemName = "";

        public Tracking(RMMModule partModule)
        {
            _partModule = partModule;
        }

        #region Saved Properties

        private bool trackingActive
        {
            get { return _partModule.trackingActive; }
            set { _partModule.trackingActive = value; }
        }

        private bool trackingPrimary
        {
            get { return _partModule.trackingPrimary; }
            set { _partModule.trackingPrimary = value; }
        }

        private string trackID
        {
            get { return _partModule.trackID; }
            set { _partModule.trackID = value; }
        }

        private float trackPartCount
        {
            get { return _partModule.trackPartCount; }
            set { _partModule.trackPartCount = value; }
        }

        private bool returnMission
        {
            get { return _partModule.returnMission; }
            set { _partModule.returnMission = value; }
        }

        private bool returnMissionDeparted
        {
            get { return _partModule.returnMissionDeparted; }
            set { _partModule.returnMissionDeparted = value; }
        }

        private string trackFolderName
        {
            get { return _partModule.trackFolderName; }
            set { _partModule.trackFolderName = value; }
        }

        private string trackName
        {
            get { return _partModule.trackName; }
            set { _partModule.trackName = value; }
        }

        private string trackCompanyName
        {
            get { return _partModule.trackCompanyName; }
            set { _partModule.trackCompanyName = value; }
        }

        private string trackVehicleName
        {
            get { return _partModule.trackVehicleName; }
            set { _partModule.trackVehicleName = value; }
        }

        private string trackLaunchSystemName
        {
            get { return _partModule.trackLaunchSystemName; }
            set { _partModule.trackLaunchSystemName = value; }
        }

        private float trackPrice
        {
            get { return _partModule.trackPrice; }
            set { _partModule.trackPrice = value; }
        }

        private float trackVehicleReturnFee
        {
            get { return _partModule.trackVehicleReturnFee; }
            set { _partModule.trackVehicleReturnFee = value; }
        }

        private float trackMissionStartTime
        {
            get { return _partModule.trackMissionStartTime; }
            set { _partModule.trackMissionStartTime = value; }
        }

        private float trackMissionTime
        {
            get { return _partModule.trackMissionTime; }
            set { _partModule.trackMissionTime = value; }
        }

        private string trackBody
        {
            get { return _partModule.trackBody; }
            set { _partModule.trackBody = value; }
        }

        private float trackMaxOrbitAltitude
        {
            get { return _partModule.trackMaxOrbitAltitude; }
            set { _partModule.trackMaxOrbitAltitude = value; }
        }

        private string trackDescription
        {
            get { return _partModule.trackDescription; }
            set { _partModule.trackDescription = value; }
        }

        private float trackMinimumCrew
        {
            get { return _partModule.trackMinimumCrew; }
            set { _partModule.trackMinimumCrew = value; }
        }

        private float trackMaximumCrew
        {
            get { return _partModule.trackMaximumCrew; }
            set { _partModule.trackMaximumCrew = value; }
        }

        private bool trackReturnEnabled
        {
            get { return _partModule.trackReturnEnabled; }
            set { _partModule.trackReturnEnabled = value; }
        }

        private bool trackSafeReturn
        {
            get { return _partModule.trackSafeReturn; }
            set { _partModule.trackSafeReturn = value; }
        }

        private string trackReturnResources
        {
            get { return _partModule.trackReturnResources; }
            set { _partModule.trackReturnResources = value; }
        }

        private double trackReturnCargoMass
        {
            get { return _partModule.trackReturnCargoMass; }
            set { _partModule.trackReturnCargoMass = value; }
        }

        private float trackPort
        {
            get { return _partModule.trackPort; }
            set { _partModule.trackPort = value; }
        }

        private float trackDockingDistance
        {
            get { return _partModule.trackDockingDistance; }
            set { _partModule.trackDockingDistance = value; }
        }

        #endregion Saved Properties



        #region Derived Properties

        private Vessel vessel
        {
            get { return _partModule.vessel; }
        }

        private Part part
        {
            get { return _partModule.part; }
        }

        #endregion Derived Properties

        public void DrawGUI()
        {
            //Tracking GUI rendering
            if (renderGUITracking)
            {
                drawGUITracking();
            }
        }


        public bool handleTracking()
        {
            if (trackingActive && trackingPrimary)
            {


                if (!returnMission)
                {
                    updatePartCount();
                    updateMinCrewCount();
                    updateMaxCrewCount();
                }

                updateTrackingVars(vessel);

                //check for succesfull docking

                if (RmmUtil.CheckDocked(vessel, part) && !returnMission && vessel.situation == Vessel.Situations.ORBITING && RmmUtil.AllowedBody(vessel.mainBody.name))
                {
                    if (RmmUtil.ForeignDockingPorts(vessel, trackID))
                    {
                        trackArrival();
                        return (false);
                    }
                }
                else if (!returnMission && !RmmUtil.CheckDocking(part) && vessel.situation == Vessel.Situations.ORBITING && RmmUtil.AllowedBody(vessel.mainBody.name))
                {
                    //determineDockingPort();
                    takeVesselSnapShot();
                    //return (false);
                }

                if (returnMission)
                {
                    if (!RmmUtil.CheckDocked(vessel, part) && !returnMissionDeparted)
                    {
                        determineReturnResources();
                        if (trackPartCount != RmmUtil.CountVesselParts(vessel)) { trackAbort(); }
                        returnMissionDeparted = true;
                    }
                }

                if ((RmmUtil.ForeignDockingPorts(vessel, trackID) || RmmUtil.CountVesselParts(vessel) > trackPartCount) && (!returnMission || (returnMission && returnMissionDeparted)))
                {
                    trackAbort();
                    return (false);
                }

                if (returnMission)
                {
                    //check for succesfull return
                    if (!trackReturnEnabled && RmmUtil.HomeBody(vessel.mainBody.name) && vessel.situation == Vessel.Situations.SUB_ORBITAL)
                    {
                        trackReturn();
                    }

                    //check for succesfull landing
                    if (!trackSafeReturn && RmmUtil.HomeBody(vessel.mainBody.name) && (vessel.situation == Vessel.Situations.LANDED || vessel.situation == Vessel.Situations.SPLASHED))
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
                                ScreenMessages.PostScreenMessage("landing confirmation " + (10 - landedTimeMessage).ToString() + " seconds", 1, ScreenMessageStyle.UPPER_CENTER);
                            }
                        }
                        else if (Planetarium.GetUniversalTime() - landedTime > 10)
                        {
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
                if (!RmmUtil.CheckDocked(vessel, part))
                {
                    trackAbort();
                }
                return false;
            }

            if (trackingActive && !trackingPrimary && returnMission && returnMissionDeparted)
            {
                ensurePrimaryExists();

                return false;
            }
            return true;
        }

        private void updatePartCount()
        {
            int currentCount = RmmUtil.CountVesselParts(vessel);

            if (currentCount < trackPartCount)
            {
                trackPartCount = currentCount;
            }
        }

        private void updateMinCrewCount()
        {
            int currentCount = RmmUtil.AstronautCrewCount(vessel);

            if (currentCount < trackMinimumCrew)
            {
                trackMinimumCrew = currentCount;
            }
        }

        private void updateMaxCrewCount()
        {
            int currentCount = RmmUtil.CrewCapacityCount(vessel);

            if (currentCount < trackMaximumCrew)
            {
                trackMaximumCrew = currentCount;
            }
        }

        private void determineReturnResources()
        {
            string content = "";

            string[] arrResource = new string[0];
            RmmUtil.DetermineProppellantArray(vessel, ref arrResource);

            foreach (String res in arrResource)
            {
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
            }
            trackReturnResources = content;
        }

        private void determineReturnCargoMass()
        {
            trackReturnCargoMass = getTrackingCargoMass();
        }

        private double getTrackingCargoMass()
        {
            double cargoMass = 0.0;

            string[] cargoArray = new string[0];
            RmmUtil.GetCargoArray(vessel, trackReturnResources, ref cargoArray);

            foreach (Part p in vessel.parts)
            {
                foreach (PartResource r in p.Resources)
                {
                    if (cargoArray.Contains(r.info.name))
                    {
                        cargoMass = cargoMass + RmmUtil.Mass(r.info.name, r.amount);
                    }
                }
            }
            return (cargoMass);
        }

        private void takeVesselSnapShot()
        {
            ConfigNode savenode = new ConfigNode();
            vessel.BackupVessel();
            vessel.protoVessel.Save(savenode);
            savenode.Save(RmmUtil.GamePath + RmmUtil.CommercialOfferingsPath + "Missions" + Path.DirectorySeparatorChar + trackFolderName + Path.DirectorySeparatorChar + "vesselfiletrack");
            vessel.BackupVessel();
        }

        private void cleanVesselSnapShot()
        {
            string[] line = System.IO.File.ReadAllLines(RmmUtil.GamePath + RmmUtil.CommercialOfferingsPath + "Missions" + Path.DirectorySeparatorChar + trackFolderName + Path.DirectorySeparatorChar + "vesselfiletrack");

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

            System.IO.File.WriteAllLines(RmmUtil.GamePath + RmmUtil.CommercialOfferingsPath + "Missions" + Path.DirectorySeparatorChar + trackFolderName + Path.DirectorySeparatorChar + "vesselfile", line);
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
                            ComOffMod.nextLogicTime = _partModule.nextLogicTime;
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



        private void trackArrival()
        {
            trackMissionTime = (float)((Planetarium.GetUniversalTime() - trackMissionStartTime) + 21600);

            trackBody = vessel.mainBody.name;

            if (RmmUtil.HomeBody(vessel.mainBody.name))
            {
                trackMaxOrbitAltitude = (float)(RmmUtil.VesselOrbitAltitude(vessel) * 1.2);
            }
            else
            {
                trackMaxOrbitAltitude = 0;
            }

            cleanVesselSnapShot();

            _partModule.PortCode = "";
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
            if (trackMinimumCrew <= RmmUtil.AstronautCrewCount(vessel) || trackMaximumCrew <= RmmUtil.CrewCapacityCount(vessel))
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

            System.IO.File.WriteAllLines(RmmUtil.GamePath + RmmUtil.CommercialOfferingsPath + "Missions" + Path.DirectorySeparatorChar + trackFolderName + Path.DirectorySeparatorChar + "info.txt", data);

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

            if (!trackingActive && RmmUtil.IsPreLaunch(vessel))
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
                GUILayout.Label("tracking", RmmStyle.Instance.LabelStyle, GUILayout.Width(60));
                if (GUILayout.Button("stop tracking", RmmStyle.Instance.ButtonStyle, GUILayout.Width(100), GUILayout.Height(22)))
                {
                    trackAbort();
                }
                GUILayout.EndHorizontal();
            }
            else if (returnMission && !trackingActive && RmmUtil.CheckDocked(vessel, part))
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
                CloseGUITracking();
            }

            GUILayout.EndVertical();
        }

        private void createMissionForm()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mission Name", RmmStyle.Instance.LabelStyle, GUILayout.Width(100));
            trackStrName = GUILayout.TextField(trackStrName, 20, RmmStyle.Instance.TextFieldStyle, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Vehicle:", RmmStyle.Instance.LabelStyle, GUILayout.Width(100));
            trackStrVehicleName = GUILayout.TextField(trackStrVehicleName, 50, RmmStyle.Instance.TextFieldStyle, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Launch System:", RmmStyle.Instance.LabelStyle, GUILayout.Width(100));
            trackStrLaunchSystemName = GUILayout.TextField(trackStrLaunchSystemName, 50, RmmStyle.Instance.TextFieldStyle, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Mission", RmmStyle.Instance.ButtonStyle, GUILayout.Width(200), GUILayout.Height(22)))
            {
                startMission();
            }
            if (GUILayout.Button("Cancel", RmmStyle.Instance.ButtonStyle, GUILayout.Width(100), GUILayout.Height(22)))
            {
                CloseGUITracking();
            }
            GUILayout.EndHorizontal();
        }


        private void returnMissionForm()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Return Mission", RmmStyle.Instance.ButtonStyle, GUILayout.Width(200), GUILayout.Height(22)))
            {
                startReturnMission();
            }
            if (GUILayout.Button("Cancel", RmmStyle.Instance.ButtonStyle, GUILayout.Width(100), GUILayout.Height(22)))
            {
                CloseGUITracking();
            }
            GUILayout.EndHorizontal();
        }

        private void startMission()
        {
            trackingActive = true;
            trackingPrimary = true;
            trackID = RmmUtil.Rand.Next(1000000, 9999999).ToString();
            trackPartCount = 99999;
            updatePartCount();
            returnMission = false;
            trackFolderName = trackID;
            trackName = trackStrName;
            trackCompanyName = "KSPCAMPAIGN:::" + HighLogic.SaveFolder;
            trackVehicleName = trackStrVehicleName;
            trackLaunchSystemName = trackStrLaunchSystemName;
            trackReturnResources = "";
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

            Directory.CreateDirectory(RmmUtil.GamePath + RmmUtil.CommercialOfferingsPath + "Missions" + Path.DirectorySeparatorChar + trackFolderName);
            _partModule.nextLogicTime = Planetarium.GetUniversalTime();
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
            _partModule.PortCode = "Mission Port";
        }

        private void startReturnMission()
        {
            trackingActive = true;
            returnMission = true;
            _partModule.nextLogicTime = Planetarium.GetUniversalTime();
            updateTrackingVars(vessel);
            updateNextLogicTime(vessel);
        }


        private float CalculateVesselPrice(bool withCargo)
        {
            double price = 0.0f;

            string[] propellantResources = new string[0];

            if (trackReturnResources == "")
            {
                RmmUtil.DetermineProppellantArray(vessel, ref propellantResources);
            }
            else
            {
                RmmUtil.GetProppellantArray(trackReturnResources, ref propellantResources);
            }
            //cost parts
            foreach (Part p in vessel.parts)
            {
                double missingResCost = 0;
                foreach (PartResource r in p.Resources)
                {
                    if (withCargo || propellantResources.Contains(r.info.name))
                    {
                        missingResCost = missingResCost + RmmUtil.Cost(r.info.name, (r.maxAmount - r.amount));
                    }
                    else
                    {
                        missingResCost = missingResCost + RmmUtil.Cost(r.info.name, r.maxAmount);
                    }
                }

                price = price + (p.partInfo.cost - missingResCost + part.GetModuleCosts(0));
            }
            return ((float)price);
        }

        public void drawGUITracking()
        {
            windowPosGUITracking = GUILayout.Window(3406, windowPosGUITracking, WindowGUITracking, "Mission Tracking", RmmStyle.Instance.WindowStyle);
        }

        [KSPEvent(name = "tracking", isDefault = false, guiActive = false, guiActiveEditor = true, guiName = "Track Mission")]
        public void tracking()
        {
            if (HighLogic.LoadedScene == GameScenes.EDITOR)
            {
                if (trackingPrimary)
                {
                    trackingPrimary = false;
                    _partModule.PortCode = "";
                }
                else
                {
                    trackingPrimary = true;
                    _partModule.PortCode = "Mission Port";
                }

            }

            if (HighLogic.LoadedScene == GameScenes.FLIGHT) { OpenGUITracking(); }
        }

        public void OpenGUITracking()
        {
            if (!trackingActive && RmmUtil.IsPreLaunch(vessel))
            {
                trackStrName = vessel.vesselName;
                trackStrVehicleName = vessel.vesselName;
                trackStrLaunchSystemName = vessel.vesselName;
                setThisPortPrimary();
            }

            renderGUITracking = true;
        }

        public void CloseGUITracking()
        {
            renderGUITracking = false;
        }
    }
}
