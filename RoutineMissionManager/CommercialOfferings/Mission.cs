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
    public class Offering
    {
        public string folderName = "";

        public string Name = "";
        public string CompanyName = "";
        public string VehicleName = "";
        public string LaunchSystemName = "";

        public double Price = 0;
        public double VehicleReturnFee = 0;
        public double Time = 0;
        public string Body = "";
        public double MaxOrbitAltitude = 0;

        public string Description = "";

        public int MinimumCrew = 0;
        public int MaximumCrew = 0;

        public bool ReturnEnabled = false;
        public bool SafeReturn = false;
        public string ReturnResources = "";
        public double ReturnCargoMass = 0.0;

        public int Port = 0;
        public float DockingDistance = 0.15f;

        public void loadOffering(String FilePath)
        {
            string[] data = System.IO.File.ReadAllLines(FilePath);

            foreach (String str in data)
            {
                string[] Line = str.Split('=');

                switch (Line[0].Trim())
                {
                    case "Name":
                        Name = Line[1].Trim();
                        break;
                    case "CompanyName":
                        CompanyName = Line[1].Trim();
                        break;
                    case "VehicleName":
                        VehicleName = Line[1].Trim();
                        break;
                    case "LaunchSystemName":
                        LaunchSystemName = Line[1].Trim();
                        break;
                    case "Price":
                        Price = Convert.ToDouble(Line[1].Trim());
                        break;
                    case "VehicleReturnFee":
                        VehicleReturnFee = Convert.ToDouble(Line[1].Trim());
                        break;
                    case "Time":
                        Time = Convert.ToDouble(Line[1].Trim());
                        break;
                    case "Body":
                        Body = Line[1].Trim();
                        break;
                    case "MaxOrbitAltitude":
                        MaxOrbitAltitude = Convert.ToDouble(Line[1].Trim());
                        break;
                    case "Description":
                        Description = Line[1];
                        break;
                    case "MinimumCrew":
                        MinimumCrew = Convert.ToInt16(Line[1].Trim());
                        break;
                    case "MaximumCrew":
                        MaximumCrew = Convert.ToInt16(Line[1].Trim());
                        break;
                    case "ReturnEnabled":
                        ReturnEnabled = (Line[1].Trim() == "True");
                        break;
                    case "SafeReturn":
                        SafeReturn = (Line[1].Trim() == "True");
                        break;
                    case "ReturnResources":
                        ReturnResources = Line[1].Trim();
                        break;
                    case "ReturnCargoMass":
                        ReturnCargoMass = Convert.ToDouble(Line[1].Trim());
                        break;
                    case "Port":
                        Port = Convert.ToInt16(Line[1].Trim());
                        break;
                    case "DockingDistance":
                        DockingDistance = (float)Convert.ToDouble(Line[1].Trim());
                        break;
                }
            }
        }
    }
}