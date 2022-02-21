using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SciChart.Charting.Visuals;


namespace Trader
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SciChartSurface.SetRuntimeLicenseKey(@"px2o1UH0Sp2Ge82uOox/xmJkY9ZL9N8HCPp9JHyWeZchQuj2BGoMVsYdGbdU4eyJ795JpxSouoC3+VoiJSoHoxWC3Mf0WMYCDnx77gE9sXCmEf6gXV+xSw3DgurMOz9P7oNw8HTZwn/4Y1IQWaCsG68j+znlLjQuu40qLVujPKIrvyztzSmc+m8GXPMGkpiHvEkUFRpHivLyRkh5GBb9TUDMzEw8H0gq0NEhcIMuXY2aZvM3nbmTIVXuwziEYXhNvCrDRQ5mxW77F+NwW6Da5DnM7QVjRVKZHHiV8unHiMzwXRXRytWLemuQ9t9chMd5KzMxppAj7DLRCL0aZskAVPZgZDwn4Jilre3r299FpsH97PMKhyThi2yqfmHIUwuvASuHj+pfTudyd1DEGP47uSrliTz9Dr2ey3OSu592bsvgvn5k78DDbAIb9/xS8tA77h9oRiorOnqdevLh7SkpJN0cZ2y3p5wS459C3J9FhdJAhgqSpvy3dr+f06upCI9hsJyCwe2z8Vhey0VBIY+Q183eVPicwPPr9Uw0Ha9fLdG2INBhuY0PW/MleYd+FGyUGhTwcuKKZLCguprPUvYWPai/lp51v8mFrzUU");
        }
    }
}
