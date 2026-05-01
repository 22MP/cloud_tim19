import { useState, useEffect } from "react";
import { Input, Textarea, Button, Alert } from "../common/ui";
import MapView, { type LatLng } from "../map/MapView";
import { createApiary, updateApiary, uploadApiaryImage } from "../../api/apiaries";
import type { Apiary } from "../../types";

interface ApiaryFormProps {
  existing?: Apiary;
  onSuccess: () => void;
  onCancel?: () => void;
}

export default function ApiaryForm({ existing, onSuccess, onCancel }: ApiaryFormProps) {
  const [name, setName] = useState(existing?.name ?? "");
  const [description, setDescription] = useState(existing?.description ?? "");
  const [position, setPosition] = useState<LatLng | null>(
    existing ? { lat: existing.latitude, lng: existing.longitude } : null
  );
  const [file, setFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(existing?.imageUrl ?? null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!file) return;
    const url = URL.createObjectURL(file);
    setPreview(url);
    return () => URL.revokeObjectURL(url);
  }, [file]);

  const handleSubmit = async () => {
    setError("");
    if (!name.trim()) return setError("Name is required.");
    if (!position) return setError("Please select a location on the map.");

    setLoading(true);
    try {
      const payload = {
        name: name.trim(),
        latitude: position.lat,
        longitude: position.lng,
        description: description.trim() || undefined,
      };

      let id = existing?.id;
      if (existing) {
        await updateApiary(existing.id, payload);
      } else {
        const res = await createApiary(payload);
        id = res.data?.id;
      }

      if (file && id) {
        await uploadApiaryImage(id, file);
      }

      onSuccess();
    } catch (err: any) {
      setError(err.response?.data?.message || "Failed to save apiary.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-5">
      <Input
        label="Apiary Name *"
        placeholder="e.g. Main Apiary"
        value={name}
        onChange={(e) => setName(e.target.value)}
      />

      <Textarea
        label="Description (optional)"
        placeholder="Describe the terrain, surroundings..."
        value={description}
        onChange={(e) => setDescription(e.target.value)}
        rows={3}
      />

      {/* Image upload */}
      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium text-gray-700">Photo (optional)</label>
        <label className="flex items-center gap-3 cursor-pointer border-2 border-dashed border-gray-300 rounded-xl p-4 hover:border-amber-400 transition">
          <span className="text-2xl">📷</span>
          <div>
            <p className="text-sm text-gray-600 font-medium">Upload image</p>
            <p className="text-xs text-gray-400">PNG, JPG up to 10MB</p>
          </div>
          <input
            type="file"
            accept="image/*"
            className="hidden"
            onChange={(e) => setFile(e.target.files?.[0] ?? null)}
          />
        </label>
        {preview && (
          <img
            src={preview}
            alt="Preview"
            className="h-32 w-full object-cover rounded-xl border border-gray-100 mt-2"
          />
        )}
      </div>

      {/* Map */}
      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium text-gray-700">Location *</label>
        {position && (
          <p className="text-xs text-amber-600 mb-1">
            Selected: {position.lat.toFixed(6)}, {position.lng.toFixed(6)}
          </p>
        )}
        <MapView position={position} setPosition={setPosition} height="280px" />
      </div>

      {error && <Alert message={error} variant="error" />}

      <div className="flex gap-3 pt-2">
        <Button onClick={handleSubmit} loading={loading} className="flex-1">
          {existing ? "Update Apiary" : "Add Apiary"}
        </Button>
        {onCancel && (
          <Button variant="secondary" onClick={onCancel}>
            Cancel
          </Button>
        )}
      </div>
    </div>
  );
}
