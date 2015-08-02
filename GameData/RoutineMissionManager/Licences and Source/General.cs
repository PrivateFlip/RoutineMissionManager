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

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class RMMGeneral : MonoBehaviour
    {
        System.IO.DirectoryInfo GamePath;
        string CommercialOfferingsPath = "/GameData/RoutineMissionManager/";

        //stock toolbar button
        private static ApplicationLauncherButton toolBarButton;
        private static Texture2D toolBarButtonTexture;

        private double lastUpdateToolBarTime = 0.0f;

        string[] allowedBodies = null;

        //GUI
        private static GUIStyle windowStyle, labelStyle, labelTextStyle, redlabelStyle, textFieldStyle, buttonStyle, horiScrollBarStyle, vertiScrollBarStyle;

        //GUI Location
        private static Rect windowPosGUILocation = new Rect(450, 200, 350, 30);
        private Vector2 scrollPositionModules;
        private RMMModule rmmmSelectGUI = null;

        //GUI TermsCondi
        private static Rect windowPosGUITermsCondi = new Rect(400, 250, 250, 300);
        private Vector2 scrollPositionDisc;

        List<RMMModule> RegisteredModuleList = new List<RMMModule>();

        public void Awake()
        {
            GamePath = System.IO.Directory.GetParent(Application.dataPath);
            RenderingManager.AddToPostDrawQueue(754, OnDraw);
        }

        void onDestroy()
        {
            if (toolBarButton != null) { removeToolbarButton(); }
        }

        private void OnDraw()
        {
            updateToolBar();
        }

        private void updateToolBar()
        {

            if (HighLogic.LoadedScene != GameScenes.FLIGHT || (lastUpdateToolBarTime + 3) > Planetarium.GetUniversalTime() || !ApplicationLauncher.Ready) { return; }

            if (toolBarButton == null && toolBarButtonVisible()) { addToolbarButton(); }
            if (toolBarButton != null && !toolBarButtonVisible()) { removeToolbarButton(); }

            lastUpdateToolBarTime = Planetarium.GetUniversalTime();
        }

        private void addToolbarButton()
        {
            toolBarButtonTexture = GameDatabase.Instance.GetTexture("RoutineMissionManager/Textures/RMMbutton", false);

            toolBarButton = ApplicationLauncher.Instance.AddModApplication(
                                onToggleOn,
                                onToggleOff,
                                null,
                                null,
                                null,
                                null,
                                ApplicationLauncher.AppScenes.FLIGHT,
                                toolBarButtonTexture);
        }

        private void removeToolbarButton()
        {
            ApplicationLauncher.Instance.RemoveModApplication(toolBarButton);
            toolBarButton = null;
        }

        void onToggleOn()
        {
            if (FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH) { startTracking();}

            if (trackingActive()) { startTracking(); return; }

            if (FlightGlobals.ActiveVessel.situation == Vessel.Situations.ORBITING &&
                (FlightGlobals.ActiveVessel.mainBody.name == "Kerbin" || FlightGlobals.ActiveVessel.mainBody.name == "Mun" || FlightGlobals.ActiveVessel.mainBody.name == "Minmus"))
            {
                startRoutine();
            }
        }

        void onToggleOff()
        {
            closeGUILocation();
            foreach (Part p in FlightGlobals.ActiveVessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "RMMModule")
                    {
                        RMMModule aRMMModule;
                        aRMMModule = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        aRMMModule.closeGUITracking();
                    }
                }
            }           
        }

        private bool toolBarButtonVisible()
        {
            if (FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH ||
                (FlightGlobals.ActiveVessel.situation == Vessel.Situations.ORBITING &&
                 bodyAllowed() &&
                 hasDockingPorts()) ||
                trackingActive())
                return (true);
            
            return (false);
        }
        
        private void startTracking()
        {
            int count = 0;
            RMMModule aRMMModule = null;
        
            foreach (Part p in FlightGlobals.ActiveVessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "RMMModule")
                    {
                        count++;
                        aRMMModule = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        if (aRMMModule.trackingPrimary) { aRMMModule.openGUITracking(); return; }
                    }
                }
            }

            if (count == 1) { aRMMModule.openGUITracking(); return; }
        }

        private void startRoutine()
        {
            if (!File.Exists(GamePath + CommercialOfferingsPath + "UserAcknowledgesKnownBugs")) { openGUITermsCondi(); return; }
            openGUILocation();
        }

        private bool trackingActive()
        {
            foreach (Part p in FlightGlobals.ActiveVessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "RMMModule")
                    {
                        RMMModule aRMMModule;
                        aRMMModule = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        if (aRMMModule.trackingActive && aRMMModule.trackingPrimary && !aRMMModule.foreignDockingPorts(p.vessel)) { return true; }
                    }
                }
            }
            return false;
        }

        private bool hasDockingPorts()
        {
            foreach (Part p in FlightGlobals.ActiveVessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "RMMModule")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool bodyAllowed()
        {
            if (allowedBodies == null)
            {
                allowedBodies = System.IO.File.ReadAllLines(GamePath + CommercialOfferingsPath + "AllowedBodies.txt");
            }

            foreach (String allowedBody in allowedBodies)
            {
                if (FlightGlobals.ActiveVessel.mainBody.name == allowedBody.Trim()) { return (true); }
            }

            return (false);
        }

        private void initStyle()
        {
            windowStyle = new GUIStyle(HighLogic.Skin.window);
            windowStyle.stretchWidth = false;
            windowStyle.stretchHeight = false;

            labelStyle = new GUIStyle(HighLogic.Skin.label);
            labelStyle.stretchWidth = false;
            labelStyle.stretchHeight = false;

            labelTextStyle = new GUIStyle(HighLogic.Skin.label);
            labelTextStyle.stretchWidth = false;
            labelTextStyle.stretchHeight = true;
            labelTextStyle.wordWrap = true;

            redlabelStyle = new GUIStyle(HighLogic.Skin.label);
            redlabelStyle.stretchWidth = false;
            redlabelStyle.stretchHeight = false;
            redlabelStyle.normal.textColor = Color.red;

            textFieldStyle = new GUIStyle(HighLogic.Skin.textField);
            textFieldStyle.stretchWidth = false;
            textFieldStyle.stretchHeight = false;

            buttonStyle = new GUIStyle(HighLogic.Skin.button);
            buttonStyle.stretchHeight = false;
            buttonStyle.stretchWidth = false;

            horiScrollBarStyle = new GUIStyle(HighLogic.Skin.horizontalScrollbar);
            vertiScrollBarStyle = new GUIStyle(HighLogic.Skin.verticalScrollbar);
        }

        /// <summary>
        /// GUILocation
        /// </summary>
        /// <param name="windowID"></param>
        private void WindowGUILocation(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 350, 30));
            if (rmmmSelectGUI != null)
            {
                rmmmSelectGUI.windowGUIMainX = windowPosGUILocation.xMin;
                rmmmSelectGUI.windowGUIMainY = windowPosGUILocation.yMin + windowPosGUILocation.height;
                rmmmSelectGUI.windowGUIMainWidth = windowPosGUILocation.width;
            }
            
            GUILayout.BeginVertical();




            scrollPositionModules = GUILayout.BeginScrollView(scrollPositionModules, true, false, horiScrollBarStyle, GUIStyle.none, GUILayout.Width(350), GUILayout.Height(50));
            GUILayout.BeginHorizontal();
            foreach (RMMModule rmmm in RegisteredModuleList)
            {
                if (GUILayout.Button(rmmm.PortCode, buttonStyle, GUILayout.Height(30)))
                {
                    if (rmmmSelectGUI != null) { rmmmSelectGUI.closeGUIMain(); }
                    rmmmSelectGUI = rmmm;
                    rmmmSelectGUI.closeGUIMain();
                    rmmmSelectGUI.openGUIMain();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private void drawGUILocation()
        {
            windowPosGUILocation = GUILayout.Window(3408, windowPosGUILocation, WindowGUILocation, "Location " + FlightGlobals.ActiveVessel.vesselName, windowStyle);
        }

        public void openGUILocation()
        {
            closeGUILocation();
            initStyle();
            makeRegisteredModuleList();
            RenderingManager.AddToPostDrawQueue(348, new Callback(drawGUILocation));
        }

        public void closeGUILocation()
        {
            if (rmmmSelectGUI != null) { rmmmSelectGUI.closeGUIMain(); }
            RenderingManager.RemoveFromPostDrawQueue(348, new Callback(drawGUILocation));
        }

        private void makeRegisteredModuleList()
        {
            RegisteredModuleList.Clear();

            foreach (Part p in FlightGlobals.ActiveVessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (pm.ClassName == "RMMModule")
                    {
                        RMMModule aRMMModule;
                        aRMMModule = p.Modules.OfType<RMMModule>().FirstOrDefault();
                        if (aRMMModule.PortCode != "" && aRMMModule.OrderingEnabled && !aRMMModule.trackingActive) 
                        {
                            RegisteredModuleList.Add(aRMMModule);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// GUITermsCondi
        /// </summary>
        /// <param name="windowID"></param>
        private void WindowGUITermsCondi(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 250, 30));

            GUILayout.BeginVertical();

            scrollPositionDisc = GUILayout.BeginScrollView(scrollPositionDisc, false, true, GUIStyle.none, vertiScrollBarStyle , GUILayout.Width(240), GUILayout.Height(300));

            GUILayout.Label( 
            "Our scientist have discovered docking ports are in fact very round in nature. This new-found knowledge explains why ships can dock in so many different ways to a docking port. In practice ships have been seen to dock straight, canted slightly to the left, a full quarter to the right and even almost upside down with a two degree counterclockwise offset." + Environment.NewLine +
            "Based on these observations some theoretical scientist have argued there could be infinite many ways to connect two docking ports." + Environment.NewLine +
            "But all this metaphysical nonsense aside, docking is already super hard on our Kerbonauts. They can't be expected to align two docking ports together and also control the relative rotation of the approach. That's like totally doing two things at the same time!" + Environment.NewLine +
            Environment.NewLine +
            "So when you order a mission to be executed unsupervised through the Routine Mission Manager, you should make sure every possible rotation between the current target docking port and the mission vessel does not lead to a collision between parts of the station and the approaching vessel. If you can not make sure of this you should not order this mission in combination with this docking port."            
            ,labelTextStyle,GUILayout.Width(205));

            if (GUILayout.Button("I understand and acknowledge", buttonStyle, GUILayout.Height(30)))
            {
                System.IO.File.Create(GamePath + CommercialOfferingsPath + "UserAcknowledgesKnownBugs");
                closeGUITermsCondi();
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private void drawGUITermsCondi()
        {
            windowPosGUITermsCondi = GUILayout.Window(3409, windowPosGUITermsCondi, WindowGUITermsCondi, "New science breakthrough!", windowStyle);
        }

        public void openGUITermsCondi()
        {
            closeGUITermsCondi();
            initStyle();
            RenderingManager.AddToPostDrawQueue(353, new Callback(drawGUITermsCondi));
        }

        public void closeGUITermsCondi()
        {
            RenderingManager.RemoveFromPostDrawQueue(353, new Callback(drawGUITermsCondi));
        }

    }
}