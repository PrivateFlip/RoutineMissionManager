﻿/*Copyright (c) 2014, Flip van Toly
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
using Contracts;

namespace CommercialOfferings
{
    public partial class RMMModule : PartModule
    {
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


    }
}