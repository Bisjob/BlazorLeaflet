using BlazorLeaflet.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Rectangle = BlazorLeaflet.Models.Rectangle;

namespace BlazorLeaflet
{
    public static class LeafletInterops
    {

        private static ConcurrentDictionary<string, (IDisposable, string, Layer)> LayerReferences { get; }
            = new ConcurrentDictionary<string, (IDisposable, string, Layer)>();

        private static readonly string baseObjectContainer = "window.leafletBlazor";

        public static ValueTask Create(IJSRuntime jsRuntime, Map map) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.create", map, DotNetObjectReference.Create(map));

        private static DotNetObjectReference<T> CreateLayerReference<T>(string mapId, T layer) where T : Layer
        {
            var result = DotNetObjectReference.Create(layer);
            LayerReferences.TryAdd(layer.Id, (result, mapId, layer));
            return result;
        }

        private static void DisposeLayerReference(string layerId)
        {
            if (LayerReferences.TryRemove(layerId, out var value))
                value.Item1.Dispose();
        }

        public static ValueTask AddLayer(IJSRuntime jsRuntime, string mapId, Layer layer)
        {
            return layer switch
            {
                TileLayer tileLayer => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addTilelayer", mapId, tileLayer, CreateLayerReference(mapId, tileLayer)),
                MbTilesLayer mbTilesLayer => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addMbTilesLayer", mapId, mbTilesLayer, CreateLayerReference(mapId, mbTilesLayer)),
                ShapefileLayer shapefileLayer => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addShapefileLayer", mapId, shapefileLayer, CreateLayerReference(mapId, shapefileLayer)),
                Marker marker => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addMarker", mapId, marker, CreateLayerReference(mapId, marker)),
                Rectangle rectangle => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addRectangle", mapId, rectangle, CreateLayerReference(mapId, rectangle)),
                Circle circle => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addCircle", mapId, circle, CreateLayerReference(mapId, circle)),
                Polygon polygon => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addPolygon", mapId, polygon, CreateLayerReference(mapId, polygon)),
                Polyline polyline => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addPolyline", mapId, polyline, CreateLayerReference(mapId, polyline)),
                ImageLayer image => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addImageLayer", mapId, image, CreateLayerReference(mapId, image)),
                GeoJsonDataLayer geo => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addGeoJsonLayer", mapId, geo, CreateLayerReference(mapId, geo)),
                HeatLayer heat => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addHeatLayer", mapId, heat, CreateLayerReference(mapId, heat)),
                _ => throw new NotImplementedException($"The layer {typeof(Layer).Name} has not been implemented."),
            };
        }

        public static ValueTask AddLayerToCluster(IJSRuntime jsRuntime, string mapId, Layer layer, int clusterID)
        {
            return layer switch
            {
                Marker marker => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addMarkerToCluster", mapId, marker, CreateLayerReference(mapId, marker), clusterID),
                _ => throw new NotImplementedException($"The layer cluster {typeof(Layer).Name} has not been implemented."),
            };
        }
        public static async ValueTask RemoveLayerFromCluster(IJSRuntime jsRuntime, string mapId, string layerId, int clusterID)
        {
            await jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.removeLayerFromCluster", mapId, layerId, clusterID);
            DisposeLayerReference(layerId);
        }
        
        public static async ValueTask StyleGeoJsonLayerFeatures(IJSRuntime jsRuntime, string mapId, string layerId, string[][] colourMap)
        {
            await jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.styleGeoJsonLayerFeatures", mapId, layerId, colourMap);
        }

        public static async ValueTask AddMarkers(IJSRuntime jsRuntime, string mapId, IEnumerable<Marker> markers)
        {
            await jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.addMarkers", mapId, markers);
        }

        public static async ValueTask RemoveLayer(IJSRuntime jsRuntime, string mapId, string layerId)
        {
            await jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.removeLayer", mapId, layerId);
            DisposeLayerReference(layerId);
        }

        public static ValueTask UpdatePopupContent(IJSRuntime jsRuntime, string mapId, Layer layer) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.updatePopupContent", mapId, layer.Id, layer.Popup?.Content);

        public static ValueTask UpdateTooltipContent(IJSRuntime jsRuntime, string mapId, Layer layer) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.updateTooltipContent", mapId, layer.Id, layer.Tooltip?.Content);

        public static ValueTask UpdateShape(IJSRuntime jsRuntime, string mapId, Layer layer) =>
            layer switch
            {
                Rectangle rectangle => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.updateRectangle", mapId, rectangle),
                Circle circle => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.updateCircle", mapId, circle),
                Polygon polygon => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.updatePolygon", mapId, polygon),
                Polyline polyline => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.updatePolyline", mapId, polyline),
                _ => throw new NotImplementedException($"The layer {typeof(Layer).Name} has not been implemented."),
            };

        public static ValueTask UpdateHeatOption(IJSRuntime jSRuntime, string mapId, HeatLayer heat) => 
            jSRuntime.InvokeVoidAsync($"{baseObjectContainer}.updateHeatOptions", mapId, heat);
        
        public static ValueTask InvalidateSize(IJSRuntime jSRuntime, string mapId, int delay) => 
            jSRuntime.InvokeVoidAsync($"{baseObjectContainer}.invalidateSize", mapId, delay);

        public static ValueTask FitBounds(IJSRuntime jsRuntime, string mapId, PointF corner1, PointF corner2, PointF? padding, float? maxZoom) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.fitBounds", mapId, corner1, corner2, padding, maxZoom);

        public static ValueTask PanTo(IJSRuntime jsRuntime, string mapId, PointF position, bool animate, float duration, float easeLinearity, bool noMoveStart) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.panTo", mapId, position, animate, duration, easeLinearity, noMoveStart);

        public static ValueTask SetMaxBounds(IJSRuntime jsRuntime, string mapId, LatLngBounds bounds)
            => jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.setMaxBounds",  mapId, bounds);

        public static ValueTask<LatLng> GetCenter(IJSRuntime jsRuntime, string mapId) =>
            jsRuntime.InvokeAsync<LatLng>($"{baseObjectContainer}.getCenter", mapId);

        public static ValueTask<float> GetZoom(IJSRuntime jsRuntime, string mapId) =>
            jsRuntime.InvokeAsync<float>($"{baseObjectContainer}.getZoom", mapId);

        public static ValueTask<LatLngBounds> GetBounds(IJSRuntime jsRuntime, string mapId) =>
            jsRuntime.InvokeAsync<LatLngBounds>($"{baseObjectContainer}.getBounds", mapId);

        public static ValueTask SetZoom(IJSRuntime jsRuntime, string mapId, float zoomLevel) => 
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.setZoom", mapId, zoomLevel);

        public static ValueTask FlyTo(IJSRuntime jsRuntime, string mapId, PointF position, float zoomLevel) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.flyTo", mapId, position, zoomLevel);

        public static ValueTask FlyToBounds(IJSRuntime jsRuntime, string mapId, PointF corner1, PointF corner2, float zoomLevel) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.flyToBounds", mapId, corner1, corner2, zoomLevel);

        public static ValueTask ZoomIn(IJSRuntime jsRuntime, string mapId, MouseEventArgs e) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.zoomIn", mapId, e);

        public static ValueTask ZoomOut(IJSRuntime jsRuntime, string mapId, MouseEventArgs e) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.zoomOut", mapId, e);

        public static ValueTask<LatLngBounds> GetBounds(IJSRuntime jsRuntime, string mapId, Layer path) =>
            jsRuntime.InvokeAsync<LatLngBounds>($"{baseObjectContainer}.getLayerBounds", mapId, path);

        public static ValueTask DisableInteraction(IJSRuntime jsRuntime, string mapId) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.disableInteraction", mapId);
        public static ValueTask EnableInteraction(IJSRuntime jsRuntime, string mapId) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.enableInteraction", mapId);


        public static ValueTask Dispose(IJSRuntime jsRuntime, string mapId) =>
            jsRuntime.InvokeVoidAsync($"{baseObjectContainer}.dispose", mapId);
    }
}
