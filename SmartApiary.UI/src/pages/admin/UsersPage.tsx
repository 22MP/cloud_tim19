import { useEffect, useState } from "react";
import { getUsers, registerUser, suspendUser, reactivateUser, deleteUser } from "../../api/users";
import type { User } from "../../types";
import {
  Button, Input, Select, Alert, Badge, Card, Modal, EmptyState, Spinner, ConfirmDialog,
} from "../../components/common/ui";
import { PageLayout } from "../../components/layout/PageLayout";
import { useToast } from "../../hooks/useToast";
import { ToastContainer } from "../../components/common/ui";

const ROLE_OPTIONS = [
  { value: "Beekeeper", label: "Beekeeper" },
  { value: "Farmer", label: "Farmer" },
];

const statusVariant = (s: string): "green" | "red" | "yellow" =>
  s === "Active" ? "green" : s === "Suspended" ? "red" : "yellow";

const roleVariant = (r: string): "purple" | "amber" | "green" =>
  r === "Admin" ? "purple" : r === "Beekeeper" ? "amber" : "green";

const emptyForm = { firstName: "", lastName: "", email: "", phoneNumber: "", role: "Beekeeper" };

export const UsersPage = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [formLoading, setFormLoading] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const [formError, setFormError] = useState("");
  const [search, setSearch] = useState("");
  const [deleteTarget, setDeleteTarget] = useState<User | null>(null);
  const { toasts, addToast, removeToast } = useToast();

  const load = async () => {
    try {
      const res = await getUsers();
      setUsers(res.data);
    } catch {
      addToast("Failed to load users.", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError("");
    if (!form.firstName || !form.lastName || !form.email) {
      return setFormError("First name, last name and email are required.");
    }
    setFormLoading(true);
    try {
      await registerUser(form);
      addToast("User registered! Activation email sent.", "success");
      setForm(emptyForm);
      setShowForm(false);
      load();
    } catch (err: any) {
      setFormError(err.response?.data?.message || "Registration failed.");
    } finally {
      setFormLoading(false);
    }
  };

  const handleSuspend = async (id: string) => {
    try { await suspendUser(id); addToast("User suspended.", "warning"); load(); }
    catch { addToast("Action failed.", "error"); }
  };

  const handleReactivate = async (id: string) => {
    try { await reactivateUser(id); addToast("User reactivated.", "success"); load(); }
    catch { addToast("Action failed.", "error"); }
  };

  const handleDelete = async (id: string) => {
    try { await deleteUser(id); addToast("User deleted.", "success"); load(); }
    catch { addToast("Delete failed.", "error"); }
  };

  const filtered = users.filter((u) => {
    const q = search.toLowerCase();
    return (
      !q ||
      u.firstName.toLowerCase().includes(q) ||
      u.lastName.toLowerCase().includes(q) ||
      u.email.toLowerCase().includes(q)
    );
  });

  return (
    <PageLayout title="User Management">
      <ToastContainer toasts={toasts} onRemove={removeToast} />

      {/* Page header */}
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-800">Users</h1>
          <p className="text-gray-500 text-sm mt-1">{users.length} total users in the system</p>
        </div>
        <Button onClick={() => setShowForm(true)} icon={<span>+</span>}>
          Register User
        </Button>
      </div>

      {/* Stats row */}
      <div className="grid grid-cols-3 gap-4 mb-8">
        {[
          { label: "Total", count: users.length, color: "bg-amber-50 border-amber-200", text: "text-amber-700" },
          { label: "Active", count: users.filter((u) => u.status === "Active").length, color: "bg-green-50 border-green-200", text: "text-green-700" },
          { label: "Suspended", count: users.filter((u) => u.status === "Suspended").length, color: "bg-red-50 border-red-200", text: "text-red-700" },
        ].map((s) => (
          <Card key={s.label} className={`p-4 border ${s.color}`}>
            <p className={`text-2xl font-bold ${s.text}`}>{s.count}</p>
            <p className="text-sm text-gray-500">{s.label} users</p>
          </Card>
        ))}
      </div>

      {/* Search */}
      <div className="mb-4">
        <Input
          placeholder="Search by name or email..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          icon={<span>🔍</span>}
        />
      </div>

      {/* Table */}
      <Card>
        {loading ? (
          <Spinner />
        ) : filtered.length === 0 ? (
          <EmptyState icon="👤" title="No users found" description="Register a new user to get started." />
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-100">
                  {["Name", "Email", "Phone", "Role", "Status", "Registered", "Actions"].map((h) => (
                    <th key={h} className="px-4 py-3 text-left text-xs font-semibold text-gray-400 uppercase tracking-wide">
                      {h}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {filtered.map((u) => (
                  <tr key={u.id} className="border-b border-gray-50 hover:bg-amber-50/40 transition">
                    <td className="px-4 py-3 font-medium text-gray-800">
                      {u.firstName} {u.lastName}
                    </td>
                    <td className="px-4 py-3 text-gray-500">{u.email}</td>
                    <td className="px-4 py-3 text-gray-400">{u.phoneNumber || "—"}</td>
                    <td className="px-4 py-3">
                      <Badge label={u.role} variant={roleVariant(u.role)} />
                    </td>
                    <td className="px-4 py-3">
                      <Badge label={u.status} variant={statusVariant(u.status)} />
                    </td>
                    <td className="px-4 py-3 text-gray-400 text-xs">
                      {new Date(u.createdAt).toLocaleDateString("en-GB")}
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex gap-2">
                        {u.status === "Active" && (
                          <Button variant="warning" size="sm" onClick={() => handleSuspend(u.id)}>
                            Suspend
                          </Button>
                        )}
                        {u.status === "Suspended" && (
                          <Button variant="ghost" size="sm" onClick={() => handleReactivate(u.id)}>
                            Reactivate
                          </Button>
                        )}
                        <Button variant="danger" size="sm" onClick={() => setDeleteTarget(u)}>
                          Delete
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Card>

      {/* Register modal */}
      <Modal open={showForm} onClose={() => { setShowForm(false); setFormError(""); setForm(emptyForm); }} title="Register New User">
        <form onSubmit={handleRegister} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Input
              label="First Name *"
              value={form.firstName}
              onChange={(e) => setForm({ ...form, firstName: e.target.value })}
              placeholder="John"
            />
            <Input
              label="Last Name *"
              value={form.lastName}
              onChange={(e) => setForm({ ...form, lastName: e.target.value })}
              placeholder="Doe"
            />
          </div>
          <Input
            label="Email *"
            type="email"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            placeholder="user@example.com"
          />
          <Input
            label="Phone (optional)"
            type="tel"
            value={form.phoneNumber}
            onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })}
            placeholder="+381..."
          />
          <Select
            label="Role *"
            value={form.role}
            onChange={(e) => setForm({ ...form, role: e.target.value })}
            options={ROLE_OPTIONS}
          />

          {formError && <Alert message={formError} variant="error" />}

          <div className="flex gap-3 pt-2">
            <Button type="submit" loading={formLoading} className="flex-1">
              Register & Send Email
            </Button>
            <Button variant="secondary" onClick={() => setShowForm(false)}>
              Cancel
            </Button>
          </div>
        </form>
      </Modal>

      {/* Delete confirm */}
      <ConfirmDialog
        open={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={() => deleteTarget && handleDelete(deleteTarget.id)}
        title="Delete User"
        message={`Are you sure you want to delete ${deleteTarget?.firstName} ${deleteTarget?.lastName}? This action cannot be undone.`}
        confirmLabel="Delete User"
        variant="danger"
      />
    </PageLayout>
  );
};
