using System;
using BlazorLeaflet.Models;
using BlazorLeaflet.Utils;
using System.Collections.ObjectModel;
using System.Drawing;
using Microsoft.JSInterop;
using BlazorLeaflet.Models.Events;
using System.Threading.Tasks;
using System.Collections.Specialized;
using BlazorLeaflet.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorLeaflet
{
    public class Map : IDisposable
    {
        /// <summary>
        /// Initial geographic center of the map
        /// </summary>
        public LatLng Center
        {
            get => center;
            set
            {
                center = value;
                if (isInitialized)
                    RunTaskInBackground(async () => await LeafletInterops.PanTo(
                        jsRuntime, Id, value.ToPointF(), false, 0, 0, false));
            }
        }

        /// <summary>
        /// Initial map zoom level
        /// </summary>
        public float Zoom
        {
            get => zoom;
            set
            {
                zoom = value;
                if (isInitialized)
                    RunTaskInBackground(async () => await LeafletInterops.SetZoom(
                        jsRuntime, Id, value));
            }
        }

        /// <summary>
        /// Minimum zoom level of the map. If not specified and at least one 
        /// GridLayer or TileLayer is in the map, the lowest of their minZoom
        /// options will be used instead.
        /// </summary>
        public float? MinZoom { get; set; }

        /// <summary>
        /// Maximum zoom level of the map. If not specified and at least one
        /// GridLayer or TileLayer is in the map, the highest of their maxZoom
        /// options will be used instead.
        /// </summary>
        public float? MaxZoom { get; set; }

        /// <summary>
        /// When this option is set, the map restricts the view to the given
        /// geographical bounds, bouncing the user back if the user tries to pan
        /// outside the view.
        /// </summary>
        public Tuple<LatLng, LatLng> MaxBounds { get; set; }

        /// <summary>
        /// Whether a zoom control is added to the map by default.
        /// <para/>
        /// Defaults to true.
        /// </summary>
        public bool ZoomControl { get; set; } = true;

        /// <summary>
        /// Event raised when the component has finished its first render.
        /// </summary>
        public event Action OnInitialized;

        public string Id { get; }

        private LatLng center = new LatLng();
        private float zoom;

        private readonly ObservableCollection<Layer> layers = new ObservableCollection<Layer>();

        private readonly ObservableCollection<Marker> markers = new ObservableCollection<Marker>();

        private readonly IJSRuntime jsRuntime;

        private bool isInitialized;

        public Map(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            Id = StringHelper.GetRandomString(10);

            layers.CollectionChanged += OnLayersChanged;
            markers.CollectionChanged += OnLayersChanged;
        }

        private async void RunTaskInBackground(Func<Task> task)
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                NotifyBackgroundExceptionOccurred(ex);
            }
        }

        /// <summary>
        /// This method MUST be called only once by the Blazor component upon rendering, and never by the user.
        /// </summary>
        public void RaiseOnInitialized()
        {
            isInitialized = true;
            OnInitialized?.Invoke();
        }

        /// <summary>
        /// Add a layer to the map.
        /// </summary>
        /// <param name="layer">The layer to be added.</param>
        /// <exception cref="System.ArgumentNullException">Throws when the layer is null.</exception>
        /// <exception cref="UninitializedMapException">Throws when the map has not been yet initialized.</exception>
        public void AddLayer(Layer layer)
        {
            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (!isInitialized)
            {
                throw new UninitializedMapException();
            }

            layers.Add(layer);
        }

        /// <summary>
        /// Remove a layer from the map.
        /// </summary>
        /// <param name="layer">The layer to be removed.</param>
        /// <exception cref="System.ArgumentNullException">Throws when the layer is null.</exception>
        /// <exception cref="UninitializedMapException">Throws when the map has not been yet initialized.</exception>
        public void RemoveLayer(Layer layer)
        {
            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (!isInitialized)
            {
                throw new UninitializedMapException();
            }

            layers.Remove(layer);
        }

        public void ClearLayers()
        {
            if (!isInitialized)
            {
                throw new UninitializedMapException();
            }

            foreach (var l in layers)
                LeafletInterops.RemoveLayer(jsRuntime, Id, l.Id);
            layers.Clear();
        }


        /// <summary>
        /// Add a layer to the map.
        /// </summary>
        /// <param name="layer">The layer to be added.</param>
        /// <exception cref="System.ArgumentNullException">Throws when the layer is null.</exception>
        /// <exception cref="UninitializedMapException">Throws when the map has not been yet initialized.</exception>
        public void AddMarker(Marker layer)
        {
            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (!isInitialized)
            {
                throw new UninitializedMapException();
            }

            markers.Add(layer);
        }

        /// <summary>
        /// Remove a layer from the map.
        /// </summary>
        /// <param name="layer">The layer to be removed.</param>
        /// <exception cref="System.ArgumentNullException">Throws when the layer is null.</exception>
        /// <exception cref="UninitializedMapException">Throws when the map has not been yet initialized.</exception>
        public void RemoveMarker(Marker layer)
        {
            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (!isInitialized)
            {
                throw new UninitializedMapException();
            }

            markers.Remove(layer);
        }

        public void ClearMarkers()
        {
            if (!isInitialized)
            {
                throw new UninitializedMapException();
            }

            foreach (var l in markers)
            {
                if (l.ClusterID == null)
                    LeafletInterops.RemoveLayer(jsRuntime, Id, l.Id);
                else
                    LeafletInterops.RemoveLayerFromCluster(jsRuntime, Id, l.Id, l.ClusterID.Value);
            }
                
            markers.Clear();
        }

        public void InvalidateSize(int delay = 0)
        {
            LeafletInterops.InvalidateSize(jsRuntime, Id, delay);
        }

        public void DisableInteraction()
        {
            LeafletInterops.DisableInteraction(jsRuntime, Id);
        }

        public void EnableInteraction()
        {
            LeafletInterops.EnableInteraction(jsRuntime, Id);
        }

        /// <summary>
        /// Get a read only collection of the current layers.
        /// </summary>
        /// <returns>A read only collection of layers.</returns>
        public IReadOnlyCollection<Layer> GetLayers()
        {
            return layers.ToList().AsReadOnly();
        }

        private void OnLayersChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in args.NewItems)
                {
                    var layer = item as Layer;
                    if (layer.ClusterID == null)
                        LeafletInterops.AddLayer(jsRuntime, Id, layer);
                    else
                        LeafletInterops.AddLayerToCluster(jsRuntime, Id, layer, layer.ClusterID.Value);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in args.OldItems)
                {
                    if (item is Layer layer)
                    {
                        if (layer.ClusterID == null)
                            LeafletInterops.RemoveLayer(jsRuntime, Id, layer.Id);
                        else
                            LeafletInterops.RemoveLayerFromCluster(jsRuntime, Id, layer.Id, layer.ClusterID.Value);
                    }
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Replace
                     || args.Action == NotifyCollectionChangedAction.Move)
            {
                foreach (var oldItem in args.OldItems)
                    if (oldItem is Layer layer)
                    {
                        if (layer.ClusterID == null)
                            LeafletInterops.RemoveLayer(jsRuntime, Id, layer.Id);
                        else
                            LeafletInterops.RemoveLayerFromCluster(jsRuntime, Id, layer.Id, layer.ClusterID.Value);
                    }

                foreach (var newItem in args.NewItems)
                {
                    var layer = newItem as Layer;
                    if (layer.ClusterID == null)
                        LeafletInterops.AddLayer(jsRuntime, Id, layer);
                    else
                        LeafletInterops.AddLayerToCluster(jsRuntime, Id, layer, layer.ClusterID.Value);
                }
            }
        }
        
        public void FitBounds(PointF corner1, PointF corner2, PointF? padding = null, float? maxZoom = null)
        {
            LeafletInterops.FitBounds(jsRuntime, Id, corner1, corner2, padding, maxZoom);
        }

        public void PanTo(PointF position, bool animate = false, float duration = 0.25f, float easeLinearity = 0.25f, bool noMoveStart = false)
        {
            LeafletInterops.PanTo(jsRuntime, Id, position, animate, duration, easeLinearity, noMoveStart);
        }

        public void FlyTo(PointF position, float zoomLevel)
        {
            LeafletInterops.FlyTo(jsRuntime, Id, position, zoomLevel);
        }
        public void FlyToBounds(PointF corner1, PointF corner2, float zoomLevel)
        {
            LeafletInterops.FlyToBounds(jsRuntime, Id, corner1, corner2, zoomLevel);
        }

        public async Task<LatLng> GetCenter() => await LeafletInterops.GetCenter(jsRuntime, Id);
        public async Task<float> GetZoom() => 
            await LeafletInterops.GetZoom(jsRuntime, Id);

        /// <summary>
        /// Increases the zoom level by one notch.
        /// 
        /// If <c>shift</c> is held down, increases it by three.
        /// </summary>
        public async Task ZoomIn(MouseEventArgs e) => await LeafletInterops.ZoomIn(jsRuntime, Id, e);

        /// <summary>
        /// Decreases the zoom level by one notch.
        /// 
        /// If <c>shift</c> is held down, decreases it by three.
        /// </summary>
        public async Task ZoomOut(MouseEventArgs e) => await LeafletInterops.ZoomOut(jsRuntime, Id, e);

        private async Task UpdateZoom()
        {
            zoom = await GetZoom();
        }

        private async Task UpdateCenter()
        {

            center = await GetCenter();
        }

        public async Task UpdateHeatOptions(HeatLayer layer)
        {
            await LeafletInterops.UpdateHeatOption(jsRuntime, Id, layer);
        }

        #region events

        public delegate void MapEventHandler(object sender, Event e);
        public delegate void MapResizeEventHandler(object sender, ResizeEvent e);

        public event MapEventHandler OnZoomLevelsChange;
        [JSInvokable]
        public void NotifyZoomLevelsChange(Event e) => OnZoomLevelsChange?.Invoke(this, e);

        public event MapResizeEventHandler OnResize;
        [JSInvokable]
        public void NotifyResize(ResizeEvent e) => OnResize?.Invoke(this, e);

        public event MapEventHandler OnUnload;
        [JSInvokable]
        public void NotifyUnload(Event e) => OnUnload?.Invoke(this, e);

        public event MapEventHandler OnViewReset;
        [JSInvokable]
        public void NotifyViewReset(Event e) => OnViewReset?.Invoke(this, e);

        public event MapEventHandler OnLoad;
        [JSInvokable]
        public void NotifyLoad(Event e) => OnLoad?.Invoke(this, e);

        public event MapEventHandler OnZoomStart;
        [JSInvokable]
        public void NotifyZoomStart(Event e) => OnZoomStart?.Invoke(this, e);

        public event MapEventHandler OnMoveStart;
        [JSInvokable]
        public void NotifyMoveStart(Event e) => OnMoveStart?.Invoke(this, e);

        public event MapEventHandler OnZoom;
        [JSInvokable]
        public void NotifyZoom(Event e) => OnZoom?.Invoke(this, e);

        public event MapEventHandler OnMove;
        [JSInvokable]
        public void NotifyMove(Event e) => OnMove?.Invoke(this, e);

        public event MapEventHandler OnZoomEnd;
        [JSInvokable]
        public async void NotifyZoomEnd(Event e)
        {
            try
            {
                await UpdateZoom();
            }
            finally
            {
                OnZoomEnd?.Invoke(this, e);
            }
        }

        public event MapEventHandler OnMoveEnd;
        [JSInvokable]
        public async void NotifyMoveEnd(Event e)
        {
            try
            {
                await UpdateCenter();
            }
            finally
            {
                OnMoveEnd?.Invoke(this, e);
            }
        }

        public event MouseEventHandler OnMouseMove;
        [JSInvokable]
        public void NotifyMouseMove(MouseEvent eventArgs) => OnMouseMove?.Invoke(this, eventArgs);

        public event MapEventHandler OnKeyPress;
        [JSInvokable]
        public void NotifyKeyPress(Event eventArgs) => OnKeyPress?.Invoke(this, eventArgs);

        public event MapEventHandler OnKeyDown;
        [JSInvokable]
        public void NotifyKeyDown(Event eventArgs) => OnKeyDown?.Invoke(this, eventArgs);

        public event MapEventHandler OnKeyUp;
        [JSInvokable]
        public void NotifyKeyUp(Event eventArgs) => OnKeyUp?.Invoke(this, eventArgs);

        public event MouseEventHandler OnPreClick;
        [JSInvokable]
        public void NotifyPreClick(MouseEvent eventArgs) => OnPreClick?.Invoke(this, eventArgs);

        public event EventHandler<Exception> BackgroundExceptionOccurred;
        private void NotifyBackgroundExceptionOccurred(Exception exception) =>
            BackgroundExceptionOccurred?.Invoke(this, exception);

        #endregion events

        #region InteractiveLayerEvents
        // Has the same events as InteractiveLayer, but it is not a layer. 
        // Could place this code in its own class and make Layer inherit from that, but not every layer is interactive...
        // Is there a way to not duplicate this code?

        public delegate void MouseEventHandler(Map sender, MouseEvent e);

        public event MouseEventHandler OnClick;
        [JSInvokable]
        public void NotifyClick(MouseEvent eventArgs) => OnClick?.Invoke(this, eventArgs);

        public event MouseEventHandler OnDblClick;
        [JSInvokable]
        public void NotifyDblClick(MouseEvent eventArgs) => OnDblClick?.Invoke(this, eventArgs);

        public event MouseEventHandler OnMouseDown;
        [JSInvokable]
        public void NotifyMouseDown(MouseEvent eventArgs) => OnMouseDown?.Invoke(this, eventArgs);

        public event MouseEventHandler OnMouseUp;
        [JSInvokable]
        public void NotifyMouseUp(MouseEvent eventArgs) => OnMouseUp?.Invoke(this, eventArgs);

        public event MouseEventHandler OnMouseOver;
        [JSInvokable]
        public void NotifyMouseOver(MouseEvent eventArgs) => OnMouseOver?.Invoke(this, eventArgs);

        public event MouseEventHandler OnMouseOut;
        [JSInvokable]
        public void NotifyMouseOut(MouseEvent eventArgs) => OnMouseOut?.Invoke(this, eventArgs);

        public event MouseEventHandler OnContextMenu;

        [JSInvokable]
        public void NotifyContextMenu(MouseEvent eventArgs) => OnContextMenu?.Invoke(this, eventArgs);

        public void Dispose()
        {
            LeafletInterops.Dispose(jsRuntime, Id);
        }

        #endregion InteractiveLayerEvents
    }
}
