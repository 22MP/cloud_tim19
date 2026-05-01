import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getApiaries, deleteApiary } from "../../api/apiaries";
import type { Apiary } from "../../types";
import ApiaryCard from "../../components/apiaries/ApiaryCard";
import ApiaryForm from "../../components/apiaries/ApiaryForm";
import {
  Button, Modal, EmptyState, Spinner, ConfirmDialog, Input,
} from "../../components/common/ui";
import { PageLayout } from "../../components/layout/PageLayout";
import { ToastContainer } from "../../components/common/ui";
import { useToast } from "../../hooks/useToast";

export default function ApiariesPage() {
  const navigate = useNavigate();
  const [apiaries, setApiaries] = useState<Apiary[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAdd, setShowAdd] = useState(false);
  const [editTarget, setEditTarget] = useState<Apiary | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Apiary | null>(null);
  const [search, setSearch] = useState("");
  const { toasts, addToast, removeToast } = useToast();

  const load = async () => {
    try {
      const res = await getApiaries();
      setApiaries(res.data);
    } catch {
      addToast("Failed to load apiaries.", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleDelete = async (id: string) => {
    try {
      await deleteApiary(id);
      addToast("Apiary deleted.", "success");
      load();
    } catch {
      addToast("Delete failed.", "error");
    }
  };

  const filtered = apiaries.filter((a) =>
    !search || a.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <PageLayout title="My Apiaries">
      <ToastContainer toasts={toasts} onRemove={removeToast} />

      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-800">My Apiaries</h1>
          <p className="text-gray-500 text-sm mt-1">{apiaries.length} apiaries registered</p>
        </div>
        <Button onClick={() => setShowAdd(true)} icon={<span>+</span>}>
          Add Apiary
        </Button>
      </div>

      {/* Search */}
      {apiaries.length > 0 && (
        <div className="mb-6">
          <Input
            placeholder="Search apiaries..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            icon={<span>🔍</span>}
          />
        </div>
      )}

      {/* Content */}
      {loading ? (
        <Spinner />
      ) : filtered.length === 0 ? (
        <EmptyState
          icon="🏡"
          title={search ? "No apiaries match your search" : "No apiaries yet"}
          description={
            search
              ? "Try a different search term."
              : "Add your first apiary to start managing your hives."
          }
          action={
            !search && (
              <Button onClick={() => setShowAdd(true)} icon={<span>+</span>}>
                Add First Apiary
              </Button>
            )
          }
        />
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {filtered.map((a) => (
            <ApiaryCard
              key={a.id}
              apiary={a}
              onClick={() => navigate(`/beekeeper/apiaries/${a.id}`)}
              onEdit={() => setEditTarget(a)}
              onDelete={() => setDeleteTarget(a)}
            />
          ))}
        </div>
      )}

      {/* Add modal */}
      <Modal
        open={showAdd}
        onClose={() => setShowAdd(false)}
        title="Add New Apiary"
        maxWidth="max-w-2xl"
      >
        <ApiaryForm
          onSuccess={() => { setShowAdd(false); addToast("Apiary added!", "success"); load(); }}
          onCancel={() => setShowAdd(false)}
        />
      </Modal>

      {/* Edit modal */}
      <Modal
        open={!!editTarget}
        onClose={() => setEditTarget(null)}
        title="Edit Apiary"
        maxWidth="max-w-2xl"
      >
        {editTarget && (
          <ApiaryForm
            existing={editTarget}
            onSuccess={() => { setEditTarget(null); addToast("Apiary updated!", "success"); load(); }}
            onCancel={() => setEditTarget(null)}
          />
        )}
      </Modal>

      {/* Delete confirm */}
      <ConfirmDialog
        open={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={() => deleteTarget && handleDelete(deleteTarget.id)}
        title="Delete Apiary"
        message={`Delete "${deleteTarget?.name}"? All hives inside will also be removed. This cannot be undone.`}
        confirmLabel="Delete Apiary"
      />
    </PageLayout>
  );
}
