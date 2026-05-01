import api from "./axiosInstance";

export const login = (email: string, password: string) =>
  api.post("/auth/login", { email, password });

export const activate = (token: string, password: string, confirmPassword: string) =>
  api.post("/auth/activate", { token, password, confirmPassword });

export const forgotPassword = (email: string) =>
  api.post("/auth/forgot-password", { email });

export const resetPassword = (token: string, password: string, confirmPassword: string) =>
  api.post("/auth/reset-password", { token, password, confirmPassword });
