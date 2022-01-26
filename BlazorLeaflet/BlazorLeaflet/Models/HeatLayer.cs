using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorLeaflet.Models
{
    public class HeatLayer : Layer
    {
        public LatLng[] LatLongs { get; set; }

        public int Radius { get; set; } = 25;

        public int Opacity { get; set; } = 150;
    }
}
