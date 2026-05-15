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

export const createInspection = (
  hiveId: string,
  data: {
    inspectionDate: string;
    bottomBoardColor: string;
    honeyFrames: number;
    honeyKg: number;
    broodFrames: number;
    queenPresent: boolean;
    notes?: string;
  }
) =>
  api.post(`/hives/${hiveId}/inspections`, data);

export const deleteInspection = (hiveId: string, inspectionId: string) =>
  api.delete(`/hives/${hiveId}/inspections/${inspectionId}`);