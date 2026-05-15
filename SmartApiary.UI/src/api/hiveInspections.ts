import api from "./axiosInstance";

export interface HiveInspection {
  id: string;
  hiveId: string;
  inspectionDate: string;
  bottomBoardColor: string;
  honeyFrames: number;
  honeyKg: number;
  broodFrames: number;
  queenPresent: boolean;
  notes?: string;
  createdAt: string;
}

export const getInspections = (hiveId: string) =>
  api.get<HiveInspection[]>(`/hives/${hiveId}/inspections`);

export const createInspection = (hiveId: string, data: any) =>
  api.post(`/hives/${hiveId}/inspections`, data);

export const updateInspection = (
  hiveId: string,
  inspectionId: string,
  data: any
) =>
  api.put(`/hives/${hiveId}/inspections/${inspectionId}`, data);

export const deleteInspection = (
  hiveId: string,
  inspectionId: string
) =>
  api.delete(`/hives/${hiveId}/inspections/${inspectionId}`);