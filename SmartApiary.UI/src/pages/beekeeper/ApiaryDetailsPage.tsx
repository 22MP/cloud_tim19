import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { getApiaryById } from "../../api/apiaries";
import { getHivesByApiary, deleteHive } from "../../api/hives";
import type { Apiary, Hive } from "../../types";
import HiveCard from "../../components/hives/HiveCard";
import HiveForm from "../../components/hives/HiveForm";
import {
  Button, Modal, EmptyState, Spinner, ConfirmDialog, Badge,
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
  const [loading, setLoading] = useState(true);
  const [showAddHive, setShowAddHive] = useState(false);
  const [editTarget, setEditTarget] = useState<Hive | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Hive | null>(null);
  const { toasts, addToast, removeToast } = useToast();

  const loadApiary = async () => {
    try {
      const res = await getApiaryById(id!);
      setApiary(res.data);
    } catch {
      addToast("Failed to load apiary.", "error");
    }
  };

  const loadHives = async () => {
    try {
      const res = await getHivesByApiary(id!);
      setHives(res.data);
    } catch {
      addToast("Failed to load hives.", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadApiary();
    loadHives();
  }, [id]);

  const handleDeleteHive = async (hiveId: string) => {
    try {
      await deleteHive(hiveId);
      addToast("Hive deleted.", "success");
      loadHives();
    } catch {
      addToast("Delete failed.", "error");
    }
  };

  return (
    <PageLayout title={apiary?.name}>
      <ToastContainer toasts={toasts} onRemove={removeToast} />

      {/* Back + header */}
      <div className="flex items-center gap-3 mb-6">
        <button
          onClick={() => navigate("/beekeeper/apiaries")}
          className="text-gray-400 hover:text-gray-600 transition text-sm flex items-center gap-1"
        >
          ← Back to Apiaries
        </button>
      </div>

      {apiary && (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
          {/* Apiary info card */}
          <div className="lg:col-span-1 bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
            {apiary.imageUrl ? (
              <img
                src={apiary.imageUrl}
                alt={apiary.name}
                className="w-full h-44 object-cover"
              />
            ) : (
              <div className="w-full h-44 bg-gradient-to-br from-amber-100 to-amber-200 flex items-center justify-center">
                <span className="text-6xl opacity-30">🏡</span>
              </div>
            )}
            <div className="p-5">
              <h2 className="text-xl font-bold text-gray-800 mb-1">{apiary.name}</h2>
              <p className="text-xs text-gray-400 mb-3">
                📍 {apiary.latitude.toFixed(5)}, {apiary.longitude.toFixed(5)}
              </p>
              {apiary.description && (
                <p className="text-sm text-gray-600 mb-3">{apiary.description}</p>
              )}
              <div className="flex gap-2">
                <Badge label={`${hives.length} hives`} variant="amber" />
                <Badge
                  label={`${hives.filter((h) => h.devicePaired).length} IoT paired`}
                  variant="green"
                />
              </div>
              <div className="mt-4">
                <Button
                  variant="secondary"
                  size="sm"
                  onClick={() => navigate(`/beekeeper/apiaries/${id}/telemetry`)}
                >
                  Open Telemetry & Alerts
                </Button>
              </div>
            </div>
          </div>

          {/* Map */}
          <div className="lg:col-span-2">
            <MapView
              position={{ lat: apiary.latitude, lng: apiary.longitude }}
              setPosition={() => {}}
              height="280px"
              readOnly
            />
          </div>
        </div>
      )}

      {/* Hives section */}
      <div className="flex items-center justify-between mb-6">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Hives</h2>
          <p className="text-gray-500 text-sm mt-0.5">{hives.length} hives in this apiary</p>
        </div>
        <Button onClick={() => setShowAddHive(true)} icon={<span>+</span>}>
          Add Hive
        </Button>
      </div>

      {loading ? (
        <Spinner />
      ) : hives.length === 0 ? (
        <EmptyState
          icon="🍯"
          title="No hives yet"
          description="Add your first hive to start tracking this apiary."
          action={
            <Button onClick={() => setShowAddHive(true)} icon={<span>+</span>}>
              Add First Hive
            </Button>
          }
        />
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
          {hives.map((h) => (
            <HiveCard
              key={h.id}
              hive={h}
              onEdit={() => setEditTarget(h)}
              onDelete={() => setDeleteTarget(h)}
            />
          ))}
        </div>
      )}

      {/* Add hive modal */}
      <Modal
        open={showAddHive}
        onClose={() => setShowAddHive(false)}
        title="Add New Hive"
      >
        <HiveForm
          apiaryId={id!}
          onSuccess={() => {
            setShowAddHive(false);
            addToast("Hive added!", "success");
            loadHives();
          }}
          onCancel={() => setShowAddHive(false)}
        />
      </Modal>

      {/* Edit hive modal */}
      <Modal
        open={!!editTarget}
        onClose={() => setEditTarget(null)}
        title="Edit Hive"
      >
        {editTarget && (
          <HiveForm
            apiaryId={id!}
            existing={editTarget}
            onSuccess={() => {
              setEditTarget(null);
              addToast("Hive updated!", "success");
              loadHives();
            }}
            onCancel={() => setEditTarget(null)}
          />
        )}
      </Modal>

      {/* Delete confirm */}
      <ConfirmDialog
        open={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={() => deleteTarget && handleDeleteHive(deleteTarget.id)}
        title="Delete Hive"
        message={`Delete hive "${deleteTarget?.label}"? This cannot be undone.`}
        confirmLabel="Delete Hive"
      />
    </PageLayout>
  );
}
