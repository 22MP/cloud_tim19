import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { useNavigate } from "react-router-dom";
import api from "../../api/axiosInstance";

interface User {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    role: string;
    status: string;
    createdAt: string;
}

interface RegisterForm {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    role: string;
}

export const UsersPage = () => {
    const { logout } = useAuth();
    const navigate = useNavigate();
    const [users, setUsers] = useState<User[]>([]);
    const [form, setForm] = useState<RegisterForm>({
        firstName: "", lastName: "", email: "", phoneNumber: "", role: "Beekeeper",
    });
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(false);

    const loadUsers = async () => {
        const res = await api.get("/users");
        setUsers(res.data);
    };

    useEffect(() => { loadUsers(); }, []);

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(""); setSuccess(""); setLoading(true);
        try {
            await api.post("/users/register", form);
            setSuccess("User registered! Activation email sent.");
            setForm({ firstName: "", lastName: "", email: "", phoneNumber: "", role: "Beekeeper" });
            loadUsers();
        } catch (err: any) {
            setError(err.response?.data?.message || "Registration failed.");
        } finally {
            setLoading(false);
        }
    };

    const handleSuspend = async (id: string) => {
        await api.patch(`/users/${id}/suspend`);
        loadUsers();
    };

    const handleReactivate = async (id: string) => {
        await api.patch(`/users/${id}/reactivate`);
        loadUsers();
    };

    const handleDelete = async (id: string) => {
        if (!confirm("Are you sure you want to delete this user?")) return;
        await api.delete(`/users/${id}`);
        loadUsers();
    };

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const statusColor = (status: string) => {
        if (status === "Active") return "bg-green-100 text-green-700";
        if (status === "Suspended") return "bg-red-100 text-red-700";
        return "bg-yellow-100 text-yellow-700";
    };

    const roleColor = (role: string) => {
        if (role === "Admin") return "bg-purple-100 text-purple-700";
        if (role === "Beekeeper") return "bg-amber-100 text-amber-700";
        return "bg-green-100 text-green-700";
    };

    return (
        <div className="min-h-screen bg-amber-50">

            {/* Navbar */}
            <nav className="bg-amber-700 text-white px-6 py-4 flex items-center justify-between">
                <div className="flex items-center gap-2">
                    <span className="font-bold text-lg">Smart Apiary - Admin</span>
                </div>
                <button
                    onClick={handleLogout}
                    className="bg-amber-600 hover:bg-amber-500 px-4 py-1.5 rounded-lg text-sm transition"
                >
                    Logout
                </button>
            </nav>

            <div className="max-w-6xl mx-auto px-6 py-8 space-y-8">

                {/* Register form */}
                <div className="bg-white rounded-2xl shadow p-6">
                    <h2 className="text-lg font-bold text-amber-800 mb-4">Register New User</h2>
                    <form onSubmit={handleRegister} className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <input
                            placeholder="First Name"
                            value={form.firstName}
                            onChange={(e) => setForm({ ...form, firstName: e.target.value })}
                            className="border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-amber-400"
                        />
                        <input
                            placeholder="Last Name"
                            value={form.lastName}
                            onChange={(e) => setForm({ ...form, lastName: e.target.value })}
                            className="border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-amber-400"
                        />
                        <input
                            placeholder="Email"
                            type="email"
                            value={form.email}
                            onChange={(e) => setForm({ ...form, email: e.target.value })}
                            className="border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-amber-400"
                        />
                        <input
                            placeholder="Phone (optional)"
                            value={form.phoneNumber}
                            onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })}
                            className="border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-amber-400"
                        />
                        <select
                            value={form.role}
                            onChange={(e) => setForm({ ...form, role: e.target.value })}
                            className="border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-amber-400"
                        >
                            <option value="Beekeeper">Beekeeper</option>
                            <option value="Farmer">Farmer</option>
                        </select>
                        <button
                            type="submit"
                            disabled={loading}
                            className="bg-amber-500 hover:bg-amber-600 text-white font-semibold rounded-lg py-2 transition disabled:opacity-50"
                        >
                            {loading ? "Registering..." : "Register User"}
                        </button>
                    </form>

                    {error && (
                        <div className="mt-3 bg-red-50 border border-red-200 text-red-600 rounded-lg px-4 py-2 text-sm">
                            {error}
                        </div>
                    )}
                    {success && (
                        <div className="mt-3 bg-green-50 border border-green-200 text-green-700 rounded-lg px-4 py-2 text-sm">
                            {success}
                        </div>
                    )}
                </div>

                {/* Users table */}
                <div className="bg-white rounded-2xl shadow p-6">
                    <h2 className="text-lg font-bold text-amber-800 mb-4">
                        All Users ({users.length})
                    </h2>
                    <div className="overflow-x-auto">
                        <table className="w-full text-sm">
                            <thead>
                                <tr className="border-b text-left text-gray-500">
                                    <th className="pb-3 font-medium">Name</th>
                                    <th className="pb-3 font-medium">Email</th>
                                    <th className="pb-3 font-medium">Role</th>
                                    <th className="pb-3 font-medium">Status</th>
                                    <th className="pb-3 font-medium">Registered</th>
                                    <th className="pb-3 font-medium">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {users.map((u) => (
                                    <tr key={u.id} className="border-b hover:bg-amber-50 transition">
                                        <td className="py-3 font-medium">{u.firstName} {u.lastName}</td>
                                        <td className="py-3 text-gray-500">{u.email}</td>
                                        <td className="py-3">
                                            <span className={`px-2 py-1 rounded-full text-xs font-medium ${roleColor(u.role)}`}>
                                                {u.role}
                                            </span>
                                        </td>
                                        <td className="py-3">
                                            <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColor(u.status)}`}>
                                                {u.status}
                                            </span>
                                        </td>
                                        <td className="py-3 text-gray-500">
                                            {new Date(u.createdAt).toLocaleDateString()}
                                        </td>
                                        <td className="py-3 flex gap-2">
                                            {u.status === "Active" && (
                                                <button
                                                    onClick={() => handleSuspend(u.id)}
                                                    className="bg-yellow-100 hover:bg-yellow-200 text-yellow-700 text-xs px-3 py-1 rounded-lg transition"
                                                >
                                                    Suspend
                                                </button>
                                            )}
                                            {u.status === "Suspended" && (
                                                <button
                                                    onClick={() => handleReactivate(u.id)}
                                                    className="bg-green-100 hover:bg-green-200 text-green-700 text-xs px-3 py-1 rounded-lg transition"
                                                >
                                                    Reactivate
                                                </button>
                                            )}
                                            <button
                                                onClick={() => handleDelete(u.id)}
                                                className="bg-red-100 hover:bg-red-200 text-red-600 text-xs px-3 py-1 rounded-lg transition"
                                            >
                                                Delete
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                        {users.length === 0 && (
                            <p className="text-center text-gray-400 py-8">No users registered yet.</p>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};