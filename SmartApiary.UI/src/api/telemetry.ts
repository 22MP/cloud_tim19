import * as signalR from "@microsoft/signalr";
import api from "./axiosInstance";

export interface TelemetrySample {
  id: string;
  deviceId: string;
  hiveId: string;
  weightKg: number;
  humidity: number;
  temperature: number;
  batteryPercentage: number;
  recordedAt: string;
}

export interface NectarDelta {
  date: string;
  weightAt8h?: number;
  weightAt20h?: number;
  deltaKg: number;
}

export interface TelemetryAlert {
  type: string;
  severity: string;
  message: string;
  timestamp: string;
  hiveId: string;
  deviceId?: string;
}

const API_ROOT = "https://localhost:7262";

export const getTelemetryByHive = (hiveId: string, from?: string, to?: string) =>
  api.get<TelemetrySample[]>(`/Telemetry/${hiveId}`, { params: { from, to } });

export const getLatestTelemetry = (hiveId: string) =>
  api.get<TelemetrySample>(`/Telemetry/${hiveId}/latest`);

export const getNectarDelta = (hiveId: string, from?: string, to?: string) =>
  api.get<NectarDelta[]>(`/Telemetry/${hiveId}/nectar-delta`, { params: { from, to } });

export const createTelemetryHubConnection = () => {
  const token = localStorage.getItem("token") ?? "";

  return new signalR.HubConnectionBuilder()
    .withUrl(`${API_ROOT}/notification-hub`, {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .build();
};
