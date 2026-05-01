import { useEffect } from "react";
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";

L.Icon.Default.mergeOptions({ iconUrl: markerIcon, shadowUrl: markerShadow });

export type LatLng = { lat: number; lng: number };

function LocationPicker({ setPosition }: { setPosition: (pos: LatLng) => void }) {
  useMapEvents({ click: (e) => setPosition({ lat: e.latlng.lat, lng: e.latlng.lng }) });
  return null;
}

function MapUpdater({ position }: { position: LatLng | null }) {
  const map = useMap();
  useEffect(() => {
    if (position) map.setView([position.lat, position.lng], 14);
  }, [position, map]);
  return null;
}

interface MapViewProps {
  position: LatLng | null;
  setPosition: (pos: LatLng) => void;
  height?: string;
  readOnly?: boolean;
}

export default function MapView({
  position,
  setPosition,
  height = "260px",
  readOnly = false,
}: MapViewProps) {
  return (
    <div className="rounded-xl overflow-hidden border border-gray-200 shadow-sm">
      <MapContainer
        center={position ? [position.lat, position.lng] : [44.8, 20.5]}
        zoom={position ? 13 : 7}
        style={{ height, width: "100%" }}
      >
        <TileLayer
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          attribution='&copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a>'
        />
        {position && <Marker position={[position.lat, position.lng]} />}
        {!readOnly && <LocationPicker setPosition={setPosition} />}
        <MapUpdater position={position} />
      </MapContainer>
      {!readOnly && (
        <p className="text-xs text-gray-400 text-center py-1.5 bg-gray-50 border-t border-gray-100">
          Click on the map to set location
        </p>
      )}
    </div>
  );
}
