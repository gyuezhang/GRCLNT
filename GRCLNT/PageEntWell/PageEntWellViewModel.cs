using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.UI.Wpf;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRCLNT
{
    public class PageEntWellViewModel : Screen
    {
        public PageEntWellViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
            InitMap();
        }
        private WndMainViewModel wndMainVM { get; set; }

        #region SocketHandler

        #endregion SocketHandler

        #region Bindings
        public int pageIndexBd { get; set; } = 0;
        public MapControl map { get; set; } = new MapControl();

        #endregion Bindings

        #region Actions
        public void SelectPageCmd(string cmdPara)
        {
            wndMainVM.SelectPage((E_Page)Enum.Parse(typeof(E_Page), cmdPara, true));
        }
        #endregion Actions

        public void InitMap()
        {
            map.Map.Layers.Add(new TileLayer(KnownTileSources.Create()));
            //map.Map.Layers.Add(CreateWellLayer());

            var centerOfBD = new Mapsui.Geometries.Point(117.309716, 39.717173);
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfBD.X, centerOfBD.Y);
            map.Map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, map.Map.Resolutions[12]);

        }
    }
}
