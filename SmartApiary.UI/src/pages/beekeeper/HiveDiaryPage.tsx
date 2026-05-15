import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import {
  getInspections,
  createInspection,
  updateInspection,
  deleteInspection,
  type HiveInspection,
} from "../../api/hiveInspections";
import { getHiveById } from "../../api/hives";
import { PageLayout } from "../../components/layout/PageLayout";
import {
  Button,
  Card,
  Modal,
  Input,
  Textarea,
  ConfirmDialog,
  EmptyState,
  Spinner,
} from "../../components/common/ui";

export default function HiveDiaryPage() {
  const { hiveId } = useParams<{ hiveId: string }>();
  const navigate = useNavigate();

  const [hive, setHive] = useState<any>(null);
  const [inspections, setInspections] = useState<HiveInspection[]>([]);
  const [loading, setLoading] = useState(true);

  const [showModal, setShowModal] = useState(false);
  const [editTarget, setEditTarget] = useState<HiveInspection | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<HiveInspection | null>(null);

  const load = async () => {
    if (!hiveId) return;

    const hiveRes = await getHiveById(hiveId);
    setHive(hiveRes.data);

    const res = await getInspections(hiveId);
    setInspections(res.data);

    setLoading(false);
  };

  useEffect(() => {
    load();
  }, [hiveId]);

  return (
    <PageLayout title="Hive Diary">
      <button
        onClick={() => navigate(-1)}
        className="text-gray-500 text-sm mb-4"
      >
        ← Back
      </button>

      {hive && (
        <Card className="p-5 mb-6">
          <h2 className="text-xl font-bold">{hive.label}</h2>
          <p>Type: {hive.type}</p>
          {hive.queenAge && (
            <p>Queen age: {hive.queenAge} years</p>
          )}
        </Card>
      )}

      <div className="flex justify-between items-center mb-6">
        <h3 className="text-lg font-bold">Inspections</h3>
        <Button
          onClick={() => {
            setEditTarget(null);
            setShowModal(true);
          }}
        >
          Add Inspection
        </Button>
      </div>

      {loading ? (
        <Spinner />
      ) : inspections.length === 0 ? (
        <EmptyState icon="📖" title="No inspections yet" />
      ) : (
        inspections.map((i) => (
          <Card key={i.id} className="p-4 mb-4">
            <div className="flex justify-between items-start">
              <div>
                <p className="font-semibold">
                  {new Date(i.inspectionDate).toLocaleString()}
                </p>
              </div>

              <Button
                variant="secondary"
                size="sm"
                onClick={() => {
                  setEditTarget(i);
                  setShowModal(true);
                }}
              >
                Edit
              </Button>
            </div>

            <div className="mt-3 space-y-1 text-sm">
              <p>Bottom board color: {i.bottomBoardColor}</p>
              <p>Honey frames: {i.honeyFrames}</p>
              <p>Honey: {i.honeyKg} kg</p>
              <p>Brood frames: {i.broodFrames}</p>
              <p>Queen present: {i.queenPresent ? "Yes" : "No"}</p>
              {i.notes && <p>Notes: {i.notes}</p>}
            </div>

            <div className="mt-4 flex justify-end">
              <Button
                variant="danger"
                size="sm"
                onClick={() => setDeleteTarget(i)}
              >
                Delete
              </Button>
            </div>
          </Card>
        ))
      )}

      <Modal
        open={showModal}
        onClose={() => setShowModal(false)}
        title={editTarget ? "Edit Inspection" : "New Inspection"}
      >
        <InspectionForm
          hiveId={hiveId!}
          editTarget={editTarget}
          onSuccess={() => {
            setShowModal(false);
            load();
          }}
        />
      </Modal>

      <ConfirmDialog
        open={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={async () => {
          if (!deleteTarget) return;
          await deleteInspection(hiveId!, deleteTarget.id);
          load();
        }}
        title="Delete Inspection"
        message="Are you sure you want to delete this inspection?"
      />
    </PageLayout>
  );
}

function InspectionForm({
  hiveId,
  editTarget,
  onSuccess,
}: {
  hiveId: string;
  editTarget: HiveInspection | null;
  onSuccess: () => void;
}) {
  const [inspectionDate, setDate] = useState(
    editTarget
      ? editTarget.inspectionDate.slice(0, 16)
      : new Date().toISOString().slice(0, 16)
  );
  const [bottomBoardColor, setColor] = useState(
    editTarget?.bottomBoardColor ?? ""
  );
  const [honeyFrames, setHoneyFrames] = useState(
    editTarget?.honeyFrames ?? 0
  );
  const [honeyKg, setHoneyKg] = useState(
    editTarget?.honeyKg ?? 0
  );
  const [broodFrames, setBroodFrames] = useState(
    editTarget?.broodFrames ?? 0
  );
  const [queenPresent, setQueenPresent] = useState(
    editTarget?.queenPresent ?? true
  );
  const [notes, setNotes] = useState(
    editTarget?.notes ?? ""
  );

  const handleSubmit = async () => {
    const payload = {
      inspectionDate,
      bottomBoardColor,
      honeyFrames,
      honeyKg,
      broodFrames,
      queenPresent,
      notes,
    };

    if (editTarget) {
      await updateInspection(hiveId, editTarget.id, payload);
    } else {
      await createInspection(hiveId, payload);
    }

    onSuccess();
  };

  return (
    <div className="space-y-4">
      <Input
        type="datetime-local"
        label="Date and time"
        value={inspectionDate}
        onChange={(e) => setDate(e.target.value)}
      />

      <Input
        label="Bottom board color"
        value={bottomBoardColor}
        onChange={(e) => setColor(e.target.value)}
      />

      <Input
        type="number"
        label="Honey frames"
        value={honeyFrames}
        onChange={(e) => setHoneyFrames(Number(e.target.value))}
      />

      <Input
        type="number"
        label="Honey (kg)"
        value={honeyKg}
        onChange={(e) => setHoneyKg(Number(e.target.value))}
      />

      <Input
        type="number"
        label="Brood frames"
        value={broodFrames}
        onChange={(e) => setBroodFrames(Number(e.target.value))}
      />

      <label className="flex items-center gap-2">
        <input
          type="checkbox"
          checked={queenPresent}
          onChange={(e) => setQueenPresent(e.target.checked)}
        />
        Queen present
      </label>

      <Textarea
        label="Notes"
        value={notes}
        onChange={(e) => setNotes(e.target.value)}
      />

      <Button onClick={handleSubmit}>
        {editTarget ? "Update" : "Save"}
      </Button>
    </div>
  );
}