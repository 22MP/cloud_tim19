import { useState } from "react";
import { useNavigate, useSearchParams, Link } from "react-router-dom";
import { activate } from "../api/auth";
import { Input, Button, Alert } from "../components/common/ui";

export const ActivatePage = () => {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") ?? "";
  const navigate = useNavigate();

  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    if (password !== confirmPassword) return setError("Passwords do not match.");
    if (password.length < 8) return setError("Password must be at least 8 characters.");

    setLoading(true);
    try {
      await activate(token, password, confirmPassword);
      setSuccess(true);
      setTimeout(() => navigate("/login"), 3000);
    } catch (err: any) {
      setError(err.response?.data?.message || "Activation failed. Link may be expired.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-amber-50 to-orange-50 flex items-center justify-center px-4">
      <div className="w-full max-w-md">
        <div className="bg-white rounded-3xl shadow-2xl p-8 border border-amber-100">
          <div className="text-center mb-8">
            <div className="w-16 h-16 bg-gradient-to-br from-amber-400 to-orange-500 rounded-2xl flex items-center justify-center mx-auto mb-4 shadow-lg">
              <span className="text-3xl">{success ? "✅" : "🔑"}</span>
            </div>
            <h1 className="text-2xl font-bold text-gray-800">
              {success ? "Account Activated!" : "Activate Account"}
            </h1>
            <p className="text-gray-500 text-sm mt-1">
              {success
                ? "Redirecting to login in 3 seconds..."
                : "Create your password to get started"}
            </p>
          </div>

          {success ? (
            <Alert message="Your account is now active. You can log in." variant="success" />
          ) : (
            <form onSubmit={handleSubmit} className="space-y-4">
              <Input
                label="New Password"
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Minimum 8 characters"
              />
              <Input
                label="Confirm Password"
                type="password"
                required
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                placeholder="Repeat your password"
              />

              {error && <Alert message={error} variant="error" />}

              <Button type="submit" loading={loading} className="w-full justify-center" size="lg">
                Activate Account
              </Button>

              <div className="text-center">
                <Link to="/login" className="text-sm text-amber-600 hover:underline">
                  Back to login
                </Link>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
};
