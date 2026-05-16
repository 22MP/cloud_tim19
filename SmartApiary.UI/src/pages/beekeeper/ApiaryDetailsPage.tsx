import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import {
  getApiaryById,
  getUpcomingSprayings,
  getNearbyCultures,
} from "../../api/apiaries";
import { getHivesByApiary, deleteHive } from "../../api/hives";
import type {
  Apiary,
  Hive,
  UpcomingSprayingAlert,
  NearbyCulture,
} from "../../types";
import HiveCard from "../../components/hives/HiveCard";
import HiveForm from "../../components/hives/HiveForm";
import {
  Button,
  Modal,
  EmptyState,
  Spinner,
  ConfirmDialog,
  Badge,
  Alert,
} from "../../components/common/ui";
import { PageLayout } from "../../components/layout/PageLayout";
import { ToastContainer } from "../../components/common/ui";
import { useToast } from "../../hooks/useToast";
import MapView from "../../components/map/MapView";

export default function ApiaryDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [apiary, setApiary] = useState<Apiary | null>(null);
  const [hives, setHives] = useState<Hive[]>([]);
  const [alerts, setAlerts] = useState<UpcomingSprayingAlert[]>([]);
  const [cultures, setCultures] = useState<NearbyCulture[]>([]);
  const [loading, setLoading] = useState(true);

  const [showAddHive, setShowAddHive] = useState(false);
  const [editTarget, setEditTarget] = useState<Hive | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Hive | null>(null);

  const { toasts, addToast, removeToast } = useToast();

  const loadApiary = async () => {
    const res = await getApiaryById(id!);
    setApiary(res.data);
  };

  const loadHives = async () => {
    const res = await getHivesByApiary(id!);
    setHives(res.data);
  };

  const loadAlerts = async () => {
    const res = await getUpcomingSprayings(id!);
    setAlerts(res.data);
  };

  const loadCultures = async () => {
    const res = await getNearbyCultures(id!);
    setCultures(res.data);
  };

  useEffect(() => {
    const load = async () => {
      try {
        await Promise.all([
          loadApiary(),
          loadHives(),
          loadAlerts(),
          loadCultures(),
        ]);
      } catch {
        addToast("Failed to load apiary details.", "error");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [id]);

  const handleDeleteHive = async (hiveId: string) => {
    await deleteHive(hiveId);
    loadHives();
  };

  return (
    <PageLayout title={apiary?.name}>
      <ToastContainer toasts={toasts} onRemove={removeToast} />

      {/* Spraying alerts */}
      {alerts.length > 0 && (
        <div className="mb-6 space-y-3">
          {alerts.map((a) => (
            <Alert
              key={a.scheduleId}
              variant="warning"
              message={
                <span>
                  ⚠️ <strong>Upcoming spraying near your apiary!</strong>
                  <br />
                  Plot: <strong>{a.plotName}</strong>
                  <br />
                  Scheduled at:{" "}
                  <strong>
                    {new Date(a.scheduledAt).toLocaleString()}
                  </strong>
                </span> as any
              }
            />
          ))}
        </div>
      )}

      {loading || !apiary ? (
        <Spinner />
      ) : (
        <>
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
            <div className="lg:col-span-1 bg-white rounded-2xl border shadow-sm">
              <div className="p-5">
                <h2 className="text-xl font-bold">
                  {apiary.name}
                </h2>
                <p className="text-xs text-gray-400">
                  📍 {apiary.latitude.toFixed(5)},{" "}
                  {apiary.longitude.toFixed(5)}
                </p>
                <div className="flex gap-2 mt-3">
                  <Badge label={`${hives.length} hives`} variant="amber" />
                </div>
              </div>
            </div>

            <div className="lg:col-span-2">
              <MapView
                position={{
                  lat: apiary.latitude,
                  lng: apiary.longitude,
                }}
                nearbyCultures={cultures}
                height="300px"
                readOnly
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
            {hives.map((h) => (
              <HiveCard
                key={h.id}
                hive={h}
                onEdit={() => setEditTarget(h)}
                onDelete={() => setDeleteTarget(h)}
                onRegisterDevice={() => {}}
              />
            ))}
          </div>
        </>
      )}
    </PageLayout>
  );
}