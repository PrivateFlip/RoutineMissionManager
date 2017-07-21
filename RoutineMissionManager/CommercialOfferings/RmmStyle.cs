using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CommercialOfferings
{
    public class RmmStyle
    {
        private static RmmStyle instance;

        private RmmStyle()
        {
            InitializeStyle();
        }

        public static RmmStyle Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new RmmStyle();
                }
                return instance;
            }
        }


        public GUIStyle WindowStyle, LabelStyle, LabelTextStyle, RedLabelStyle, TextFieldStyle, ButtonStyle, HoriScrollBarStyle, VertiScrollBarStyle;



        private void InitializeStyle()
        {
            WindowStyle = new GUIStyle(HighLogic.Skin.window);
            WindowStyle.stretchWidth = false;
            WindowStyle.stretchHeight = false;
                
            LabelStyle = new GUIStyle(HighLogic.Skin.label);
            LabelStyle.stretchWidth = false;
            LabelStyle.stretchHeight = false;
                
            LabelTextStyle = new GUIStyle(HighLogic.Skin.label);
            LabelTextStyle.stretchWidth = false;
            LabelTextStyle.stretchHeight = true;
            LabelTextStyle.wordWrap = true;
                
            RedLabelStyle = new GUIStyle(HighLogic.Skin.label);
            RedLabelStyle.stretchWidth = false;
            RedLabelStyle.stretchHeight = false;
            RedLabelStyle.normal.textColor = Color.red;
                
            TextFieldStyle = new GUIStyle(HighLogic.Skin.textField);
            TextFieldStyle.stretchWidth = false;
            TextFieldStyle.stretchHeight = false;
                
            ButtonStyle = new GUIStyle(HighLogic.Skin.button);
            ButtonStyle.stretchHeight = false;
            ButtonStyle.stretchWidth = false;
                
            HoriScrollBarStyle = new GUIStyle(HighLogic.Skin.horizontalScrollbar);
            VertiScrollBarStyle = new GUIStyle(HighLogic.Skin.verticalScrollbar);
        }
    }
}
