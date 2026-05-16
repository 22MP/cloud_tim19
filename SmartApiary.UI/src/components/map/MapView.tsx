import { useEffect } from "react";
import {
  MapContainer,
  TileLayer,
  Marker,
  useMap,
} from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";

export type LatLng = { lat: number; lng: number };

interface NearbyCulture {
  plotId: string;
  plotName: string;
  latitude: number;
  longitude: number;
  cultureType: number;
}

interface MapViewProps {
  position: LatLng | null;
  nearbyCultures?: NearbyCulture[];
  height?: string;
  readOnly?: boolean;
}

function MapUpdater({ position }: { position: LatLng | null }) {
  const map = useMap();
  useEffect(() => {
    if (position) map.setView([position.lat, position.lng], 13);
  }, [position]);
  return null;
}

const cultureIcons: Record<number, string> = {
  1: "🌻",
  2: "🌼",
  3: "💜",
  4: "🍀",
  5: "🌿",
  6: "🌽",
  7: "🌾",
};

export default function MapView({
  position,
  nearbyCultures = [],
  height = "260px",
}: MapViewProps) {
  return (
    <div className="rounded-xl overflow-hidden border shadow-sm">
      <MapContainer
        center={
          position ? [position.lat, position.lng] : [44.8, 20.5]
        }
        zoom={13}
        style={{ height, width: "100%" }}
      >
        <TileLayer
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          attribution="© OpenStreetMap"
        />

        {position && (
          <Marker position={[position.lat, position.lng]} />
        )}

        {nearbyCultures.map((c) => (
          <Marker
            key={c.plotId}
            position={[c.latitude, c.longitude]}
            icon={L.divIcon({
              html: `<div style="font-size:24px">${cultureIcons[c.cultureType] ?? "🌱"}</div>`,
              className: "",
            })}
          />
        ))}

        <MapUpdater position={position} />
      </MapContainer>
    </div>
  );
}