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
        [KSPField(isPersistant = false, guiActive = false)]
        public bool DevMode = false;

        string GamePath;
        string CommercialOfferingsPath = "GameData" + Path.DirectorySeparatorChar + "RoutineMissionManager" +  Path.DirectorySeparatorChar;

        //GUI
        private static GUIStyle windowStyle, labelStyle, redlabelStyle, textFieldStyle, buttonStyle;
        private bool initializedStyles = false;

        public override void OnAwake()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                GamePath = KSPUtil.ApplicationRootPath;

                if (part != null) { part.force_activate(); }
                ArrivalStage = 0;
                nextLogicTime = Planetarium.GetUniversalTime();

                if (DevMode) { OrderingEnabled = true; }
            }
        }

        public override void OnFixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight) { return; }
            setModule();
            if (nextLogicTime == 0 || nextLogicTime > Planetarium.GetUniversalTime()) { return; }
            if (vessel.packed || !vessel.loaded)
            {
                nextLogicTime = Planetarium.GetUniversalTime();
                return;
            }

            //if (!moduleSet) { setModule(); moduleSet = true; }

            if (trackingActive || trackingPrimary)
            {
                if (!handleTracking()) { return; }
            }
            else
            {
                if (!handleCommercialVehicleMode()) { return; }
                handleArrivalCompletion();
            }
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.Repaint || Event.current.isMouse)
            {
                // preDraw code
            }
            drawGUI();
        }

        private void drawGUI()
        {
            if (!initializedStyles)
            {
                initStyle();
                initializedStyles = true;
            }

            //Tracking GUI rendering
            if (renderGUITracking)
            {
                drawGUITracking();
            }

            //Routine GUI rendering
            if (renderGUIMain)
            {
                drawGUIMain();
            }
            if (renderGUIOffering)
            {
                drawGUIOffering();
            }
            if (renderGUIMission)
            {
                drawGUIMission();
            }
            if (renderGUIPrefCrew)
            {
                drawGUIPrefCrew();
            }
            if (renderGUIRegister)
            {
                drawGUIRegister();
            }
        }

        private void initStyle()
        {
            windowStyle = new GUIStyle(HighLogic.Skin.window);
            windowStyle.stretchWidth = false;
            windowStyle.stretchHeight = false;

            labelStyle = new GUIStyle(HighLogic.Skin.label);
            labelStyle.stretchWidth = false;
            labelStyle.stretchHeight = false;

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
        }
    }
}
