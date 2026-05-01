import api from "./axiosInstance";

export const getUsers = () => api.get("/users");

export const registerUser = (data: {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  role: string;
}) => api.post("/users/register", data);

export const suspendUser = (id: string) => api.patch(`/users/${id}/suspend`);

export const reactivateUser = (id: string) => api.patch(`/users/${id}/reactivate`);

export const deleteUser = (id: string) => api.delete(`/users/${id}`);
