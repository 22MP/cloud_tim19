import { useState } from "react";
import api from "../api/axiosInstance";

export const ForgotPasswordPage = () => {
    const [email, setEmail] = useState("");
    const [sent, setSent] = useState(false);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setLoading(true);
        try {
            await api.post("/auth/forgot-password", { email });
            setSent(true);
        } catch (err: any) {
            setError(err.response?.data?.message || "Something went wrong.");
        } finally {
            setLoading(false);
        }
    };

    if (sent)
        return (
            <div className="min-h-screen bg-amber-50 flex items-center justify-center">
                <div className="bg-white rounded-2xl shadow-lg p-8 text-center max-w-md">
                    <h2 className="text-xl font-bold text-amber-800">Check your inbox</h2>
                    <p className="text-gray-500 mt-2">
                        If an account exists for {email}, a reset link has been sent.
                    </p>
                    <a href="/login" className="mt-4 inline-block text-amber-600 hover:underline text-sm">
                        Back to login
                    </a>
                </div>
            </div>
        );

    return (
        <div className="min-h-screen bg-amber-50 flex items-center justify-center px-4">
            <div className="bg-white rounded-2xl shadow-lg p-8 w-full max-w-md">
                <div className="text-center mb-8">
                    <h1 className="text-2xl font-bold text-amber-800">Forgot Password</h1>
                    <p className="text-gray-500 text-sm mt-1">
                        Enter your email and we'll send you a reset link
                    </p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-amber-400"
                            placeholder="your@email.com"
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
                        {loading ? "Sending..." : "Send Reset Link"}
                    </button>

                    <div className="text-center">
                        <a href="/login" className="text-sm text-amber-600 hover:underline">
                            Back to login
                        </a>
                    </div>
                </form>
            </div>
        </div>
    );
};