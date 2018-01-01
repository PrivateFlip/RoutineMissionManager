﻿using CommercialOfferings.MissionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CommercialOfferings.Gui
{
    class TrackingDetailWindow : WindowBase
    {
        private Vector2 scrollPositionTracking;

        public Mission TrackingMission;
        public CheckList ValidCheckList;

        public TrackingDetailWindow() : base("Tracking", new Rect(), 400, 300)
        {

        }

        public override void WindowUpdate()
        {

        }

        public override void WindowUI()
        {
            GUILayout.BeginVertical();

            scrollPositionTracking = GUILayout.BeginScrollView(scrollPositionTracking, false, false, RmmStyle.Instance.HoriScrollBarStyle, RmmStyle.Instance.VertiScrollBarStyle, GUILayout.Width(390), GUILayout.Height(500));

            float labelWidth = 350;

            if (TrackingMission != null)
            {
                if (TrackingMission.Info != null)
                {
                    MissionInfo info = TrackingMission.Info;

                    GUILayout.Label("INFO", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Type:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(info.Type.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Name:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(info.Name.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                }
                if (TrackingMission.Launch != null)
                {
                    MissionLaunch launch = TrackingMission.Launch;

                    GUILayout.Label("", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("LAUNCH", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Time:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(launch.Time.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Body:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(launch.Body.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Vessel:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(launch.VesselName.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Value:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(launch.Funds.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Crew:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(launch.Crew.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Parts:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label((launch.Parts == null ? "0" : launch.Parts.Count.ToString()), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                }
                if (TrackingMission.Departure != null)
                {
                    MissionDeparture departure = TrackingMission.Departure;

                    GUILayout.Label("", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("DEPARTURE", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Time:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(departure.Time.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Body:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(departure.Body.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Crew:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(departure.Crew.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Crew capacity:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(departure.CrewCapacity.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Parts:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label((departure.Parts == null ? "no parts tracked" : departure.Parts.Count.ToString()), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    if (departure.Resources != null)
                    {
                        foreach (MissionResource resource in departure.Resources)
                        {
                            GUILayout.Label(resource.Name + " " + resource.Amount.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        }
                    }
                    if (departure.Proppellants != null)
                    {
                        foreach (String proppellant in departure.Proppellants)
                        {
                            GUILayout.Label("Proppellant " + proppellant, RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        }
                    }
                }
                if (TrackingMission.Landings != null)
                {
                    foreach (MissionLanding landing in TrackingMission.Landings)
                    {
                        GUILayout.Label("", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label("LANDING", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label("Time:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label(landing.Time.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label("Body:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label(landing.Body.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label("Value:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label(landing.Funds.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label("Crew:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label(landing.Crew.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label("Crew capacity:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label(landing.CrewCapacity.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label("Parts:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        GUILayout.Label((landing.Parts == null ? "no parts tracked" : landing.Parts.Count.ToString()), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                        if (landing.Resources != null)
                        {
                            foreach (MissionResource resource in landing.Resources)
                            {
                                GUILayout.Label(resource.Name + " " + resource.Amount.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                            }
                        }
                    }
                }
                if (TrackingMission.Arrival != null)
                {
                    MissionArrival arrival = TrackingMission.Arrival;

                    GUILayout.Label("", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("ARRIVAL", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Time:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(arrival.Time.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Body:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(arrival.Body.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Crew:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(arrival.Crew.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Crew capacity:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label(arrival.CrewCapacity.ToString(), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label("Parts:", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    GUILayout.Label((arrival.Parts == null ? "no parts tracked" : arrival.Parts.Count.ToString()), RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                }
            }

            if (ValidCheckList != null)
            {
                GUILayout.Label("", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));

                if (ValidCheckList.CheckSucces)
                {
                    GUILayout.Label("Valid", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                }
                else
                {
                    GUILayout.Label("Invalid", RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    foreach (String message in ValidCheckList.Messages)
                    {
                        GUILayout.Label(message, RmmStyle.Instance.LabelStyle, GUILayout.Width(labelWidth));
                    }
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
    }
}
