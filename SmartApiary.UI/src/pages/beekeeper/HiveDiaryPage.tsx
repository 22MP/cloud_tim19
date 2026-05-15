import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import {
  getInspections,
  createInspection,
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
          <p>Tip: {hive.type}</p>
          {hive.queenAge && <p>Starost matice: {hive.queenAge} godina</p>}
        </Card>
      )}

      <div className="flex justify-between items-center mb-6">
        <h3 className="text-lg font-bold">Pregledi</h3>
        <Button onClick={() => setShowModal(true)}>Dodaj novi pregled</Button>
      </div>

      {loading ? (
        <Spinner />
      ) : inspections.length === 0 ? (
        <EmptyState icon="📖" title="Nema pregleda" />
      ) : (
        inspections.map((i) => (
          <Card key={i.id} className="p-4 mb-3">
            <div className="flex justify-between">
              <div>
                <p className="font-semibold">
                  {new Date(i.inspectionDate).toLocaleString()}
                </p>
                <p>Med: {i.honeyKg} kg</p>
                <p>Ramovi med: {i.honeyFrames}</p>
                <p>Ramovi leglo: {i.broodFrames}</p>
                <p>Matica: {i.queenPresent ? "Da" : "Ne"}</p>
                {i.notes && <p className="text-gray-500">{i.notes}</p>}
              </div>
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
        title="Novi pregled"
      >
        <InspectionForm
          hiveId={hiveId!}
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
        title="Delete inspection"
        message="Are you sure?"
      />
    </PageLayout>
  );
}

function InspectionForm({
  hiveId,
  onSuccess,
}: {
  hiveId: string;
  onSuccess: () => void;
}) {
  const [inspectionDate, setDate] = useState(
    new Date().toISOString().slice(0, 16)
  );
  const [bottomBoardColor, setColor] = useState("");
  const [honeyFrames, setHoneyFrames] = useState(0);
  const [honeyKg, setHoneyKg] = useState(0);
  const [broodFrames, setBroodFrames] = useState(0);
  const [queenPresent, setQueenPresent] = useState(true);
  const [notes, setNotes] = useState("");

  const handleSubmit = async () => {
    await createInspection(hiveId, {
      inspectionDate,
      bottomBoardColor,
      honeyFrames,
      honeyKg,
      broodFrames,
      queenPresent,
      notes,
    });

    onSuccess();
  };

  return (
    <div className="space-y-4">
      <Input
        type="datetime-local"
        label="Datum i vreme"
        value={inspectionDate}
        onChange={(e) => setDate(e.target.value)}
      />

      <Input
        label="Boja podnjače"
        value={bottomBoardColor}
        onChange={(e) => setColor(e.target.value)}
      />

      <Input
        type="number"
        label="Ramovi sa medom"
        value={honeyFrames}
        onChange={(e) => setHoneyFrames(Number(e.target.value))}
      />

      <Input
        type="number"
        label="Med (kg)"
        value={honeyKg}
        onChange={(e) => setHoneyKg(Number(e.target.value))}
      />

      <Input
        type="number"
        label="Ramovi sa leglom"
        value={broodFrames}
        onChange={(e) => setBroodFrames(Number(e.target.value))}
      />

      <label className="flex items-center gap-2">
        <input
          type="checkbox"
          checked={queenPresent}
          onChange={(e) => setQueenPresent(e.target.checked)}
        />
        Matica prisutna
      </label>

      <Textarea
        label="Napomena"
        value={notes}
        onChange={(e) => setNotes(e.target.value)}
      />

      <Button onClick={handleSubmit}>Sačuvaj</Button>
    </div>
  );
}