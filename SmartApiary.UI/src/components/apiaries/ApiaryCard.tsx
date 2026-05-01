import type { Apiary } from "../../types";
import { Button, Badge } from "../common/ui";

interface ApiaryCardProps {
  apiary: Apiary;
  onClick: () => void;
  onDelete: () => void;
  onEdit: () => void;
}

export default function ApiaryCard({ apiary, onClick, onDelete, onEdit }: ApiaryCardProps) {
  return (
    <div className="bg-white rounded-2xl border border-gray-100 shadow-sm hover:shadow-md transition-all duration-200 overflow-hidden group">
      {/* Image */}
      <div
        className="relative h-40 bg-gradient-to-br from-amber-100 to-amber-200 cursor-pointer"
        onClick={onClick}
      >
        {apiary.thumbnailUrl || apiary.imageUrl ? (
          <img
            src={apiary.thumbnailUrl || apiary.imageUrl}
            alt={apiary.name}
            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
          />
        ) : (
          <div className="flex items-center justify-center h-full">
            <span className="text-5xl opacity-30">🏡</span>
          </div>
        )}
        <div className="absolute top-3 right-3">
          <Badge label={`${apiary.hiveCount ?? 0} hives`} variant="amber" />
        </div>
      </div>

      {/* Content */}
      <div className="p-4">
        <h3
          className="font-bold text-gray-800 text-lg mb-1 cursor-pointer hover:text-amber-700 transition"
          onClick={onClick}
        >
          {apiary.name}
        </h3>

        <div className="flex items-center gap-1 text-gray-400 text-xs mb-1">
          <span>📍</span>
          <span>
            {apiary.latitude.toFixed(4)}, {apiary.longitude.toFixed(4)}
          </span>
        </div>

        {apiary.description && (
          <p className="text-gray-500 text-sm line-clamp-2 mt-1">{apiary.description}</p>
        )}

        {/* Actions */}
        <div className="flex gap-2 mt-4 pt-4 border-t border-gray-50">
          <Button variant="primary" size="sm" className="flex-1" onClick={onClick}>
            Open
          </Button>
          <Button variant="secondary" size="sm" onClick={onEdit}>
            Edit
          </Button>
          <Button variant="danger" size="sm" onClick={onDelete}>
            Delete
          </Button>
        </div>
      </div>
    </div>
  );
}
