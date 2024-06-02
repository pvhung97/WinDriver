﻿namespace UIADriver.services.pattern
{
    public abstract class TransformPattern2Service<T, U> : PatternService<T, U>
    {
        protected TransformPattern2Service(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void Zoom(string elementId, double value);
        public abstract void ZoomByUnit(string elementId, string zoomUnit);
        public abstract bool CanZoom(string elementId);
        public abstract double GetZoomLevel(string elementId);
        public abstract double GetZoomMinimum(string elementId);
        public abstract double GetZoomMaximum(string elementId);
    }
}
