import { useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import api from "../api/axiosInstance";

export const ResetPasswordPage = () => {
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token") || "";
    const navigate = useNavigate();
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState(false);
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setLoading(true);
        try {
            await api.post("/auth/reset-password", { token, password, confirmPassword });
            setSuccess(true);
            setTimeout(() => navigate("/login"), 3000);
        } catch (err: any) {
            setError(err.response?.data?.message || "Reset failed.");
        } finally {
            setLoading(false);
        }
    };

    if (success)
        return (
            <div className="min-h-screen bg-amber-50 flex items-center justify-center">
                <div className="bg-white rounded-2xl shadow-lg p-8 text-center">
                    <h2 className="text-xl font-bold text-green-700">Password Reset!</h2>
                    <p className="text-gray-500 mt-2">Redirecting to login...</p>
                </div>
            </div>
        );

    return (
        <div className="min-h-screen bg-amber-50 flex items-center justify-center px-4">
            <div className="bg-white rounded-2xl shadow-lg p-8 w-full max-w-md">
                <div className="text-center mb-8">
                    <h1 className="text-2xl font-bold text-amber-800">Reset Password</h1>
                    <p className="text-gray-500 text-sm mt-1">Enter your new password</p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            New Password
                        </label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-amber-400"
                            placeholder="******"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Confirm Password
                        </label>
                        <input
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-amber-400"
                            placeholder="*******"
                        />
                    </div>

                    {error && (
                        <div className="bg-red-50 border border-red-200 text-red-600 rounded-lg px-4 py-2 text-sm">
                            {error}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={loading}
                        className="w-full bg-amber-500 hover:bg-amber-600 text-white font-semibold rounded-lg py-2 transition disabled:opacity-50"
                    >
                        {loading ? "Resetting..." : "Reset Password"}
                    </button>
                </form>
            </div>
        </div>
    );
};