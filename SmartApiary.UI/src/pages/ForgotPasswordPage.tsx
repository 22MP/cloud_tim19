import { useState } from "react";
import { Link } from "react-router-dom";
import { forgotPassword } from "../api/auth";
import { Input, Button, Alert } from "../components/common/ui";

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
      await forgotPassword(email);
      setSent(true);
    } catch (err: any) {
      setError(err.response?.data?.message || "Something went wrong.");
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
              <span className="text-3xl">{sent ? "📬" : "📧"}</span>
            </div>
            <h1 className="text-2xl font-bold text-gray-800">
              {sent ? "Check your inbox" : "Forgot Password"}
            </h1>
            <p className="text-gray-500 text-sm mt-1">
              {sent
                ? `We sent a reset link to ${email}`
                : "Enter your email and we'll send a reset link"}
            </p>
          </div>

          {sent ? (
            <div className="space-y-4">
              <Alert
                message="If an account exists for this email, a reset link has been sent."
                variant="success"
              />
              <Link
                to="/login"
                className="block text-center text-sm text-amber-600 hover:underline mt-4"
              >
                Back to login
              </Link>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-4">
              <Input
                label="Email address"
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="your@email.com"
              />

              {error && <Alert message={error} variant="error" />}

              <Button type="submit" loading={loading} className="w-full justify-center" size="lg">
                Send Reset Link
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
